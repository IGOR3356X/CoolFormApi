namespace CoolFormApi.DTO.Response;

public class FormAnswersDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string User { get; set; }
    
    public string Photo { get; set; }
    public DateTime? CreatedAt { get; set; }
    
    public int Score { get; set; }
    
    public int FormScore { get; set; }

}