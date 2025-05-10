using LabTracker.Application.Contracts;
using LabTracker.Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LabTracker.Infrastructure.Services;

public class FileValidatorFactory : IFileValidatorFactory
{
    private readonly IServiceProvider _provider;

    public FileValidatorFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IFileValidator GetFileValidator(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (FileExtensionsHelper.IsImageExtension(extension))
        {
            return _provider.GetRequiredService<ImageFileValidator>();
        }

        throw new NotSupportedException($"Unsupported file extension: {extension}.");
    }
}