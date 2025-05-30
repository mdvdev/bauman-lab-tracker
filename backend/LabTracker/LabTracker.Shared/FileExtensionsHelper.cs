namespace Shared;

public static class FileExtensionsHelper
{
    private static readonly HashSet<string> SupportedImageExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

    private static readonly HashSet<string> SupportedImageMimeTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/gif", "image/webp"
        };

    private static readonly HashSet<string> SupportedTextDocumentExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".pdf", ".docx"
        };

    private static readonly HashSet<string> SupportedTextDocumentMimeTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "text/plain",
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

    public static bool IsImageExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return SupportedImageExtensions.Contains(extension);
    }

    public static bool IsSupportedImageMimeType(string mimeType)
    {
        return SupportedImageMimeTypes.Contains(mimeType);
    }

    public static bool IsTextDocumentExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return SupportedTextDocumentExtensions.Contains(extension);
    }

    public static bool IsSupportedTextDocumentMimeType(string mimeType)
    {
        return SupportedTextDocumentMimeTypes.Contains(mimeType);
    }
}