namespace CoolFormApi.DTO.Response;

public class QuestionResultDto
{
    public int QuestionId { get; set; }
    public int PointsEarned { get; set; }
    public bool IsCorrect { get; set; }
}