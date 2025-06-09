using CoolFormApi.DTO.User;
using CoolFormApi.Interfaces;
using CoolFormApi.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoolFormApi.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class UserController:ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService, IS3Service s3Service)
    {
        _userService = userService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Админ")]
    public async Task<IActionResult> GetAllUsers([FromQuery] string? fullName)
    {
        return Ok(await _userService.GetUsersAsync(fullName));
    }
    
    [Authorize(Roles = "Админ,Учитель,Ученик")]
    [HttpGet("{userId}")]
    [ActionName("GetUserById")]
    public async Task<IActionResult> GetUserById([FromRoute]int userId)
    {
        var gg = await _userService.GetUserById(userId);
        
        if (gg  == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(gg);
    }
    
    [HttpPost]
    [Authorize(Roles = "Админ")]
    public async Task<IActionResult> CreateUser([FromForm]CreateUserDTO createUserDto)
    {
        var response = await _userService.RegisterUser(createUserDto);
        if(response == null)
            return BadRequest(new { message = "Пользователь с таким ником уже существует"});
        return CreatedAtAction(nameof(GetUserById),new {userId = response.Id}, response);
    }
    
    [HttpPut("{userId}")]
    [Authorize(Roles = "Админ,Учитель,Ученик")]
    public async Task<IActionResult> UpdateUser(int userId, [FromForm] UpdateUserDTO updateUserDto)
    {
        var response = await _userService.UpdateUser(updateUserDto, userId);
        switch (response)
        {
            case UserSevicesErrors.NotFound:
                return BadRequest(new { message = "User not found" });
            case UserSevicesErrors.AlreadyExists:
                return BadRequest(new { message = "Username already exist" });
            case UserSevicesErrors.Ok:
                return Ok(new { message = "User successfully updated" });
            default:
                return BadRequest(new { message = "Something went wrong" });
        }
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = "Админ")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var isDeleted = await _userService.DeleteUser(userId);
        if(!isDeleted)
            return BadRequest(new { message = "User not found" });
        return NoContent();
    }
}