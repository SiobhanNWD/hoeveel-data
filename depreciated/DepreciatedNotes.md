The hoeveel.aggregator/depreciated folder (aka. script graveyard) contains all files that are no longer in use.
These scripts were moved here instead of deleted in the case that they need to be easily restored. 
_dp has been added behind all files and classes in this folder to prevent future naming issues.


Old Uifw csv sourcescofig.json:
{
  "uifwSourceUrl": "https://munimoney-media.s3.eu-west-1.amazonaws.com/munimoney-media/media/bulk_downloads/uifwexp_facts_v1/uifwexp_facts_v1__2023-09-01.csv",
  "uifwSourcePath": "data/raw/uifw-source_2012-2021.csv",
  "_uifwSourceFormat": ".csv - 2012 to 2021 - demarcation_code,financial_year,item_code,item_label,amount,id"
}

old program.cs
{
    using Hoeveel.Aggregator.Aggregators;
    using Hoeveel.Aggregator.Loaders;
    using Hoeveel.Aggregator.Mappers;

    var sourceConfig = SourceConfigLoader.Load();

    // 1. Download CSV
    await CsvSourceDownloader.DownloadAsync(
        sourceConfig.uifwSourceUrl,
        sourceConfig.uifwSourcePath);

    // 2. Load CSV → raw rows
    var uifwRows = CsvLoader.Load(
        sourceConfig.uifwSourcePath,
        UifwSourceMapper.Map);

    // 3. Aggregate by municipality and year
    var aggregated = UifwAggregator.AggregateByMunicipalityAndYear(uifwRows);

    // 4. Build municipality domain entities
    var municipalities = MunicipalityBuilder.BuildFromUifw(aggregated);

    // 5. Verify
    Console.WriteLine($"Built {municipalities.Count} municipalities");
}