
using CoolFormApi.DTO.Form;
using CoolFormApi.DTO.Questions;
using CoolFormApi.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoolFormApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormController: ControllerBase
{
    private readonly IFormService _formService;
    
    public FormController(IFormService formService)
    {
        _formService = formService;
    }
    
    [HttpGet("MyFormsForTeachers")]
    [Authorize(Roles = "Админ,Учитель")]
    public async Task<IActionResult> GetMyFormsForTeachers()
    {
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
        var gg = await _formService.GetTeacherFormsForTeachers(userId);
        return Ok(gg);
    }
    
    [HttpGet("MyFormsForGroups")]
    [Authorize(Roles = "Админ,Учитель")]
    public async Task<IActionResult> GetMyFormsForGroups([FromQuery] List<int>? groupIds)
    {
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
        var forms = await _formService.GetTeacherFormsForStudents(userId, groupIds);
        return Ok(forms);
    }
    
    [HttpGet("StudentFormsForHisGroup")]
    [Authorize(Roles = "Ученик")]
    public async Task<IActionResult> GetUserFormsForGroups()
    {
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
        var gg = await _formService.GetStudentFormsForHisGroup(userId);
        return Ok(gg);
    }

    [HttpPost]
    [Authorize(Roles = "Админ,Учитель")]
    public async Task<IActionResult> CreateForm([FromBody] CreateFormDTO createFormDto)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
        var gg = await _formService.CreateForm(createFormDto,userId);
        return Ok(gg);
    }

    [HttpPut]
    [Authorize(Roles = "Админ,Учитель")]
    [Route ("{formId}")]
    public async Task<IActionResult> UpdateForm([FromRoute]int formId, [FromBody] UpdateFormDTO updateFormDto)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);  
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
        var updated = await _formService.UpdateForm(formId,updateFormDto,userId);
        if (!updated)
        {
            return NotFound();
        }
        return Ok(new { message = "Form updated" });
    }

    [HttpDelete]
    [Authorize(Roles = "Админ,Учитель")]
    [Route("{formId}")]
    public async Task<IActionResult> DeleteForm([FromRoute]int formId)
    {
        var deleted = await _formService.DeleteForm(formId);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}