namespace CoolFormApi.DTO.Questions;

public class UpdateQuestionDTO
{
    public int FormId { get; set; }
    public string QuestionText { get; set; } = null!;
    public string QuestionType { get; set; } = null!;
    public string? CorrectAnswer { get; set; }
    
    public int? Points { get; set; }
    public List<string> Options { get; set; } = new();
}