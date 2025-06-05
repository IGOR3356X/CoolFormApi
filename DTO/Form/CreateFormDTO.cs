using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace CoolFormApi.DTO.Form;

public class CreateFormDTO
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsPublic { get; set; }
    [EnsureNoDuplicates(ErrorMessage = "Group IDs contain duplicates")]
    public List<int> GroupIds { get; set; } = new(); // Новое поле
}

public class UpdateFormDTO
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsPublic { get; set; }
    [EnsureNoDuplicates(ErrorMessage = "Group IDs contain duplicates")]
    public List<int> GroupIds { get; set; } = new();
}

public class EnsureNoDuplicatesAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        if (value is List<int> list)
        {
            return list.Count == list.Distinct().Count()
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
        return ValidationResult.Success;
    }
}