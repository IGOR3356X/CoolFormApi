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
        var isExist = await _userService.UpdateUser(updateUserDto, userId);
        if (!isExist)
            return BadRequest("User not found or Username already exist");

        return Ok();
    }
}