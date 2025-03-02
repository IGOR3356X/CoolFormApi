using CoolFormApi.DTO.Auth;
using CoolFormApi.DTO.User;
using CoolFormApi.Models;

namespace CoolFormApi.Interfaces;

public interface IUserService
{
    public Task<User?> RegisterUser(AuthDTO authDTO);
    
    public Task<User?> LoginUser(AuthDTO authDTO);
    
    public Task<UserSevicesErrors> UpdateUser(UpdateUserDTO updateUserDTO,int userId);
}