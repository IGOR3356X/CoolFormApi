using System.Diagnostics;
using CoolFormApi.DTO.Auth;
using CoolFormApi.DTO.User;
using CoolFormApi.Interfaces;
using CoolFormApi.Interfaces.IServices;
using CoolFormApi.Mappers.MapForms;
using CoolFormApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoolFormApi.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IS3Service _s3Service;

    public UserService(IGenericRepository<User> userRepository, IS3Service s3Service)
    {
        _userRepository = userRepository;
        _s3Service = s3Service;
    }


    public async Task<List<GetUsersDTO>> GetUsersAsync(string FullName)
    {
        List<User> users = new List<User>();
        
        if (!String.IsNullOrWhiteSpace(FullName))
        {
            users = await _userRepository.GetQueryable()
                .Include(x => x.Group)
                .Where(x => x.Fullname.ToLower().Contains(FullName.ToLower()))
                .ToListAsync();
        }
        else
        {
            users = await _userRepository.GetQueryable()
                .Include(x => x.Group)
                .OrderBy(u => u.Id).ToListAsync();
        }

        return users.Select(u => new GetUsersDTO
        {
            Id = u.Id,
            Login = u.Login,
            Photo = u.Photo,
            RoleId = u.RoleId,
            Group = u.Group.Name,
            FullName = u.Fullname
        }).ToList();
    }

    public async Task<CreatedUserDTO?> RegisterUser(CreateUserDTO createUserDto)
    {
        if (await _userRepository.GetQueryable().Where(x => x.Login.Contains(createUserDto.Login)).AnyAsync())
        {
            return null;
        }

        ;
        var created = await _userRepository.CreateAsync(createUserDto.fromCreateUserDtoToUser());
        return created.fromUserToCreatedUserDto();
    }

    public async Task<User?> LoginUser(AuthDTO authDTO)
    {
        return await _userRepository.GetQueryable()
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Login == authDTO.Login && x.Password == authDTO.Password);
    }

    public async Task<UserSevicesErrors> UpdateUser(UpdateUserDTO updateUserDto, int userId)
    {
        string? photo = null;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return UserSevicesErrors.NotFound;

        user.Login = updateUserDto.Login ?? user.Login;
        user.Password = updateUserDto.Password ?? user.Password;
        user.RoleId = updateUserDto.RoleId ?? user.RoleId;
        user.GroupId = updateUserDto.GroupId ?? user.GroupId;
        user.Fullname = updateUserDto.FullName ?? user.Fullname;

        if (await _userRepository.GetQueryable().Where(x => x.Login == user.Login && x.Id != userId)
                .FirstOrDefaultAsync() != null)
        {
            return UserSevicesErrors.AlreadyExists;
        }

        if (updateUserDto.File != null && updateUserDto.File.Length > 0)
        {
            photo = await _s3Service.UploadFileAsync(updateUserDto.File, userId);
        }

        user.Photo = photo ?? user.Photo;

        await _userRepository.UpdateAsync(user);

        return UserSevicesErrors.Ok;
    }

    public async Task<GetUserDTO> GetUserById(int userId)
    {
        var gg = await _userRepository.GetQueryable()
            .Include(x => x.Group)
            .Where(x => x.Id == userId)
            .FirstOrDefaultAsync();
        return gg.FromUserToGetUser();
    }

    public async Task<bool> DeleteUser(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;
        await _userRepository.DeleteAsync(user);
        return true;
    }
}