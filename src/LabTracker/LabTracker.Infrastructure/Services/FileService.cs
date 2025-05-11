using LabTracker.Application.Contracts;
using LabTracker.Infrastructure.Abstractions;

namespace LabTracker.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IFileValidatorFactory _fileValidatorFactory;

    public FileService(IFileValidatorFactory fileValidatorFactory)
    {
        _fileValidatorFactory = fileValidatorFactory;
    }

    public async Task<string> SaveFileAsync(Stream stream, string saveDirectory, string fileName)
    {
        var fileValidator = _fileValidatorFactory.GetFileValidator(fileName);

        fileValidator.ValidateFile(stream, fileName);

        Directory.CreateDirectory(saveDirectory);

        var uniqueFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid() +
                             Path.GetExtension(fileName);

        var filePath = Path.Combine(saveDirectory, uniqueFileName);

        await using var destination = File.Create(filePath);
        await stream.CopyToAsync(destination);

        return filePath;
    }

    public void DeleteFile(string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        File.Delete(fileName);
    }
}