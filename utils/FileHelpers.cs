using System;
using System.IO;

namespace Hoeveel.Aggregator.Utils;

public static class FileHelpers
{
    // Ensure the directory for the provided file path exists.
    // Safe no-op if the path is null/empty or points to the current directory.
    public static void EnsureDirectoryForFile(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
