using System.Net.Http;

namespace Hoeveel.Aggregator.Loaders;

public static class JsonSourceDownloader
{
    // Downloads JSON from ANY URL and saves it to disk at the given filePath
    public static async Task DownloadAsync(string url, string filePath)
    {
        using var httpClient = new HttpClient();                // Create HttpClient so that we can make HTTP requests

        var jsonData = await httpClient.GetStringAsync(url);    // Download JSON as a string

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);  // Ensure target folder exists

        File.WriteAllText(filePath, jsonData);                  // Write JSON to disk
    }
}