using LabTracker.Application.Contracts;
using Microsoft.AspNetCore.Http;

namespace LabTracker.Infrastructure.Services;

public class FileService : IFileService
{
    private const int MaxFileSizeBytes = 50 * 1024 * 1024; // 5 MB.
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", "image/webp"];
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];

    public async Task<string> SaveImageAsync(IFormFile file, string saveDirectory, string fileName)
    {
        if (file.Length is <= 0 or > MaxFileSizeBytes)
            throw new InvalidOperationException("Invalid file size.");

        var extension = Path.GetExtension(file.FileName);

        if (!AllowedExtensions.Contains(extension) || !AllowedContentTypes.Contains(file.ContentType))
            throw new InvalidOperationException("Invalid file format.");

        Directory.CreateDirectory(saveDirectory);

        var uniqueFileName = fileName + "_" + Guid.NewGuid() + extension;
        var filePath = Path.Combine(saveDirectory, uniqueFileName);

        await using var stream = File.Create(filePath);
        await file.CopyToAsync(stream);
        
        return filePath;
    }

    public void DeleteImage(string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        File.Delete(fileName);
    }
}