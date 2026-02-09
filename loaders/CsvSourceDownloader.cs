using System.Net.Http;      //Imports networking functionality so we can make HTTP requests.

public static class CsvSourceDownloader
{
    //An asynchronous method that downloads a CSV file from a URL and saves it to disk
    // Resturns a Task so it can be awaited
    //Example call in Program.cs (Main must be async (static async Task Main(string[] args), so that it download → load → aggregate): 
    //  var uifwUrl = "https://municipaldata.treasury.gov.za/downloads/csv/uifwexp.csv";
    //  var outputPath = "data/raw/uifw-source.csv";
    //  await UifwSourceDownloader.DownloadAsync(uifwUrl, outputPath);

    public static async Task DownloadAsync(string url, string outputPath)
    {
        using var httpClient = new HttpClient();    //creates an HTTP client for making web requests. 'using' ensures it's disposed correctly after use
    
        var csvData = await httpClient.GetStringAsync(url);     //Downloads the CSV file as a string from the given URL. 'await' pauses execution until the download completes

            Hoeveel.Aggregator.Utils.FileHelpers.EnsureDirectoryForFile(outputPath); // Call to ensure the directory exists

        File.WriteAllText(outputPath, csvData); //Writes the downloaded CSV string to disk at the given path
    }

    public static async Task DownloadAsyncStream(string url, string outputPath)
    {
        if (string.IsNullOrEmpty(outputPath))
        {
            throw new ArgumentNullException(nameof(outputPath), "The output path cannot be null or empty.");
        }

        using (var httpClient = new HttpClient())
        {
            // Download the CSV file as a stream
            var stream = await httpClient.GetStreamAsync(url);  // GetStreamAsync is better for large files

            // Create the output directory if it doesn't exist
                Hoeveel.Aggregator.Utils.FileHelpers.EnsureDirectoryForFile(outputPath); // Call to ensure the directory exists

            // Write the stream to the file
            using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);  // Copy the stream to the file
            }

            Console.WriteLine($"File downloaded to: {outputPath}");
        }
    }

}
