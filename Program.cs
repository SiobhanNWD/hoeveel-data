using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

namespace Hoeveel.Aggregator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Define the URLs for downloading
            string censusMuniUrl = "https://raw.githubusercontent.com/afrith/census-2022-muni-sats/main/person-indicators-muni.csv";
            string censusProvUrl = "https://raw.githubusercontent.com/afrith/census-2022-muni-sats/main/person-indicators-province.csv";

            string outputPathMuni = "data/raw/census-municipality_2022.csv";
            string outputPathProv = "data/raw/census-province_2022.csv";

            // Validate URLs and output paths
            if (string.IsNullOrEmpty(outputPathMuni) || string.IsNullOrEmpty(outputPathProv))
            {
                Console.WriteLine("Invalid file paths provided. Exiting.");
                return;
            }

            // Download the files
            Console.WriteLine("Downloading census municipality CSV...");
            await CsvSourceDownloader.DownloadAsync(censusMuniUrl, outputPathMuni);

            Console.WriteLine("Downloading census province CSV...");
            await CsvSourceDownloader.DownloadAsync(censusProvUrl, outputPathProv);
        }
    }
}



/*
using Hoeveel.Aggregator.Loaders;
using Hoeveel.Aggregator.Models.Raw;   // TreasuryFactsResponse, UifwFactsRow
using Hoeveel.Aggregator.Builders;
using Hoeveel.Aggregator.Models.Stored;

//================== UIFW INGESTION SERVICE ==================
// 1. Download UIFW facts JSON from Treasury API
var sourceConfig = SourceConfigLoader.Load();   // Load all source configuration from config/sources.json
var uifwSource = sourceConfig.Uifw;             // Extract UIFW-specific source configuration

var uifwUrl = uifwSource.BuildUrl();           // Build the UIFW facts URL from config values
var uifwOutputPath = uifwSource.FilePath;      // Output file path defined in config

await JsonSourceDownloader.DownloadAsync(uifwUrl, uifwOutputPath);      // Download UIFW facts JSON to disk
Console.WriteLine($"UIFW facts downloaded to {uifwOutputPath}");        // Log to confirm download to output path

// 2. Load and deserialize UIFW facts JSON into strongly typed rows (UifwRows)
var uifwRows = JsonLoader.Load<TreasuryFactsResponse<UifwRow>>(uifwOutputPath);

// 2.1 Sanity checks (verify mapping and data shape)
Console.WriteLine($"Uifw facts rows loaded: {uifwRows.Data.Count}");

foreach (var row in uifwRows.Data.Take(3))      // Print 5 of the rows for verification purposes
{
    Console.WriteLine(
        $"Municipality={row.DemarcationCode}, " +
        $"Year={row.FinancialYear}, " +
        $"Item={row.ItemCode}, " +
        $"Amount={row.Amount}"
    );
}

// 3. Build Municipality aggregates from UIFW facts
var municipalities = MunicipalityBuilder.BuildFromUifw(uifwRows);

Console.WriteLine($"Municipalities built: {municipalities.Count}");

// 3.1 Sanity check aggregated values
foreach (var m in municipalities.Take(3))
{
    Console.WriteLine(
        $"Municipality={m.Code}, " +
        $"Unauthorised={m.Unauthorised}, " +
        $"Irregular={m.Irregular}, " +
        $"Fruitless={m.Fruitless}, " +
        $"UIFW={m.Uifw}"
    );
}
*/