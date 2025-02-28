namespace CoolFormApi.DTO.Form;

public class UpdateFormDTO
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int UserId { get; set; }
}