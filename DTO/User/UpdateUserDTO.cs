namespace CoolFormApi.DTO.User;

public class UpdateUserDTO
{
    public string? Login { get; set; }

    public string? Password { get; set; }
    
    public IFormFile? File { get; set; }
}