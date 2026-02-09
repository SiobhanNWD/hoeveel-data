using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hoeveel.Aggregator.Loaders
{
    public static class FileSourceDownloader
    {
        // Downloads any file from the provided URL and saves it to the output path
            //if github, use the raw link
        public static async Task DownloadAsync(string url, string outputPath)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url), "The URL cannot be null or empty.");

            if (string.IsNullOrEmpty(outputPath))
                throw new ArgumentNullException(nameof(outputPath), "The output path cannot be null or empty.");

            using var httpClient = new HttpClient();

            try
            {
                // Perform the GET request to download the file as a stream
                var stream = await httpClient.GetStreamAsync(url);

                // Ensure the directory for the output file exists
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write the stream to the file
                using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream);

                Console.WriteLine($"File downloaded successfully to: {outputPath}");
            }
            catch (HttpRequestException e)
            {
                // Handle request exceptions
                Console.WriteLine($"Error downloading the file: {e.Message}");
            }
            catch (Exception e)
            {
                // Catch other unexpected errors
                Console.WriteLine($"An unexpected error occurred: {e.Message}");
            }
        }
    }
}
