﻿using CoolFormApi.DTO.Auth;
using CoolFormApi.DTO.User;
using CoolFormApi.Models;

namespace CoolFormApi.Mappers.MapForms;

public static class UserMappers
{
    public static User? fromAuthDtoToUser(this AuthDTO authDto)
    {
        return new User()
        {
            Login = authDto.Login,
            Password = authDto.Password,
        };
    }

    public static GetUserDTO FromUserToGetUser(this User user)
    {
        return new GetUserDTO()
        {
            Login = user.Login,
            Photo = user.Photo
        };
    }
}