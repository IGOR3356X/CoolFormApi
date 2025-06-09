using CoolFormApi.DTO.Auth;
using CoolFormApi.DTO.User;
using CoolFormApi.Models;

namespace CoolFormApi.Interfaces;

public interface IUserService
{
    public Task<List<GetUsersDTO>> GetUsersAsync(string FullName);
    public Task<CreatedUserDTO?> RegisterUser(CreateUserDTO createUserDto);
    
    public Task<User?> LoginUser(AuthDTO authDTO);
    
    public Task<UserSevicesErrors> UpdateUser(UpdateUserDTO updateUserDTO,int userId);

    public Task<GetUserDTO> GetUserById(int userId);

    public Task<bool> DeleteUser(int userId);
}