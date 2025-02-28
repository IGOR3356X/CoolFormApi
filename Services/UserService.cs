using System.Diagnostics;
using CoolFormApi.DTO.Auth;
using CoolFormApi.DTO.User;
using CoolFormApi.Interfaces;
using CoolFormApi.Mappers.MapForms;
using CoolFormApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoolFormApi.Services;

public class UserService: IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    
    public UserService(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User> RegisterUser(AuthDTO authDTO)
    {
        return await _userRepository.CreateAsync(authDTO.fromAuthDtoToUser());
    }

    public async Task<User> LoginUser(string login)
    {
        return await _userRepository.GetQueryable().FirstOrDefaultAsync(x => x.Login == login);
    }

    public async Task<bool> UpdateUser(UpdateUserDTO updateUserDTO,int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if(user == null)
            return false;
        
        user.Id = userId;
        user.Login = updateUserDTO.Login;
        user.Password = updateUserDTO.Password;
        user.Photo = updateUserDTO.Photo;
        await _userRepository.UpdateAsync(user);
        
        return true;
    }
}