
using CoolFormApi.DTO.Form;

namespace CoolFormApi.Interfaces.IServices;

public interface IFormService
{
    Task<List<FormDTO>> GetTeacherFormsForTeachers(int userId);
    Task<List<FormDTO>> GetTeacherFormsForStudents(int userId, List<int>? groupIds = null);
    Task<List<FormDTO>> GetStudentFormsForHisGroup(int userId);
    Task<CreatedFromDTO> CreateForm(CreateFormDTO createFormDto,int userId);
    Task<bool> UpdateForm(int formId, UpdateFormDTO updateFormDto,int userId);
    Task<bool> DeleteForm(int userId);
}