namespace LabTracker.Infrastructure.Helpers;

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

    public static bool IsImageExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return SupportedImageExtensions.Contains(extension);
    }

    public static bool IsSupportedImageMimeType(string mimeType)
    {
        return SupportedImageMimeTypes.Contains(mimeType);
    }
}