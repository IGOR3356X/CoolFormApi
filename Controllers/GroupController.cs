using System.ComponentModel.DataAnnotations;
using CoolFormApi.DTO.Group;
using CoolFormApi.DTO.Questions;
using CoolFormApi.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoolFormApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController:ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Админ")]
    public async Task<IActionResult> GetGrops()
    {
        var gg = await _groupService.GetGroups();
        return Ok(gg);
    }
    
    [HttpGet("{groupId:int}")]
    [Authorize(Roles = "Админ")]
    public async Task<IActionResult> GetGropById([FromRoute] int groupId)
    {
        var gg = await _groupService.GetGroupById(groupId);
        return Ok(gg);
    }

    [HttpPost]
    [Authorize(Roles = "Админ")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDTO createGroupDto)
    {
        var gg = await _groupService.CreateGroup(createGroupDto);
        return CreatedAtAction(nameof(GetGropById), new {groupId = gg.Id}, gg);
    }

    [HttpPut]
    [Authorize(Roles = "Админ")]
    [Route("{groupId:int}")]
    public async Task<IActionResult> UpdateQuestion([FromBody] UpdateGroupDTO updateGroupDto,[FromRoute] int groupId)
    {
        var gg = await _groupService.UpdateGroup(updateGroupDto, groupId);
        if (gg == false)
            return BadRequest(new {message = "Группа не найдена"});
        return Ok(new { message = "Группа успешно обловлена" });
    }
    
    [HttpDelete]
    [Authorize(Roles = "Админ")]
    [Route("{groupId:int}")]
    public async Task<IActionResult> DeleteQuestion([FromRoute] int groupId)
    {
        var gg = await _groupService.DeleteGroup(groupId);
        if (gg == false)
            return BadRequest(new {message = "Группа не найдена"});
        return NoContent();
    }
}