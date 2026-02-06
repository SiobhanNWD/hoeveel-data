using Hoeveel.Aggregator.Loaders;
using Hoeveel.Aggregator.Models.Raw;   // TreasuryFactsResponse, UifwFactsRow

var sourceConfig = SourceConfigLoader.Load();
var uifwSource = sourceConfig.Uifw;

// 1. Download UIFW FACTS (this is where the amounts live)
var factsUrl = uifwSource.BuildFactsUrl();           // /cubes/uifwexp/facts?cut=...
var factsOutputPath = uifwSource.FilePath;           // from config/sources.json

await JsonSourceDownloader.DownloadAsync(factsUrl, factsOutputPath);
Console.WriteLine($"UIFW facts downloaded to {factsOutputPath}");

// 2. Load and deserialize UIFW facts JSON
var factsResponse = JsonLoader.Load<TreasuryFactsResponse<UifwFactsRow>>(factsOutputPath);


// 3. Basic sanity checks (mapping verification)
Console.WriteLine($"Facts rows loaded: {factsResponse.Data.Count}");

foreach (var row in factsResponse.Data.Take(5))
{
    Console.WriteLine(
        $"Municipality={row.DemarcationCode}, " +
        $"Year={row.FinancialYear}, " +
        $"Item={row.ItemCode}, " +
        $"Amount={row.Amount}"
    );
}