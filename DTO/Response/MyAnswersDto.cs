namespace CoolFormApi.DTO.Response;

public class MyAnswersDto
{
    public int Id {get; set;}
    public string Form { get; set; }
    
    public int Score { get; set; }
    
    public int FormScore { get; set; }

    public DateTime? CreatedAt { get; set; }
}