using Hoeveel.Aggregator.Loaders;
using Hoeveel.Aggregator.Models.Raw;        // TreasuryFactsResponse, UifwRow, CensusMuniRow
using Hoeveel.Aggregator.Models.Config;
using Hoeveel.Aggregator.Mappers;           // CensusMuniMapper
using Hoeveel.Aggregator.Builders;          // MunicipalityBuilder
using Hoeveel.Aggregator.Exporters;         // JsonExportService
using Hoeveel.Aggregator.Models.Stored;     // Municipality, Province, Nation, JsonExportInfo

//await FileSourceDownloader.DownloadAsync("https://municipaldata.treasury.gov.za/api/cubes/uifwexp/members/financial_year_end.year", "data/raw/uifwYears");

// Get the Source configs from config/sources.json 
var sourceConfig = SourceConfigLoader.Load();               // Load all source configuration from config/sources.json
var uifwSource = sourceConfig.Uifw;                         // Extract UIFW-specific source configuration
var censusMuniSource = sourceConfig.CensusMuni;             // Extract UIFW-specific source configuration
var censusProvSource = sourceConfig.CensusProv;             // Extract UIFW-specific source configuration
var electionsSource = sourceConfig.Elections;               // Extract Elections source configuration

int[] years = sourceConfig.ExportOptions.Years;             // Years to download and process, defined in config/sources.json (defaults to [2011, 2022] if not specified)
List<int> downloadedUifwYears = new List<int>();    
List<int> downloadedCensusYears = new List<int>();
List<int> downloadedElectionYears = new List<int>();        // Keep track of which election years we have downloaded to avoid re-downloading the same year multiple times when processing multiple target years that may map to the same nearest election year in the config when building municipalities/provinces/nation

foreach (var year in years)                                 // Loop through each year and perform the entire download, deserialize, build and export process for each year (this allows us to easily build historical data by just adding more years to the config in the future without changing any code)
{
    Console.ForegroundColor = ConsoleColor.DarkMagenta;  // Pick your colour here
    Console.WriteLine($"Downloading Deserializing and Exporting files for year {year}...");
    Console.ResetColor();                          // Always reset afterwards

    await DownloadDeserializeExportPerYear(year);
}

async Task DownloadDeserializeExportPerYear(int year)
{
    //* ================== CENSUS, UIFW  & ELECTIONS DOWNLOAD ==================
    Console.ForegroundColor = ConsoleColor.Blue;  // Pick your colour here
    Console.WriteLine("Downloading Census, UIFW and Elections files...");
    Console.ResetColor();                          // Always reset afterwards

    // 1. Get the Census and UIFW source configuration from config/sources.json
    
    int uifwYear = GetNearestYear(year, uifwSource.Years);                  // Get the nearest available year from the config for the target year
    var uifwUrl = uifwSource.BuildUrl(uifwYear);                            // Build the UIFW facts URL from config values
    var uifwFilePath = uifwSource.BuildPath(uifwYear);                      // Output file path defined in config

    int censusMuniYear = GetNearestYear(year, censusMuniSource.Years);      // Get the nearest available year from the config for the target year
    var censusMuniUrl = censusMuniSource.Url;                               // Build the UIFW facts URL from config values
    var censusMuniFilePath = censusMuniSource.BuildPath(censusMuniYear);    // Output file path defined in config

    int censusProvYear = GetNearestYear(year, censusProvSource.Years);      // Get the nearest available year from the config for the target year
    var censusProvUrl = censusProvSource.Url;                               // Build the UIFW facts URL from config values
    var censusProvFilePath = censusProvSource.BuildPath(censusProvYear);    // Output file path defined in config

    int electionsYear = GetNearestElectionsYear(year, electionsSource.Years);   // Get the nearest available year from the config for the target year
    var electionsFilePath = electionsSource.BuildPath(electionsYear);           // Output file path defined in config 
    
    Console.WriteLine($"Desired year: {year}, UIFW year: {uifwYear}, Census Muni year: {censusMuniYear}, Census Prov year: {censusProvYear}, Elections year: {electionsYear}");

    // 2. Download Census, UIFW and Elections files to disk (if the files for these years haven't been downloaded yet)
    Console.WriteLine($"Downloading sources for year {year}...");
        if (!downloadedUifwYears.Contains(uifwYear))
        {
            FileSourceDownloader.DownloadAsync(uifwUrl, uifwFilePath).Wait();
            downloadedUifwYears.Add(uifwYear);
        }
        if (!downloadedCensusYears.Contains(censusMuniYear))
        {
            FileSourceDownloader.DownloadAsync(censusMuniUrl, censusMuniFilePath).Wait();
            FileSourceDownloader.DownloadAsync(censusProvUrl, censusProvFilePath).Wait();
            downloadedCensusYears.Add(censusMuniYear);   // We can just track the municipality census years since they will be the same as the province census years in the config (if we wanted to be more precise we could track them separately but it would add complexity and in practice they will always be the same since they come from the same census datasets just different files for muni vs prov)
        }
        if (!downloadedElectionYears.Contains(electionsYear))
        {
            ElectionsSourceDownloader.DownloadAndConsolidateAsync(electionsFilePath, electionsYear).Wait();   // Download and consolidate elections data for the specified year (this will loop through the provinces and download each province file for the specified year and then consolidate them into a single file for that year)
            downloadedElectionYears.Add(electionsYear);
        }
        //Elections source is a bit different since we have multiple province files to download - we can loop through the provinces and download each one
        /*
        List<string> provinceFilePaths = new List<string>();
        foreach (var province in electionsSource.Provinces)
        {
            var provinceUrl = electionsSource.BuildProvinceUrl(province);
            var provinceFilePath = electionsSource.BuildPath(electionsYear, province);                           //Path.Combine(Path.GetDirectoryName(electionsFilePath) ?? "", $"elections-{province}_{year}.csv");
            Console.WriteLine($"Downloading elections CSV for province {province} and year {electionsYear}...");
            FileSourceDownloader.DownloadAsync(provinceUrl, provinceFilePath).Wait();
            provinceFilePaths.Add(provinceFilePath);
        }
        await ElectionsSourceDownloader.ConsolidateElectionsDataAsync(electionsFilePath, provinceFilePaths);
        */
        
    //* ================== CENSUS, UIFW & ELECTIONS LOADING & DESERIALIZING  ==================
    Console.ForegroundColor = ConsoleColor.Blue;  // Pick your colour here
    Console.WriteLine("Loading and deserializing Census, UIFW and Elections files...");
    Console.ResetColor();

    // 1. Load and deserialize UIFW & Census files into strongly typed rows
    var uifwRows = JsonLoader.Load<TreasuryFactsResponse<UifwRow>>(uifwFilePath);    // Load UIFW facts JSON and deserialize to strongly typed UifwRow objects
    var censusMuniRows = CsvLoader.Load(censusMuniFilePath, cols => CensusMuniMapper.Map(cols, censusMuniYear));   // Load and map census municipality CSV to strongly typed rows
    var censusProvRows = CsvLoader.Load(censusProvFilePath, cols => CensusProvMapper.Map(cols, censusProvYear));   // Load and map census province CSV to strongly typed rows
    var electionsRows = CsvLoader.Load(electionsFilePath, ElectionsCSVMapper.Map);   // Load and map elections CSV to strongly typed rows

    // 2. Print some rows for verification purposes
    foreach (var row in uifwRows.Data.Take(2))      
    {    Console.WriteLine($"UIFW: Municipality={row.DemarcationCode}, " + $"Year={row.FinancialYear}, " + $"Item={row.ItemCode}, " + $"Amount={row.Amount}");}

    foreach (var row in censusMuniRows.Take(2))
    {    Console.WriteLine($"CENSUS MUNI: Municipality={row.MunicipalityCode}, " + $"Name={row.Name}, " + $"Province={row.ProvinceCode}, " + $"Population={row.Population}");}

    foreach (var row in censusProvRows.Take(2))      // Print 5 of the rows for verification purposes
    {    Console.WriteLine($"CENSUS PROV: Province={row.ProvinceCode}, " + $"Name={row.Name}, " + $"Population={row.Population}");}

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
    var outputPath = sourceConfig.ExportOptions.BuildPath(year);   // Output file path defined in config for the final JSON export
    // 2. Define export info to include in the JSON (e.g. source years used for each dataset) - this is useful for tracking and documentation purposes to know exactly which source years were used for each export when we have multiple years of data available in the config and are picking the nearest year for each source based on the target year when building
    var jsonExportInfo = new JsonExportInfo
    {
        _year = year,
        _uifwYear = uifwYear,
        _censusMuniYear = censusMuniYear,
        _censusProvYear = censusProvYear,
        _electionsYear = electionsYear
    };

    // 3. Export Nation object
    JsonExportService.ExportNation(sourceConfig.ExportOptions.BuildPath(year), nation, jsonExportInfo, sourceConfig.ExportOptions.JsonSettings);
    //JsonExportService.ExportNation(sourceConfig.ExportOptions.BuildPath(year), nation, sourceConfig.ExportOptions.JsonSettings);   // Exports the Nation object to JSON using the specified export options from config

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("JSON export complete.");
    Console.ResetColor();
}

// Get the nearest available year from the config for a given target year (used for elections source since we have multiple years of data and need to pick the closest one to the target year for each municipality/province/nation when building)
int GetNearestYear(int targetYear, int[] availableYears)
{
    return availableYears
        .OrderBy(y => Math.Abs(y - targetYear))
        .FirstOrDefault();
}

// Gets the nearest available year that is less than or equal to the target year (used for sources where we want to ensure we don't pick a future year that doesn't have data yet when building)
int GetNearestElectionsYear(int targetYear, int[] availableYears)
{
    return availableYears
        .Where(y => y <= targetYear)
        .OrderByDescending(y => y)
        .FirstOrDefault();
}


/*
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
await FileSourceDownloader.DownloadAsync(uifwUrl, uifwFilePath);

Console.WriteLine("Downloading census municipality CSV...");
await FileSourceDownloader.DownloadAsync(censusMuniUrl, censusMuniFilePath);

Console.WriteLine("Downloading census province CSV...");
await FileSourceDownloader.DownloadAsync(censusProvUrl, censusProvFilePath);

Console.WriteLine("Downloading consolidated elections CSV...");
//if (!File.Exists(electionsFilePath))
//{
    //await ElectionsSourceDownloader.DownloadAndConsolidateAsync(electionsFilePath);
//}

foreach (var province in electionsSource.Provinces)
{
    var provinceUrl = electionsSource.BuildProvinceUrl(province);
    var provinceFilePath = Path.Combine(Path.GetDirectoryName(electionsFilePath) ?? "", $"elections-{province}.csv");
    Console.WriteLine($"Downloading elections CSV for province {province}...");
    await FileSourceDownloader.DownloadAsync(provinceUrl, provinceFilePath);
}

Console.ForegroundColor = ConsoleColor.Green;  // Pick your colour here
Console.WriteLine("Census, UIFW and Elections file downloads test complete.");
Console.ResetColor();                          // Always reset afterwards
*/
