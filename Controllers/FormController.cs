
using CoolFormApi.DTO.Form;
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
    
    [HttpGet]
    [Authorize]
    [Route ("{userId}")]
    public async Task<IActionResult> GetForm(int userId)
    {
        
        var gg = await _formService.GetUserForms(userId);
        return Ok(gg);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateForm(CreateFormDTO createFormDto)
    {
        return Ok(await _formService.CreateForm(createFormDto));
    }

    [HttpPut]
    [Authorize]
    [Route ("{formId}")]
    public async Task<IActionResult> UpdateForm([FromRoute]int formId, [FromBody] UpdateFormDTO updateFormDto)
    {
        var updated = await _formService.UpdateForm(formId, updateFormDto);
        if (!updated)
        {
            return NotFound();
        }
        return Ok(new { message = "Form updated" });
    }

    [HttpDelete]
    [Authorize]
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