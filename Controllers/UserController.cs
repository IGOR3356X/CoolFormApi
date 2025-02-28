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
    private readonly IS3Service _s3Service;
    
    public UserController(IUserService userService, IS3Service s3Service)
    {
        _userService = userService;
        _s3Service = s3Service;
    }
    
    [HttpPut("{userId}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser (int userId, [FromForm] UpdateUserDTO updateUserDto)
    {
        if (updateUserDto.File != null && updateUserDto.File.Length > 0)
        {
            var gg = await _s3Service.UploadFileAsync(updateUserDto.File, updateUserDto.Login.ToLower());
            updateUserDto.Photo = gg;
        }

        var isExist = await _userService.UpdateUser(updateUserDto, userId);
        if (!isExist)
            return BadRequest("User  not found");

        return Ok();
    }
}