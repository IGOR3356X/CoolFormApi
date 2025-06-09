using CoolFormApi.DTO.Questions;
using CoolFormApi.Interfaces;
using CoolFormApi.Interfaces.IServices;
using CoolFormApi.Mappers.MapForms;
using CoolFormApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoolFormApi.Services;

public class QuestionService : IQuestionService
{
    private readonly IGenericRepository<Question> _repositoryQestions;
    private readonly IGenericRepository<Option> _repositoryOptions;
    private readonly IGenericRepository<Correctanswer> _repositoryCorAnswer;
    private readonly IGenericRepository<Response> _repositoryResponse;
    private readonly IGenericRepository<Form> _repositoryForm;

    public QuestionService(IGenericRepository<Question> repositoryQestions,
        IGenericRepository<Option> repositoryOptions,
        IGenericRepository<Correctanswer> repositoryCorAnswer,
        IGenericRepository<Response> repositoryResponse,
        IGenericRepository<Form> repositoryForm)
    {
        _repositoryQestions = repositoryQestions;
        _repositoryOptions = repositoryOptions;
        _repositoryCorAnswer = repositoryCorAnswer;
        _repositoryResponse = repositoryResponse;
        _repositoryForm = repositoryForm;
    }

    public async Task<GetFormDescription> getAllQuestions(int formId)
    {
        
        var formWithGroups = await _repositoryForm.GetQueryable()
            .Include(f => f.FormGroups)
            .ThenInclude(fg => fg.Group)
            .Include(f => f.Questions)
            .ThenInclude(q => q.Options)
            .Include(f => f.Questions)
            .ThenInclude(q => q.Correctanswers)
            .ThenInclude(ca => ca.Option)
            .FirstOrDefaultAsync(f => f.Id == formId);

        if (formWithGroups == null)
            return null;
        
        var groupsDto = formWithGroups.FormGroups?
            .Select(fg => new GetGroups
            {
                Id = fg.Group.Id,
                GruopName = fg.Group.Name
            })
            .ToList() ?? new List<GetGroups>();

        // Преобразуем вопросы в DTO
        var questionsDto = formWithGroups.Questions?
            .Select(q => new GetAllQuestionsFromForm
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                QuestionType = q.QuestionType,
                Points = q.Points ?? 0,
                QuestionOptions = q.Options?
                    .Select(o => new QuestionOptions
                    {
                        Id = o.Id,
                        OptionText = o.OptionText
                    })
                    .ToList() ?? new List<QuestionOptions>(),
                CorrectTextAnswer = (q.QuestionType == "text") ? q.CorrectAnswer : null,
                CorrectOptions = (q.QuestionType == "radio" || q.QuestionType == "checkbox")
                    ? q.Correctanswers?
                        .Select(ca => new QuestionOptions
                        {
                            Id = ca.Option.Id,
                            OptionText = ca.Option.OptionText
                        })
                        .ToList()
                    : null
            })
            .ToList() ?? new List<GetAllQuestionsFromForm>();

        return new GetFormDescription
        {
            FormName = formWithGroups.Name,
            FormDescription = formWithGroups.Description,
            IsPublic = formWithGroups.IsPublic,
            Groups = groupsDto,
            Questions = questionsDto
        };
    }

    public async Task<CreatedQuestion> AddQuestion(CreateNewQuestionDTO newQuestion)
    {
        var question = newQuestion.FromCreateNewQuestionDtoToQuestion();
        question.CorrectAnswer = newQuestion.QuestionType == "text"
            ? newQuestion.CorrectAnswer
            : null;

        var createdQuestion = await _repositoryQestions.CreateAsync(question);

        if (newQuestion.QuestionType != "text" && newQuestion.Options?.Count > 0)
        {
            foreach (var optionText in newQuestion.Options)
            {
                var option = new Option
                {
                    QuestionId = createdQuestion.Id,
                    OptionText = optionText
                };
                await _repositoryOptions.CreateAsync(option);
            }

            if (!string.IsNullOrEmpty(newQuestion.CorrectAnswer))
            {
                var options =
                    (_repositoryOptions.GetQueryable().Where(x => x.QuestionId == createdQuestion.Id)).ToList();

                var correctAnswers = newQuestion.QuestionType switch
                {
                    "radio" => new[] { newQuestion.CorrectAnswer.Trim() },
                    "checkbox" => newQuestion.CorrectAnswer
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim()),
                    _ => Enumerable.Empty<string>()
                };

                foreach (var option in options.Where(o => correctAnswers.Contains(o.OptionText)))
                {
                    await _repositoryCorAnswer.CreateAsync(new Correctanswer()
                    {
                        Questionid = createdQuestion.Id,
                        Optionid = option.Id
                    });
                }
            }
        }

        return new CreatedQuestion()
        {
            Id = createdQuestion.Id
        };
    }

    public async Task UpdateQuestion(UpdateQuestionDTO updatedQuestion, int questionId)
    {
        var existingQuestion = await _repositoryQestions.GetByIdAsync(questionId);

        // Обновление основного вопроса
        existingQuestion.Points = updatedQuestion.Points;
        existingQuestion.QuestionText = updatedQuestion.QuestionText;
        existingQuestion.QuestionType = updatedQuestion.QuestionType;
        existingQuestion.CorrectAnswer = updatedQuestion.QuestionType == "text"
            ? updatedQuestion.CorrectAnswer
            : null;

        await _repositoryQestions.UpdateAsync(existingQuestion);

        // Получаем все связанные данные одним запросом
        var oldOptions = await _repositoryOptions.GetQueryable()
            .Where(o => o.QuestionId == existingQuestion.Id)
            .ToListAsync();

        var oldCorrectAnswers = await _repositoryCorAnswer.GetQueryable()
            .Where(c => c.Questionid == existingQuestion.Id)
            .ToListAsync();

        // Массовое удаление
        if (oldCorrectAnswers.Any())
            await _repositoryCorAnswer.DeleteRangeAsync(oldCorrectAnswers);

        if (oldOptions.Any())
            await _repositoryOptions.DeleteRangeAsync(oldOptions);

        // Добавление новых вариантов
        if (updatedQuestion.QuestionType != "text" && updatedQuestion.Options.Count > 0)
        {
            var newOptions = updatedQuestion.Options.Select(optionText => new Option
            {
                QuestionId = existingQuestion.Id,
                OptionText = optionText
            }).ToList();

            await _repositoryOptions.CreateRangeAsync(newOptions);

            // Обработка правильных ответов
            if (!string.IsNullOrEmpty(updatedQuestion.CorrectAnswer))
            {
                var correctAnswers = updatedQuestion.QuestionType switch
                {
                    "radio" => new[] { updatedQuestion.CorrectAnswer.Trim() },
                    "checkbox" => updatedQuestion.CorrectAnswer.Split(','),
                    _ => Array.Empty<string>()
                };

                var options = await _repositoryOptions.GetQueryable()
                    .Where(o => o.QuestionId == existingQuestion.Id)
                    .ToListAsync();

                var correctAnswerEntities = options
                    .Where(o => correctAnswers.Contains(o.OptionText))
                    .Select(o => new Correctanswer
                    {
                        Questionid = existingQuestion.Id,
                        Optionid = o.Id
                    });

                await _repositoryCorAnswer.CreateRangeAsync(correctAnswerEntities);
            }
        }
    }

    public async Task DeleteQuestion(int questionId)
    {
        // Получаем вопрос
        var question = await _repositoryQestions.GetByIdAsync(questionId);
        if (question == null)
        {
            throw new KeyNotFoundException("Question not found");
        }

        // Удаляем сам вопрос
        await _repositoryQestions.DeleteAsync(questionId);
    }

    public async Task UpdateFormMaxScore(int formId)
    {
        var total = await _repositoryQestions.GetQueryable()
            .Where(q => q.FormId == formId)
            .SumAsync(q => q.Points);

        var form = await _repositoryForm.GetByIdAsync(formId);
        form.MaxScore = total ?? 0;
        await _repositoryForm.UpdateAsync(form);
    }
}