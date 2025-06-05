using CoolFormApi.DTO.Response;
using CoolFormApi.Models;

namespace CoolFormApi.Mappers.MapForms;

public static class ResponseMappers
{
    public static MyAnswersDto FromScoreToAnswer(this Score score)
    {
        return new MyAnswersDto()
        {
            Id = score.Id,
            Form = score.Form.Name,
            CreatedAt = score.CreatedAt,
            Score = score.Score1,
            FormScore = score.Form.MaxScore
        };
    }
    
    public static FormAnswersDTO FromScoreToFormAnswer(this Score score)
    {
        return new FormAnswersDTO()
        {
            Id = score.Id,
            UserId = score.UserId,
            User = score.User.Login ?? "Ноу нейм",
            Photo = score.User.Photo,
            CreatedAt = score.CreatedAt,
            Score = score.Score1,
            FormScore = score.Form.MaxScore
        };
    }
}