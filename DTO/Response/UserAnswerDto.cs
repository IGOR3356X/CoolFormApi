namespace CoolFormApi.DTO.Response;

public class UserAnswerDto
{
    public int QuestionId { get; set; }
    public string? TextAnswer { get; set; }
    public List<int>? SelectedOptionIds { get; set; }
}