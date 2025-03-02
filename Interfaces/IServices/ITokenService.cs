using CoolFormApi.Models;

namespace CoolFormApi.Interfaces.IServices;

public interface ITokenService
{
    string CreateToken(User? user);
}