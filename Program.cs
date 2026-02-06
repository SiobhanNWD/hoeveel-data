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

// 2. Load and deserialize UIFW facts JSON into stringly typed rows (UifwRows)
var uifwRows = JsonLoader.Load<TreasuryFactsResponse<UifwRow>>(uifwOutputPath);

// 3. Sanity checks (verify mapping and data shape)
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

// 4. Build Municipality aggregates from UIFW facts
var municipalities = MunicipalityBuilder.BuildFromUifw(uifwRows);

Console.WriteLine($"Municipalities built: {municipalities.Count}");

// 5. Sanity check aggregated values
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
