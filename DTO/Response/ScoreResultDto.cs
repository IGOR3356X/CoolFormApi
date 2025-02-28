namespace CoolFormApi.DTO.Response;

public class ScoreResultDto
{
    public int TotalScore { get; set; }
    public List<QuestionResultDto> QuestionResults { get; set; } = new();
}