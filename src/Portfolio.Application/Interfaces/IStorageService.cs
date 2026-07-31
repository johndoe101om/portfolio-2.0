using Portfolio.Application.DTOs;

namespace Portfolio.Application.Interfaces;

public interface IStorageService
{
    Task<ImageUploadResultDto> UploadImageAsync(Stream fileStream, string fileName, string contentType, string folderPath, CancellationToken ct = default);
    Task<bool> DeleteImageAsync(string storagePath, CancellationToken ct = default);
}
