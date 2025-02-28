namespace CoolFormApi.DTO.Response;

public class SubmitAnswersDto
{
    public int FormId { get; set; }
    public int UserId { get; set; }
    public List<UserAnswerDto> Answers { get; set; } = new();
}