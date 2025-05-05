using Microsoft.AspNetCore.Http;

namespace LabTracker.Application.Contracts;

public interface IFileService
{
    Task<string> SaveImageAsync(IFormFile file, string saveDirectory, string fileName);
}