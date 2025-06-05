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
    private readonly IGenericRepository<Correctanswer> _correctAnswerRepository;
    private readonly IGenericRepository<Score> _scoreRepository;
    private readonly IGenericRepository<ResponseOption> _responseOptionRepository;
    private readonly ILogger<ResponseService> _logger;

    public ResponseService(
        IGenericRepository<Response> responseRepository,
        IGenericRepository<Question> questionRepository,
        IGenericRepository<Option> optionRepository,
        IGenericRepository<Correctanswer> correctAnswerRepository,
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
        var scoreResult = new ScoreResultDto { QuestionResults = new List<QuestionResultDto>() };
        var responses = new List<Response>();
        var responseOptions = new List<ResponseOption>(); // Для немедленного сохранения

        // Создаем Score
        var score = new Score
        {
            FormId = submitDto.FormId,
            UserId = submitDto.UserId,
            Score1 = 0,
            CreatedAt = DateTime.Now
        };
        await _scoreRepository.CreateAsync(score);

        // Обрабатываем каждый ответ
        foreach (var answer in submitDto.Answers)
        {
            var question = await _questionRepository.GetQueryable()
                .Include(q => q.Correctanswers)
                .FirstOrDefaultAsync(q => q.Id == answer.QuestionId);

            if (question == null) continue;

            var response = new Response
            {
                FormId = submitDto.FormId,
                UserId = submitDto.UserId,
                QuestionId = answer.QuestionId,
                UserAnswer = answer.TextAnswer ?? string.Empty,
                Points = 0,
                IsCorrect = false,
                ScoreId = score.Id
            };

            switch (question.QuestionType.ToLowerInvariant())
            {
                case "text":
                    ProcessTextAnswer(answer, question, response);
                    break;

                case "radio":
                case "checkbox":
                    await ProcessOptionsAnswer(answer, question, response, responseOptions);
                    break;
            }

            responses.Add(response);
            scoreResult.TotalScore += response.Points ?? 0;
            scoreResult.QuestionResults.Add(new QuestionResultDto
            {
                QuestionId = answer.QuestionId,
                PointsEarned = response.Points ?? 0,
                IsCorrect = response.IsCorrect ?? false
            });
        }

        // Сохраняем ответы
        await _responseRepository.CreateRangeAsync(responses);

        // Сохраняем выбранные варианты
        if (responseOptions.Any())
        {
            await _responseOptionRepository.CreateRangeAsync(responseOptions);
        }

        // Обновляем общий счет
        score.Score1 = scoreResult.TotalScore;
        await _scoreRepository.UpdateAsync(score);

        return scoreResult;
    }

    private async Task ProcessOptionsAnswer(
        UserAnswerDto answer, 
        Question question, 
        Response response,
        List<ResponseOption> responseOptions)
    {
        if (answer.SelectedOptionIds == null || !answer.SelectedOptionIds.Any())
        {
            response.Points = 0;
            response.IsCorrect = false;
            return;
        }

        // Проверка существования вариантов
        var existingOptionIds = await _optionRepository.GetQueryable()
            .Where(o => answer.SelectedOptionIds.Contains(o.Id))
            .Select(o => o.Id)
            .ToListAsync();

        var invalidIds = answer.SelectedOptionIds.Except(existingOptionIds).ToList();
        if (invalidIds.Any())
        {
            throw new ArgumentException($"Invalid Option IDs: {string.Join(", ", invalidIds)}");
        }

        // Получаем правильные ответы
        var correctAnswers = await _correctAnswerRepository.GetQueryable()
            .Where(ca => ca.Questionid == question.Id)
            .Select(ca => ca.Optionid)
            .ToListAsync();

        // Проверяем правильность
        bool isFullyCorrect = false;
        var selectedIds = answer.SelectedOptionIds;

        if (correctAnswers.Any())
        {
            isFullyCorrect = question.QuestionType.ToLowerInvariant() switch
            {
                "radio" => selectedIds.Count == 1 && correctAnswers.Contains(selectedIds[0]),
                "checkbox" => selectedIds.Count == correctAnswers.Count && 
                              selectedIds.All(correctAnswers.Contains),
                _ => false
            };
        }

        response.IsCorrect = isFullyCorrect;
        response.Points = isFullyCorrect ? question.Points : 0;

        // Создаем записи для ResponseOptions
        foreach (var optionId in selectedIds)
        {
            responseOptions.Add(new ResponseOption
            {
                Response = response, // Связь через навигационное свойство
                OptionId = optionId
            });
        }
    }

    private void ProcessTextAnswer(UserAnswerDto answer, Question question, Response response)
    {
        var isCorrect = string.Equals(
            answer.TextAnswer?.Trim() ?? "",
            question.CorrectAnswer?.Trim() ?? "",
            StringComparison.OrdinalIgnoreCase
        );

        response.IsCorrect = isCorrect;
        response.Points = isCorrect ? question.Points : 0;
    }

    public async Task<List<MyAnswersDto>> GetUserAnswers(int userId)
    {
        return await _scoreRepository.GetQueryable()
            .Include(x => x.Form)
            .Include(x => x.User)
            .Where(x => x.UserId == userId)
            .Select(x => x.FromScoreToAnswer())
            .ToListAsync();
    }

    public async Task<List<FormAnswersDTO>> GetFormAnswers(int formId)
    {
        return await _scoreRepository.GetQueryable()
            .Include(x => x.Form)
            .Include(x => x.User)
            .Where(x => x.FormId == formId)
            .Select(x => x.FromScoreToFormAnswer())
            .ToListAsync();
    }

    public async Task<AttemptDetailsDto> GetAttemptDetailsAsync(int scoreId, int currentUserId)
    {
        var score = await _scoreRepository.GetQueryable()
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == scoreId);

        var responses = await _responseRepository.GetQueryable()
            .Include(r => r.Question)
            .ThenInclude(q => q.Options) // Добавлено: загружаем ВСЕ варианты вопроса
            .Include(r => r.Question)
            .ThenInclude(q => q.Correctanswers)
            .ThenInclude(ca => ca.Option)
            .Include(r => r.ResponseOptions)
            .ThenInclude(ro => ro.Option)
            .Where(r => r.ScoreId == scoreId)
            .ToListAsync();

        var answerDetails = new List<UserAnswerDetailDto>();

        foreach (var response in responses)
        {
            var question = response.Question;
            var detail = new UserAnswerDetailDto
            {
                QuestionId = response.QuestionId,
                QuestionText = question.QuestionText,
                QuestionType = question.QuestionType,
                IsCorrect = response.IsCorrect ?? false,
                PointsEarned = response.Points ?? 0,
                AllOptions = question.Options?.Select(o => o.OptionText).ToList() ?? new List<string>() // Все варианты
            };

            if (question.QuestionType.ToLower() == "text")
            {
                detail.UserTextAnswer = response.UserAnswer;
                detail.CorrectTextAnswer = question.CorrectAnswer; // Правильный текст
            }
            else
            {
                detail.UserSelectedOptions = response.ResponseOptions
                    .Select(ro => ro.Option.OptionText)
                    .ToList();

                detail.CorrectOptions = question.Correctanswers
                    .Select(ca => ca.Option.OptionText)
                    .ToList();
            }

            answerDetails.Add(detail);
        }

        return new AttemptDetailsDto
        {
            ScoreId = score.Id,
            TotalScore = score.Score1,
            CreatedAt = score.CreatedAt,
            Answers = answerDetails
        };
    }
}