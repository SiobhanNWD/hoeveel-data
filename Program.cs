using Hoeveel.Aggregator.Loaders;
using Hoeveel.Aggregator.Models.Raw;   // TreasuryFactsResponse, UifwRow, CensusMuniRow
using Hoeveel.Aggregator.Models.Config;
using Hoeveel.Aggregator.Mappers;     // CensusMuniMapper

// ================== CENSUS AND UIFW DOWNLOAD TEST ==================

// Hardcoded source configuration (URLs)
var censusProvUrl = "https://raw.githubusercontent.com/afrith/census-2022-muni-stats/refs/heads/main/person-indicators-province.csv";
var censusMuniUrl = "https://raw.githubusercontent.com/afrith/census-2022-muni-stats/refs/heads/main/person-indicators-muni.csv";

// Set file paths where the files will be saved locally
var censusProvFilePath = "data/raw/census-province_2022.csv";
var censusMuniFilePath = "data/raw/census-muni_2022.csv";

// UIFW configuration (from sources.json)
//var uifwUrl = "https://municipaldata.treasury.gov.za/api/cubes/uifwexp/facts?cut=financial_year_end.year:2022&format=json";
//var uifwFilePath = "data/raw/uifw-facts_2022.json";

// ------------------
// PROVINCE CENSUS DOWNLOAD
// ------------------
Console.WriteLine("Downloading census province CSV...");
await FileSourceDownloader.DownloadAsync(
    censusProvUrl,           // Hardcoded URL for province CSV
    censusProvFilePath       // File path to save the CSV
);

Console.WriteLine($"Census province CSV downloaded to {censusProvFilePath}");

// ------------------
// MUNICIPALITY CENSUS DOWNLOAD
// ------------------
Console.WriteLine("Downloading census municipality CSV...");
await FileSourceDownloader.DownloadAsync(
    censusMuniUrl,           // Hardcoded URL for municipality CSV
    censusMuniFilePath       // File path to save the CSV
);

Console.WriteLine($"Census municipality CSV downloaded to {censusMuniFilePath}");

// ------------------
// UIFW DOWNLOAD
// ------------------
/* Console.WriteLine("Downloading UIFW facts JSON...");
await JsonSourceDownloader.DownloadAsync(
    uifwUrl,                 // Hardcoded URL for UIFW JSON
    uifwFilePath             // Path to save the file
);

Console.WriteLine($"UIFW facts JSON downloaded to {uifwFilePath}");
*/

// 1. Download UIFW facts JSON from Treasury API
var sourceConfig = SourceConfigLoader.Load();   // Load all source configuration from config/sources.json
var uifwSource = sourceConfig.Uifw;             // Extract UIFW-specific source configuration

var uifwUrl = uifwSource.BuildUrl();           // Build the UIFW facts URL from config values
var uifwOutputPath = uifwSource.FilePath;      // Output file path defined in config

await JsonSourceDownloader.DownloadAsync(uifwUrl, uifwOutputPath);      // Download UIFW facts JSON to disk
Console.WriteLine($"UIFW facts downloaded to {uifwOutputPath}");        // Log to confirm download to output path

// ------------------
// SUCCESSFUL TEST LOGGING
// ------------------
Console.WriteLine("Census and UIFW file downloads test complete.");

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