namespace CoolFormApi.DTO.Response;

public class FormAnswersDTO
{
    public string User { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public int Score { get; set; }
    
    public int FormScore { get; set; }

}