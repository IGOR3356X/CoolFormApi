using Amazon.S3;
using Amazon.S3.Model;
using CoolFormApi.Interfaces.IServices;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly ILogger<S3Service> _logger;

    public S3Service(
        IConfiguration config,
        ILogger<S3Service> logger,
        IAmazonS3 s3Client)
    {
        _logger = logger;
        _s3Client = s3Client;
        _bucketName = config["YandexObjectStorage:BucketName"] 
                      ?? throw new ArgumentException("Missing bucket name");
    }

    public async Task<string> UploadFileAsync(IFormFile file, string fileName)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;  // Сброс позиции потока

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"photos/{fileName}",
                InputStream = memoryStream,
                ContentType = file.ContentType,
                AutoCloseStream = false,
                UseChunkEncoding = false  // Важно для Yandex S3
            };

            var response = await _s3Client.PutObjectAsync(request);
            
            return $"https://{_bucketName}.storage.yandexcloud.net/photos/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "S3 upload failed for {FileName}", fileName);
            throw;
        }
    }
}