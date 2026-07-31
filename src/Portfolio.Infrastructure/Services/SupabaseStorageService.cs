using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;
using System.Net.Http.Headers;

namespace Portfolio.Infrastructure.Services;

public class SupabaseStorageService : IStorageService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IHostEnvironment _env;
    private readonly ILogger<SupabaseStorageService> _logger;
    private readonly string _supabaseUrl;
    private readonly string _supabaseKey;
    private readonly string _bucketName = "project-images";

    public SupabaseStorageService(
        HttpClient httpClient,
        IConfiguration config,
        IHostEnvironment env,
        ILogger<SupabaseStorageService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _env = env;
        _logger = logger;
        _supabaseUrl = config["SUPABASE_URL"] ?? config["Supabase:Url"] ?? string.Empty;
        _supabaseKey = config["SUPABASE_SERVICE_KEY"] ?? config["Supabase:ServiceKey"] ?? config["SUPABASE_KEY"] ?? string.Empty;
    }

    public async Task<ImageUploadResultDto> UploadImageAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string folderPath,
        CancellationToken ct = default)
    {
        try
        {
            var sanitizedFileName = $"{Guid.NewGuid():N}_{Path.GetFileNameWithoutExtension(fileName)}{Path.GetExtension(fileName)}";
            var relativePath = string.IsNullOrWhiteSpace(folderPath) ? sanitizedFileName : $"{folderPath.Trim('/')}/{sanitizedFileName}";

            // Check if Supabase configured
            if (!string.IsNullOrWhiteSpace(_supabaseUrl) && !string.IsNullOrWhiteSpace(_supabaseKey) && !_supabaseUrl.Contains("YOUR_SUPABASE"))
            {
                var endpoint = $"{_supabaseUrl.TrimEnd('/')}/storage/v1/object/{_bucketName}/{relativePath}";
                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
                request.Headers.Add("apikey", _supabaseKey);

                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream, ct);
                var content = new ByteArrayContent(memoryStream.ToArray());
                content.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType) ? "image/webp" : contentType);
                request.Content = content;

                var response = await _httpClient.SendAsync(request, ct);
                if (response.IsSuccessStatusCode)
                {
                    var publicUrl = $"{_supabaseUrl.TrimEnd('/')}/storage/v1/object/public/{_bucketName}/{relativePath}";
                    _logger.LogInformation("Image uploaded to Supabase Storage: {Path}", relativePath);
                    return new ImageUploadResultDto(true, relativePath, publicUrl, "Image uploaded successfully to Supabase Storage.");
                }

                _logger.LogWarning("Supabase upload failed with status {StatusCode}, falling back to local storage", response.StatusCode);
            }

            // Local fallback upload to root uploads folder
            var rootPath = _env.ContentRootPath ?? Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(rootPath, "wwwroot", "uploads", folderPath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(targetFolder);

            var localFilePath = Path.Combine(targetFolder, sanitizedFileName);
            using (var destinationStream = File.Create(localFilePath))
            {
                fileStream.Position = 0;
                await fileStream.CopyToAsync(destinationStream, ct);
            }

            var localPublicUrl = $"/uploads/{folderPath.Trim('/')}/{sanitizedFileName}";
            _logger.LogInformation("Image saved locally: {Path}", localPublicUrl);
            return new ImageUploadResultDto(true, relativePath, localPublicUrl, "Image saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to storage");
            return new ImageUploadResultDto(false, string.Empty, string.Empty, $"Upload error: {ex.Message}");
        }
    }

    public async Task<bool> DeleteImageAsync(string storagePath, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(storagePath)) return true;

            if (!string.IsNullOrWhiteSpace(_supabaseUrl) && !string.IsNullOrWhiteSpace(_supabaseKey) && !_supabaseUrl.Contains("YOUR_SUPABASE"))
            {
                var endpoint = $"{_supabaseUrl.TrimEnd('/')}/storage/v1/object/{_bucketName}/{storagePath}";
                using var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
                request.Headers.Add("apikey", _supabaseKey);

                var response = await _httpClient.SendAsync(request, ct);
                return response.IsSuccessStatusCode;
            }

            var rootPath = _env.ContentRootPath ?? Directory.GetCurrentDirectory();
            var localFilePath = Path.Combine(rootPath, "wwwroot", storagePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(localFilePath))
            {
                File.Delete(localFilePath);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {Path}", storagePath);
            return false;
        }
    }
}
