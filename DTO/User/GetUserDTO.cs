namespace CoolFormApi.DTO.User;

public class GetUserDTO
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string? Photo { get; set; }
    public int RoleId { get; set; }
    public string? Group { get; set; }
    public string FullName { get; set; }
}

public class GetUsersDTO
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string? Photo { get; set; }
    public int RoleId { get; set; }
    public string? Group { get; set; }
    public string FullName { get; set; }
}