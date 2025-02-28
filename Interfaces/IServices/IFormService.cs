
using CoolFormApi.DTO.Form;

namespace CoolFormApi.Interfaces.IServices;

public interface IFormService
{
    public Task<List<FormDTO>> GetUserForms(int userId);
    public Task<CreatedFromDTO> CreateForm(CreateFormDTO createFormDto);
    public Task<bool> UpdateForm(int formId, UpdateFormDTO updateFormDto);
    public Task<bool> DeleteForm(int userId);
}