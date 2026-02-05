using Hoeveel.Aggregator.Loaders;
using Hoeveel.Aggregator.Mappers;
using Hoeveel.Aggregator.Models.Raw;
using Hoeveel.Aggregator.Aggregators;

var sourceConfig = SourceConfigLoader.Load();

// 1. Download CSV
await CsvSourceDownloader.DownloadAsync(
    sourceConfig.uifwSourceUrl,
    sourceConfig.uifwSourcePath);

// 2. Load CSV → raw rows
var uifwRows = CsvLoader.Load(
    sourceConfig.uifwSourcePath,
    UifwSourceMapper.Map);

// 3. Aggregate
var aggregated = UifwAggregator.AggregateByMunicipalityAndYear(uifwRows);

// 4. Verify
Console.WriteLine($"Aggregated {aggregated.Count} municipality-year rows");

