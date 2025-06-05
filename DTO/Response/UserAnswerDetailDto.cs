namespace CoolFormApi.DTO.Response;

public class AttemptDetailsDto
{
    public int ScoreId { get; set; }
    public int TotalScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<UserAnswerDetailDto> Answers { get; set; }
}

public class UserAnswerDetailDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public string QuestionType { get; set; }
    
    // Для всех типов вопросов
    public List<string> AllOptions { get; set; } // Все варианты ответов (для radio/checkbox)
    
    // Для текстовых вопросов
    public string UserTextAnswer { get; set; }
    public string CorrectTextAnswer { get; set; }
    
    // Для вопросов с выбором
    public List<string> UserSelectedOptions { get; set; }
    public List<string> CorrectOptions { get; set; }
    
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
}