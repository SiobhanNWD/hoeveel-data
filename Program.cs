using Hoeveel.Aggregator.Loaders;
using Hoeveel.Aggregator.Models.Raw;   // TreasuryFactsResponse, UifwRow, CensusMuniRow
using Hoeveel.Aggregator.Models.Config;
using Hoeveel.Aggregator.Mappers;     // CensusMuniMapper
using Hoeveel.Aggregator.Builders;    // MunicipalityBuilder
using Hoeveel.Aggregator.Exporters;   // JsonExportService

//* ================== CENSUS, UIFW  & ELECTIONS DOWNLOAD ==================
Console.ForegroundColor = ConsoleColor.Blue;  // Pick your colour here
Console.WriteLine("Downloading Census, UIFW and Elections files...");
Console.ResetColor();                          // Always reset afterwards

// 1. Get the Census and UIFW source configuration from config/sources.json
var sourceConfig = SourceConfigLoader.Load();   // Load all source configuration from config/sources.json

var uifwSource = sourceConfig.Uifw;                     // Extract UIFW-specific source configuration
var uifwUrl = uifwSource.BuildUrl();                    // Build the UIFW facts URL from config values
var uifwFilePath = uifwSource.FilePath;                 // Output file path defined in config

var censusMuniSource = sourceConfig.CensusMuni;         // Extract UIFW-specific source configuration
var censusMuniUrl = censusMuniSource.Url;               // Build the UIFW facts URL from config values
var censusMuniFilePath = censusMuniSource.FilePath;     // Output file path defined in config

var censusProvSource = sourceConfig.CensusProv;         // Extract UIFW-specific source configuration
var censusProvUrl = censusProvSource.Url;               // Build the UIFW facts URL from config values
var censusProvFilePath = censusProvSource.FilePath;     // Output file path defined in config

var electionsSource = sourceConfig.Elections;           // Extract Elections source configuration
var electionsFilePath = electionsSource.FilePath;       // Output file path defined in config 

// 2. Download Census, UIFW and Elections files to disk
Console.WriteLine("Downloading uifw JSON...");
await JsonSourceDownloader.DownloadAsync(uifwUrl, uifwFilePath);

Console.WriteLine("Downloading census municipality CSV...");
await FileSourceDownloader.DownloadAsync(censusMuniUrl, censusMuniFilePath);

Console.WriteLine("Downloading census province CSV...");
await FileSourceDownloader.DownloadAsync(censusProvUrl, censusProvFilePath);

Console.WriteLine("Downloading consolidated elections CSV...");
if (!File.Exists(electionsFilePath))
{
    await ElectionsSourceDownloader.DownloadAndConsolidateAsync(electionsFilePath);
}

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine("Census, UIFW and Elections file downloads test complete.");
Console.ResetColor();                          // Always reset afterwards


//* ================== CENSUS, UIFW & ELECTIONS LOADING & DESERIALIZING  ==================
Console.ForegroundColor = ConsoleColor.Blue;  // Pick your colour here
Console.WriteLine("Loading and deserializing Census, UIFW and Elections files...");
Console.ResetColor();

// 1. Load and deserialize UIFW & Census files into strongly typed rows
var uifwRows = JsonLoader.Load<TreasuryFactsResponse<UifwRow>>(uifwFilePath);    // Load UIFW facts JSON and deserialize to strongly typed UifwRow objects
var censusMuniRows = CsvLoader.Load(censusMuniFilePath, CensusMuniMapper.Map);   // Load and map census municipality CSV to strongly typed rows
var censusProvRows = CsvLoader.Load(censusProvFilePath, CensusProvMapper.Map);   // Load and map census province CSV to strongly typed rows

// Elections: load consolidated elections CSV if present
List<ElectionsRow> electionsRows = new List<ElectionsRow>();
if (File.Exists(electionsFilePath))
{
    electionsRows = CsvLoader.Load(electionsFilePath, ElectionsCSVMapper.Map);   // Load and map elections CSV to strongly typed rows
}

// 2. Print some rows for verification purposes
foreach (var row in uifwRows.Data.Take(2))      
{    Console.WriteLine($"UIFW: Municipality={row.DemarcationCode}, " + $"Year={row.FinancialYear}, " + $"Item={row.ItemCode}, " + $"Amount={row.Amount}");}

foreach (var row in censusMuniRows.Take(2))
{    Console.WriteLine($"CENSUS MUNI: Municipality={row.MunicipalityCode}, " + $"Name={row.Name}, " + $"Province={row.ProvinceCode}, " + $"Population={row.Population2022}");}

foreach (var row in censusProvRows.Take(2))      // Print 5 of the rows for verification purposes
{    Console.WriteLine($"CENSUS PROV: Province={row.ProvinceCode}, " + $"Name={row.Name}, " + $"Population={row.Population2022}");}

if (electionsRows.Count > 0)
{
    foreach (var row in electionsRows.Take(2))
    {
        Console.WriteLine($"ELECTIONS: Municipality={row.MunicipalityCode}, " + $"Party={row.PartyName}, " + $"Votes={row.Votes}");
    }
}

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine("Census, UIFW and Elections loading & deserializing complete.");
Console.ResetColor();                          // Always reset afterwards


//* ================== BUILDING MUNICIPALITIES, PROVINCES & NATION WITH UIFW, CENSUS & ELECTIONS DATA  ==================
Console.ForegroundColor = ConsoleColor.Blue;  // Pick your colour here
Console.WriteLine("Building municipalities, provinces and nation with UIFW, Census and Elections data...");
Console.ResetColor();

// 1. Build Municipality aggregates from UIFW facts and enrich with Census data
var municipalities = MunicipalityBuilder.BuildMunicipalities(uifwRows, censusMuniRows, electionsRows);

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
        $"UIFW={m.Uifw}" +
        $", GoverningParty={m.GoverningParty}"
    );
}

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine($"Municipality building complete with {municipalities.Count} municipalities.");
Console.ResetColor(); 

// 2. Build Province aggregates from Municipalities and enrich with Census and elections data
var provinces = ProvinceBuilder.BuildFromMunicipalities(municipalities, censusProvRows, electionsRows);

foreach (var p in provinces.Take(3))
{
    Console.WriteLine(
        $"Province={p.Code}, " +
        $"Name={p.Name}, " +
        $"Population={p.Population}, " +
        $"Unauthorised={p.Unauthorised}, " +
        $"Irregular={p.Irregular}, " +
        $"Fruitless={p.Fruitless}, " +
        $"UIFW={p.Uifw}" +
        $", GoverningParty={p.GoverningParty}"
    );
}

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine($"Province building complete with {provinces.Count} provinces.");
Console.ResetColor(); 

var nation = NationBuilder.BuildFromProvinces(provinces, electionsRows);

 Console.WriteLine(
        $"Name={nation.Name}, " +
        $"Population={nation.Population}, " +
        $"Unauthorised={nation.Unauthorised}, " +
        $"Irregular={nation.Irregular}, " +
        $"Fruitless={nation.Fruitless}, " +
        $"UIFW={nation.Uifw}" +
        $", GoverningParty={nation.GoverningParty}"
    );

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine($"Nation building complete.");
Console.ResetColor();     

//* ================== EXPORTING STRUCTURED JSON OUTPUT ==================
Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine("Exporting Nation JSON to disk...");
Console.ResetColor();

// 1. Define output path (per financial year)
var outputPath = sourceConfig.ExportOptions.FilePath;   // Output file path defined in config for the final JSON export

// 2. Export Nation object
JsonExportService.ExportNation(outputPath, nation, sourceConfig.ExportOptions.JsonSettings);   // Exports the Nation object to JSON using the specified export options from config


Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("JSON export complete.");
Console.ResetColor();
