using LabTracker.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Shared;

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
            return _provider.GetRequiredService<ImageFileValidator>();

        if (FileExtensionsHelper.IsTextDocumentExtension(extension))
            return _provider.GetRequiredService<TextFileValidator>();

        throw new NotSupportedException($"Unsupported file extension: {extension}.");
    }
}