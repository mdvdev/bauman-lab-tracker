using LabTracker.Shared.Contracts;
using MimeKit;
using Shared;

namespace LabTracker.Infrastructure.Services;

public class TextFileValidator : IFileValidator
{
    private const int MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB.

    public void ValidateFile(Stream stream, string fileName)
    {
        if (stream.Length is <= 0 or > MaxFileSizeBytes)
            throw new InvalidOperationException(
                $"Invalid file size: size must be between 1 and {MaxFileSizeBytes} bytes.");

        var mimeType = MimeTypes.GetMimeType(fileName);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!FileExtensionsHelper.IsTextDocumentExtension(extension))
        {
            throw new NotSupportedException("Unsupported file extension.");
        }

        if (!FileExtensionsHelper.IsSupportedTextDocumentMimeType(mimeType))
        {
            throw new NotSupportedException("Unsupported MIME type.");
        }
    }
}