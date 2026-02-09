using Hoeveel.Aggregator.Loaders;
using Hoeveel.Aggregator.Models.Raw;   // TreasuryFactsResponse, UifwRow, CensusMuniRow
using Hoeveel.Aggregator.Models.Config;
using Hoeveel.Aggregator.Mappers;     // CensusMuniMapper
using Hoeveel.Aggregator.Builders;    // MunicipalityBuilder

//* ================== CENSUS AND UIFW DOWNLOAD ==================
Console.ForegroundColor = ConsoleColor.Blue;  // Pick your colour here
Console.WriteLine("Downloading Census and UIFW files...");
Console.ResetColor();                          // Always reset afterwards

// 1. Get the Census and UIFW source configuration from config/sources.json
var sourceConfig = SourceConfigLoader.Load();   // Load all source configuration from config/sources.json

var uifwSource = sourceConfig.Uifw;             // Extract UIFW-specific source configuration
var uifwUrl = uifwSource.BuildUrl();           // Build the UIFW facts URL from config values
var uifwFilePath = uifwSource.FilePath;      // Output file path defined in config

var censusMuniSource = sourceConfig.CensusMuni;             // Extract UIFW-specific source configuration
var censusMuniUrl = censusMuniSource.Url;           // Build the UIFW facts URL from config values
var censusMuniFilePath = censusMuniSource.FilePath;      // Output file path defined in config

var censusProvSource = sourceConfig.CensusProv;             // Extract UIFW-specific source configuration
var censusProvUrl = censusProvSource.Url;           // Build the UIFW facts URL from config values
var censusProvFilePath = censusProvSource.FilePath;      // Output file path defined in config

// 2. Download Census and UIFW files to disk
Console.WriteLine("Downloading uifw JSON...");
await JsonSourceDownloader.DownloadAsync(uifwUrl, uifwFilePath);

Console.WriteLine("Downloading census municipality CSV...");
await FileSourceDownloader.DownloadAsync(censusMuniUrl, censusMuniFilePath);

Console.WriteLine("Downloading census province CSV...");
await FileSourceDownloader.DownloadAsync(censusProvUrl, censusProvFilePath);

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine("Census and UIFW file downloads test complete.");
Console.ResetColor();                          // Always reset afterwards


//* ================== CENSUS AND UIFW LOADING & DESERIALIZING  ==================
Console.ForegroundColor = ConsoleColor.Blue;  // Pick your colour here
Console.WriteLine("Loading and deserializing Census and UIFW files...");
Console.ResetColor();

// 1. Load and deserialize UIFW & Census files into strongly typed rows
var uifwRows = JsonLoader.Load<TreasuryFactsResponse<UifwRow>>(uifwFilePath);    // Load UIFW facts JSON and deserialize to strongly typed UifwRow objects
var censusMuniRows = CsvLoader.Load(censusMuniFilePath, CensusMuniMapper.Map);   // Load and map census municipality CSV to strongly typed rows
var censusProvRows = CsvLoader.Load(censusProvFilePath, CensusProvMapper.Map);   // Load and map census province CSV to strongly typed rows

// 2. Print some rows for verification purposes
foreach (var row in uifwRows.Data.Take(2))      
{    Console.WriteLine($"UIFW: Municipality={row.DemarcationCode}, " + $"Year={row.FinancialYear}, " + $"Item={row.ItemCode}, " + $"Amount={row.Amount}");}

foreach (var row in censusMuniRows.Take(2))
{    Console.WriteLine($"CENSUS MUNI: Municipality={row.MunicipalityCode}, " + $"Name={row.Name}, " + $"Province={row.ProvinceCode}, " + $"Population={row.Population2022}");}

foreach (var row in censusProvRows.Take(2))      // Print 5 of the rows for verification purposes
{    Console.WriteLine($"CENSUS PROV: Province={row.ProvinceCode}, " + $"Name={row.Name}, " + $"Population={row.Population2022}");}

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine("Census and UIFW loading & deserializing complete.");
Console.ResetColor();                          // Always reset afterwards


//* ================== BUILDING MUNICIPALITIES, PROVINCES & NATION WITH UIFW & CENSUS DATA  ==================
Console.ForegroundColor = ConsoleColor.Blue;  // Pick your colour here
Console.WriteLine("Building municipalities, provinces and nation with UIFW and Census data...");
Console.ResetColor();

// 1. Build Municipality aggregates from UIFW facts and enrich with Census data
var municipalities = MunicipalityBuilder.BuildMunicipalities(uifwRows, censusMuniRows);

foreach (var m in municipalities.Take(3))
{
    Console.WriteLine(
        $"Municipality={m.Code}, " +
        $"Name={m.Name}, " +
        $"Population={m.Population}, " +
        $"Province={m.ProvinceCode}, " +
        $"Unauthorised={m.Unauthorised}, " +
        $"Irregular={m.Irregular}, " +
        $"Fruitless={m.Fruitless}, " +
        $"UIFW={m.Uifw}"
    );
}

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine($"Municipality building complete with {municipalities.Count} municipalities.");
Console.ResetColor(); 

// 2. Build Province aggregates from Municipalities and enrich with Census data
var provinces = ProvinceBuilder.BuildFromMunicipalities(municipalities, censusProvRows);

foreach (var p in provinces.Take(3))
{
    Console.WriteLine(
        $"Province={p.Code}, " +
        $"Name={p.Name}, " +
        $"Population={p.Population}, " +
        $"Unauthorised={p.Unauthorised}, " +
        $"Irregular={p.Irregular}, " +
        $"Fruitless={p.Fruitless}, " +
        $"UIFW={p.Uifw}"
    );
}

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine($"Province building complete with {provinces.Count} provinces.");
Console.ResetColor(); 

var nation = NationBuilder.BuildFromProvinces(provinces);

 Console.WriteLine(
        $"Name={nation.Name}, " +
        $"Population={nation.Population}, " +
        $"Unauthorised={nation.Unauthorised}, " +
        $"Irregular={nation.Irregular}, " +
        $"Fruitless={nation.Fruitless}, " +
        $"UIFW={nation.Uifw}"
    );

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine($"Nation building complete.");
Console.ResetColor();     
