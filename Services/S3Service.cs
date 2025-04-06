using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CoolFormApi.Interfaces.IServices;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = "coolformuserstorage";
    private readonly ILogger<S3Service> _logger;

    public S3Service(IConfiguration configuration, ILogger<S3Service> logger)
    {
        _logger = logger;
    
        var accessKey = configuration["YandexObjectStorage:AccessKey"];
        var secretKey = configuration["YandexObjectStorage:SecretKey"];
    
        _logger.LogInformation("Using AccessKey: {AccessKey}", accessKey?[..3] + "***"); // Частичный лог для безопасности
        _logger.LogInformation("Using SecretKey: {SecretKey}", secretKey?[..3] + "***");
    
        var config = new AmazonS3Config
        {
            ServiceURL = "https://storage.yandexcloud.net",
            AuthenticationRegion = "ru-central1",
            ForcePathStyle = true
        };
    
        _s3Client = new AmazonS3Client(accessKey, secretKey, config);
    }

    public async Task<string> UploadFileAsync(IFormFile file, int UserId)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;  // Сброс позиции потока

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"UserPhotos/{UserId}/{file.FileName}",
                InputStream = memoryStream,
                ContentType = file.ContentType,
                AutoCloseStream = false,
                UseChunkEncoding = false  // Важно для Yandex S3
            };

            var response = await _s3Client.PutObjectAsync(request);
            
            return $"https://{_bucketName}.storage.yandexcloud.net/UserPhotos/{UserId}/{file.FileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "S3 upload failed for {FileName}", file.Name);
            throw;
        }
    }
}