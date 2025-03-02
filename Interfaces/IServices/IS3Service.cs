namespace CoolFormApi.Interfaces.IServices;

public interface IS3Service
{
    public Task<string> UploadFileAsync(IFormFile file, string fileName, int UserId);
}