using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CoolFormApi.DTO.User;

public class UpdateUserDTO
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public IFormFile? File { get; set; }
    public int? RoleId { get; set; }
    public int? GroupId { get; set; } 
}