﻿using CoolFormApi.DTO.Auth;
using CoolFormApi.Interfaces;
using CoolFormApi.Interfaces.IServices;
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
    
    [HttpPost("api/register")]
    public async Task<IActionResult> Registration([FromBody]AuthDTO authDto)
    {
        try
        {
            var createdUser = await _userService.RegisterUser(authDto);

            return Ok(
                new ResponseAuthDTO()
                {
                    Token = _tokenService.CreateToken(createdUser)
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost("api/login")]
    public async Task<IActionResult> Login(AuthDTO authDto)
    {
        var user = await _userService.LoginUser(authDto.Login.ToLower());

        if (user == null || user.Password != authDto.Password || user.Login != authDto.Login)
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