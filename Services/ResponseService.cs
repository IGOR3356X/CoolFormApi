using CoolFormApi.DTO.Response;
using CoolFormApi.Interfaces;
using CoolFormApi.Interfaces.IServices;
using CoolFormApi.Mappers.MapForms;
using CoolFormApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoolFormApi.Services;

public class ResponseService : IResponseService
{
    private readonly IGenericRepository<Response> _responseRepository;
    private readonly IGenericRepository<Question> _questionRepository;
    private readonly IGenericRepository<Option> _optionRepository;
    private readonly IGenericRepository<CorrectAnswer> _correctAnswerRepository;
    private readonly IGenericRepository<Score> _scoreRepository;
    private readonly IGenericRepository<ResponseOption> _responseOptionRepository;
    private readonly ILogger<ResponseService> _logger;

    public ResponseService(
        IGenericRepository<Response> responseRepository,
        IGenericRepository<Question> questionRepository,
        IGenericRepository<Option> optionRepository,
        IGenericRepository<CorrectAnswer> correctAnswerRepository,
        IGenericRepository<Score> scoreRepository,
        IGenericRepository<ResponseOption> responseOptionRepository,
        ILogger<ResponseService> logger)
    {
        _responseRepository = responseRepository;
        _questionRepository = questionRepository;
        _optionRepository = optionRepository;
        _correctAnswerRepository = correctAnswerRepository;
        _scoreRepository = scoreRepository;
        _responseOptionRepository = responseOptionRepository;
        _logger = logger;
    }

    public async Task<ScoreResultDto> SubmitAnswersAsync(SubmitAnswersDto submitDto)
    {
        var scoreResult = new ScoreResultDto();
        var responses = new List<Response>();

        foreach (var answer in submitDto.Answers)
        {
            var question = await _questionRepository.GetQueryable()
                .Include(q => q.Options)
                .Include(q => q.Correctanswers)
                .FirstOrDefaultAsync(q => q.Id == answer.QuestionId);

            if (question == null)
            {
                throw new KeyNotFoundException($"Question with ID {answer.QuestionId} not found");
            }

            var response = new Response
            {
                FormId = submitDto.FormId,
                UserId = submitDto.UserId,
                QuestionId = answer.QuestionId,
                UserAnswer = answer.TextAnswer,
                Points = 0,
                IsCorrect = false
            };

            // Проверка правильности ответа
            switch (question.QuestionType.ToLower())
            {
                case "text":
                    ProcessTextAnswer(answer, question, response);
                    break;

                case "radio":
                case "checkbox":
                    await ProcessOptionsAnswer(answer, question, response);
                    break;
            }

            scoreResult.TotalScore += response.Points.Value;
            scoreResult.QuestionResults.Add(new QuestionResultDto
            {
                QuestionId = answer.QuestionId,
                PointsEarned = response.Points ?? 0,
                IsCorrect = response.IsCorrect ?? false
            });

            responses.Add(response);
        }

        // Сохраняем все ответы
        await _responseRepository.CreateRangeAsync(responses);

        // Сохраняем связи с вариантами ответов
        var responseOptions = new List<ResponseOption>();

        foreach (var response in responses)
        {
            if (response.ResponseOptions != null)
            {
                foreach (var option in response.ResponseOptions)
                {
                    // Проверяем, не добавлен ли уже этот вариант ответа
                    if (!responseOptions.Any(ro => ro.OptionId == option.OptionId && ro.ResponseId == response.Id))
                    {
                        responseOptions.Add(new ResponseOption
                        {
                            ResponseId = response.Id, // Убедитесь, что response.Id задан
                            OptionId = option.OptionId // Указываем только OptionId
                        });
                    }
                }
            }
        }

        if (responseOptions.Any())
        {
            await _responseOptionRepository.CreateRangeAsync(responseOptions);
        }

        // Сохраняем общий счёт
        var score = new Score
        {
            FormId = submitDto.FormId,
            UserId = submitDto.UserId,
            Score1 = scoreResult.TotalScore,
            CreatedAt = DateTime.Now
        };

        await _scoreRepository.CreateAsync(score);

        return scoreResult;
    }

    private void ProcessTextAnswer(UserAnswerDto answer, Question question, Response response)
    {
        if (string.IsNullOrWhiteSpace(answer.TextAnswer))
        {
            response.Points = 0;
            response.IsCorrect = false;
            return;
        }

        var isCorrect = answer.TextAnswer.Trim().Equals(
            question.CorrectAnswer?.Trim(),
            StringComparison.OrdinalIgnoreCase);

        response.IsCorrect = isCorrect;
        response.Points = isCorrect ? question.Points : 0;
    }

    private async Task ProcessOptionsAnswer(UserAnswerDto answer, Question question, Response response)
    {
        if (answer.SelectedOptionIds == null || !answer.SelectedOptionIds.Any())
        {
            response.Points = 0;
            response.IsCorrect = false;
            return;
        }

        // Получаем правильные ответы
        var correctAnswers = await _correctAnswerRepository.GetQueryable()
            .Where(ca => ca.Questionid == question.Id)
            .Select(ca => ca.Optionid)
            .ToListAsync();

        // Получаем выбранные ответы
        var selectedOptions = await _optionRepository.GetQueryable()
            .Where(o => answer.SelectedOptionIds.Contains(o.Id))
            .ToListAsync();

        // Проверяем соответствие
        var isFullyCorrect = false;
        var isPartiallyCorrect = false;

        switch (question.QuestionType.ToLower())
        {
            case "radio":
                // Для radio вопросов полностью правильный ответ
                isFullyCorrect = selectedOptions.Count == 1 && correctAnswers.Contains(selectedOptions[0].Id);
                break;

            case "checkbox":
                // Для checkbox вопросов
                var selectedOptionIds = selectedOptions.Select(o => o.Id).ToList();
                var correctSelectedCount = selectedOptionIds.Count(id => correctAnswers.Contains(id));

                // Полностью правильный ответ
                isFullyCorrect = selectedOptionIds.Count == correctAnswers.Count &&
                                 selectedOptionIds.All(id => correctAnswers.Contains(id));

                // Частично правильный ответ (хотя бы один правильный)
                isPartiallyCorrect = correctSelectedCount > 0;
                break;
        }

        // Начисление баллов
        if (isFullyCorrect)
        {
            response.Points = question.Points; // Полные баллы за полностью правильный ответ
            response.IsCorrect = true;
        }
        else if (isPartiallyCorrect && question.QuestionType.ToLower() == "checkbox")
        {
            response.Points = question.Points / 2; // Половина баллов за частично правильный ответ
            response.IsCorrect = false; // Ответ не считается полностью правильным
        }
        else
        {
            response.Points = 0; // Нет баллов за неправильный ответ
            response.IsCorrect = false;
        }

        // Сохраняем связи с вариантами ответов
        response.ResponseOptions = selectedOptions.Select(o => new ResponseOption
        {
            ResponseId = response.Id, // Убедитесь, что response.Id задан
            OptionId = o.Id           // Указываем только OptionId
        }).ToList();
    }

    public async Task<List<MyAnswersDto>> GetUserAnswers(int userId)
    {
        return await _scoreRepository.GetQueryable()
            .Include(x => x.Form)
            .Include(x => x.User)
            .Where(x=> x.UserId == userId)
            .Select(x => x.FromScoreToAnswer())
            .ToListAsync();
    }

    public async Task<List<FormAnswersDTO>> GetFormAnswers(int formId)
    {
        return await _scoreRepository.GetQueryable()
            .Include(x => x.Form)
            .Include(x => x.User)
            .Where(x=> x.FormId == formId)
            .Select(x => x.FromScoreToFormAnswer())
            .ToListAsync();
    }
}