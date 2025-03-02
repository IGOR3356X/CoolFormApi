using CoolFormApi.DTO.Questions;
using CoolFormApi.Models;

namespace CoolFormApi.Interfaces.IServices;

public interface IQuestionService
{
    public Task<GetFormDescription> getAllQuestions(int formId);

    public Task<CreatedQuestion> AddQuestion(CreateNewQuestionDTO newQuestion);

    public Task UpdateQuestion(UpdateQuestionDTO updatedQuestion, int questionId);
    public Task DeleteQuestion(int questionId);
    public Task UpdateFormMaxScore(int formId);
}