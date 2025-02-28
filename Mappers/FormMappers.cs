using CoolFormApi.DTO.Form;
using CoolFormApi.Models;

namespace CoolFormApi.Mappers.MapForms;

public static class FormMappers
{
    public static FormDTO fromFormToFormDTO(this Form form)
    {
        return new FormDTO()
        {
            Id = form.Id,
            Name = form.Name,
            Description = form.Description,
            User = form.User.Login,
        };
    }

    public static Form fromCreateFormDTOToForm(this CreateFormDTO formDTO)
    {
        return new Form()
        {
            Name = formDTO.Name,
            Description = formDTO.Description,
            UserId = formDTO.UserId,
        };
    }

    public static CreatedFromDTO fromFormToCreatedFormDTO(this Form form)
    {
        return new CreatedFromDTO()
        {
            Id = form.Id,
            Name = form.Name,
            Description = form.Description,
            UserId = form.UserId,
        };
    }
}