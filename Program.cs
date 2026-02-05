using Hoeveel.Aggregator.Mappers;
using Hoeveel.Aggregator.Models.Raw;
using Hoeveel.Aggregator.Loaders;

var sourceConfig = SourceConfigLoader.Load(); //Getting the urls and paths from config

// 1. Download CSV
await CsvSourceDownloader.DownloadAsync(
    sourceConfig.uifwSourceUrl,
    sourceConfig.uifwSourcePath);

// 2. Load CSV → objects
var uifwRows = CsvLoader.Load(
    sourceConfig.uifwSourcePath,
    UifwSourceMapper.Map);

// 3. Verify
Console.WriteLine($"Loaded {uifwRows.Count} UIFW rows");


