namespace CoolFormApi.DTO.User;

public class CreateUserDTO
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public int GroupId { get; set; }
    public int RoleId { get; set; }
    public string FullName { get; set; }
}

public class CreatedUserDTO
{
    public int? Id { get; set; }
    public string? Login { get; set; }
    public int GroupId { get; set; }
    public string? Password { get; set; }
    public int RoleId { get; set; }
    public string FullName { get; set; }
}