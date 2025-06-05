using CoolFormApi.DTO.Auth;
using CoolFormApi.Interfaces;
using CoolFormApi.Interfaces.IServices;
using CoolFormApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoolFormApi.Controllers;

[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;
    
    public AuthorizationController(ITokenService tokenService, IUserService userService)
    {
        _tokenService = tokenService;
        _userService = userService;
    }
    
    [HttpPost("api/login")]
    public async Task<IActionResult> Login(AuthDTO authDto)
    {
        var user = await _userService.LoginUser(authDto);

        if (user == null)
        {
            return Unauthorized("Username not found and/or password");
        }

        return Ok(
            new ResponseAuthDTO()
            {
                Token = _tokenService.CreateToken(user)
            }
        );
    }
}