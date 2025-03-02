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
    
    [HttpPut("{userId}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser (int userId, [FromForm] UpdateUserDTO updateUserDto)
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

    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUser(int userId)
    {
        var gg = await _userService.getUserById(userId);
        if (gg  == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(gg);
    }
}