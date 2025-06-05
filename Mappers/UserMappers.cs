using CoolFormApi.DTO.Auth;
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
            Id = user.Id,
            Login = user.Login,
            Password = user.Password,
            Photo = user.Photo,
            RoleId = user.RoleId,
        };
    }
    

    public static User fromCreateUserDtoToUser(this CreateUserDTO createUserDto)
    {
        return new User()
        {
            Login = createUserDto.Login,
            Password = createUserDto.Password,
            RoleId = createUserDto.RoleId,
            GroupId = createUserDto.GroupId,
        };
    }
    
    public static CreatedUserDTO fromUserToCreatedUserDto(this User user)
    {
        return new CreatedUserDTO()
        {
            Id = user.Id,
            Login = user.Login,
            Password = user.Password,
            RoleId = user.RoleId,
            GroupId = user.GroupId
        };
    }
}