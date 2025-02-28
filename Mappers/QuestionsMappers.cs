using CoolFormApi.DTO.Questions;
using CoolFormApi.Models;

namespace CoolFormApi.Mappers.MapForms;

public static class QuestionsMappers
{
    // public static GetAllQuestionsFromForm GetAllQuestionsFromForm(this Question question)
    // {
    //     return new GetAllQuestionsFromForm()
    //     {
    //         Id = question.Id,
    //         QuestionText = question.QuestionText,
    //         QuestionType = question.QuestionType
    //     };
    // }

    public static Question FromCreateNewQuestionDtoToQuestion(this CreateNewQuestionDTO questionDto)
    {
        return new Question()
        {
            FormId = questionDto.FormId,
            QuestionText = questionDto.QuestionText,
            QuestionType = questionDto.QuestionType,
            Points = questionDto.Points,
            CorrectAnswer = questionDto.QuestionType == "text" ? questionDto.CorrectAnswer : null
        };
    }
}