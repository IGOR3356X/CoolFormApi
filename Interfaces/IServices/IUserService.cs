using CoolFormApi.DTO.Auth;
using CoolFormApi.DTO.User;
using CoolFormApi.Models;

namespace CoolFormApi.Interfaces;

public interface IUserService
{
    public Task<User> RegisterUser(AuthDTO authDTO);
    
    public Task<User> LoginUser(string login);
    
    public Task<bool> UpdateUser(UpdateUserDTO updateUserDTO,int userId);
}