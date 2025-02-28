using CoolFormApi.DTO.Questions;
using CoolFormApi.Models;

namespace CoolFormApi.Interfaces.IServices;

public interface IQuestionService
{
    public Task<List<GetAllQuestionsFromForm>> getAllQuestions(int formId);

    public Task AddQuestion(CreateNewQuestionDTO newQuestion);

    public Task UpdateQuestion(UpdateQuestionDTO updatedQuestion, int questionId);
    public Task DeleteQuestion(int questionId);
    public Task UpdateFormMaxScore(int formId);
}