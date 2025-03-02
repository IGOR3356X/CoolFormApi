using System.Diagnostics;
using CoolFormApi.DTO.Auth;
using CoolFormApi.DTO.User;
using CoolFormApi.Interfaces;
using CoolFormApi.Interfaces.IServices;
using CoolFormApi.Mappers.MapForms;
using CoolFormApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoolFormApi.Services;

public class UserService: IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IS3Service _s3Service;
    
    public UserService(IGenericRepository<User> userRepository, IS3Service s3Service)
    {
        _userRepository = userRepository;
        _s3Service = s3Service;
    }
    
    public async Task<User?> RegisterUser(AuthDTO authDTO)
    {
        if (await _userRepository.GetQueryable().Where(x => x.Login.Contains(authDTO.Login)).AnyAsync())
        {
            return null;
        };
        return await _userRepository.CreateAsync(authDTO.fromAuthDtoToUser());
    }

    public async Task<User?> LoginUser(AuthDTO authDTO)
    {
        return await _userRepository.GetQueryable().FirstOrDefaultAsync(x => x.Login.Contains(authDTO.Login) && x.Password.Contains(authDTO.Password));
    }

    public async Task<UserSevicesErrors> UpdateUser(UpdateUserDTO updateUserDto,int userId)
    {
        string? photo = null;
        
        var user = await _userRepository.GetByIdAsync(userId);
        if(user == null)
            return UserSevicesErrors.NotFound;
        
        user.Login = updateUserDto.Login;
        user.Password = updateUserDto.Password;
        
        if (await _userRepository.GetQueryable().Where(x => x.Login.Contains(user.Login) && x.Id != userId).FirstOrDefaultAsync() != null)
        {
            return UserSevicesErrors.AlreadyExists;
        }
        
        if (updateUserDto.File != null && updateUserDto.File.Length > 0)
        {
            photo = await _s3Service.UploadFileAsync(updateUserDto.File, updateUserDto.Login.ToLower(),userId);
        }
        
        user.Photo = photo;
        
        await _userRepository.UpdateAsync(user);
        
        return UserSevicesErrors.Ok;
    }

    public async Task<GetUserDTO> getUserById(int userId)
    {
        var gg =await _userRepository.GetByIdAsync(userId);
        return gg.FromUserToGetUser();
    }
}