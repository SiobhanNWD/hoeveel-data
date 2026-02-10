===============================================================
                HOEVEEL DATA & AGGREGATOR
===============================================================
This project aggregates official South African public finance data
into a hierarchical structure that mirrors the country’s system of government.

All data is generated programmatically from official CSV and JSON API sources
and is intended to be stored as structured JSON (one file per financial year).


===============================================================
                    Required Exstensions
===============================================================
- .NET Install Tool (Microsoft)
- C/C++ (Microsoft)
- C# (Microsoft)
- C# Dev Kit (Microsoft)
- Edit CSV (janisdd)
- GitHub Pull Requests (GitHub)
- JSON Tools (Erik Lynd)
- REST Client (Huachao Mao)
# Please ensure you have .NET 8 SDK installed

Suggested:
- Better Comments (Aaron Bond)


===============================================================
                        File Structure
===============================================================

Hoeveel.Aggregator/                                  **Main aggregation service for municipal finance data**

├─ .github/                                          **GitHub Actions workflows & repo config**
│  └─ workflows/
│     └─ *.yml                                       `CI / scheduled aggregation workflows`
│
├─ builders/                                         **Builds stored domain entities from raw data**
│  ├─ MunicipalityBuilder.cs                         `Creates Municipality entities from UIFW & Census rows`
│  ├─ ProvinceBuilder.cs                             `Builds Province entities from municipalities & Census data`
│  └─ NationBuilder.cs                               `Builds the Nation entity by aggregating provinces`
│
├─ config/                                           **Runtime configuration files**
│  └─ sources.json                                   `Defines external data sources (UIFW API, Census CSVs, years, paths)`
│
├─ data/                                             **Local data storage (non-code assets)**
│  ├─ raw/                                           **Downloaded source data exactly as received**
│  │  ├─ census-muni_2022.csv                         `Municipality population dataset (Census)`
│  │  ├─ census-province_2022.csv                     `Province population dataset (Census)`
│  │  ├─ elections-2021-consolidated.csv              `Municipal election results`
│  │  ├─ person-indicators-province.csv               `Provincial demographic indicators`
│  │  └─ uifw-facts_2022.json                          `Raw UIFW expenditure facts from Treasury API`
│  │
│  └─ stored/                                        **Generated output data (committed by Actions)**
│     └─ hoeveel-data_2022.json                       `Aggregated national/provincial/municipal dataset`
│
├─ deprecated/                                       **Legacy or superseded components (not active)**
│
├─ exporters/                                        **Output writers / serializers**
│  └─ JsonExportService.cs                            `Exports stored domain models to JSON`
│
├─ loaders/                                          **Infrastructure for downloading and loading data**
│  ├─ CsvLoader.cs                                   `Generic CSV loader mapping rows via mapper`
│  ├─ CsvSourceDownloader.cs                          `Downloads CSV sources defined in config`
│  ├─ ElectionsSourceDownloader.cs                    `Election-specific source downloader`
│  ├─ FileSourceDownloader.cs                         `Generic file downloader (JSON & CSV)`
│  ├─ JsonLoader.cs                                  `Generic JSON deserializer to typed objects`
│  ├─ JsonSourceDownloader.cs                         `Downloads JSON sources defined in config`
│  └─ SourceConfigLoader.cs                           `Loads and parses config/sources.json`
│
├─ mappers/                                          **Row-to-model mappers for CSV-based datasets**
│  ├─ CensusMuniMapper.cs                            `Maps municipality census CSV row`
│  ├─ CensusProvMapper.cs                            `Maps province census CSV row`
│  ├─ ElectionsCSVMapper.cs                          `Maps elections CSV rows`
│  └─ ElectionsMapper.cs                             `Normalizes election data into domain models`
│
├─ models/                                           **All strongly-typed data models**
│  ├─ config/                                        **Typed representations of source configuration**
│  │  ├─ CsvSourceConfig.cs                           `CSV source configuration`
│  │  ├─ ExportOptionsConfig.cs                       `Controls export behaviour & output paths`
│  │  ├─ GithubUrlConverter.cs                        `Normalizes GitHub-hosted raw URLs`
│  │  ├─ SourceConfig.cs                              `Root config model mapping sources.json`
│  │  └─ UifwSourceConfig.cs                          `UIFW-specific source config and URL builders`
│  │
│  ├─ raw/                                           **Models matching external source schemas exactly**
│  │  ├─ CensusMuniRow.cs                             `Municipality census row`
│  │  ├─ CensusProvRow.cs                             `Province census row`
│  │  ├─ ElectionsRow.cs                              `Election results row`
│  │  ├─ TreasuryFactsResponse.cs                     `Envelope for UIFW /facts API responses`
│  │  └─ UifwRow.cs                                   `Single UIFW expenditure fact`
│  │
│  └─ stored/                                        **Internal aggregated domain entities**
│     ├─ Municipality.cs                              `Municipality with aggregated indicators`
│     ├─ Province.cs                                  `Province aggregating municipalities`
│     └─ Nation.cs                                    `National roll-up entity`
│
├─ utils/                                            **Shared helpers & utilities**
│
├─ Program.cs                                        `Application entry point (aggregation orchestration)`
├─ Hoeveel.Aggregator.csproj                          `C# project definition (.NET 8)`
├─ Hoeveel.Aggregator.sln                             `Visual Studio solution`
├─ .gitignore                                        `Ignored files and folders`
├─ bin/                                               `Build outputs (generated)`
└─ obj/                                               `Intermediate build artefacts (generated)`


===============================================================
                      System Overview
===============================================================
Hoeveel.Aggregator is a deterministic data‑aggregation service that converts official South African public datasets into a structured, hierarchical JSON representation of government finances and governance.

The system consumes:
- UIFW (Treasury) financial data
- Census population data
- IEC 2021 Municipal Election results
and produces a single structured output per financial year.

Aggregation hierarchy:
**Municipality → Province → Nation**

Raw source files are stored exactly as received. All aggregation and enrichment is performed programmatically.


===============================================================
                     Aggregation Logic
===============================================================
1. Load source configuration from config/sources.json
2. Build source URLs and output file paths from configuration
3. Download raw source files (UIFW JSON + Census CSVs + Elections CSV)
   (**JsonSourceDownloader** / **FileSourceDownloader** / **ElectionsSourceDownloader**)
4. Load and deserialize JSON/CSV into strongly typed rows using mappers
   (**JsonLoader** / **CsvLoader**)
   (**CensusProvMapper** / **CensusMuniMapper** / **ElectionsCSVMapper** / **ElectionsMapper**)
   (**TrasuryFactsRespnse** / **UifwRow** / **CensusMuniRow** / **CensusProvRow** / **ElectionsRow**)
5. Build domain entities from raw data
   - Municipalities (UIFW + Census enrichment + Elections data)
   - Provinces (aggregate municipalities + Census enrichment + Elections data)
   - Nation (aggregate provinces + Elections data)
   (**MunicipalityBuilder** / **ProvinceBuilder** / **NationBuilder**)
6. Export structured domain JSON to data/stored/
   (**JsonExportService**)
7. Commit updated output to main (Nightly via GitHub Actions)


===============================================================
                     Data Entity Structure
===============================================================
This project aggregates official public finance (UIFW) and population data
into a hierarchical structure that mirrors South Africa’s system of government.

All data is generated programmatically from official public sources
(Treasury API + Census CSV + `Elections` files) and stored as structured JSON (one file per financial year).

Aggregation flow: `Municipality → Province → Nation`
Raw source files are stored separately from generated outputs.

# Nation
Represents South Africa as a whole.
Population and expenditure values are dynamically calculated from Provinces.

# Properties:
- `string` **name** – Country name (fixed: "South Africa")
- `List<Province>` **provinces** – List of provincial entities
- `decimal` **population** – Sum of provincial populations
- `decimal` **unauthorised** – Sum of provincial unauthorised expenditure
- `decimal` **irregular** – Sum of provincial irregular expenditure
- `decimal` **fruitless** – Sum of provincial fruitless expenditure
- `decimal` **uifw** – Sum of unauthorised + irregular + fruitless expenditure
- `string` **governingParty** – The governing party of this Province (from Elections)

# Notes:
- All monetary values are calculated from Provinces (not stored independently)
- Population is calculated from Provinces
- No values are manually set at national level


# Province
Represents a South African province.
Expenditure values are dynamically calculated from Municipalities.

# Properties:
- `string` **name** – Province name (e.g. Gauteng)
- `string` **code** – Province code (e.g. GP)
- `int` **population** – Total provincial population (from Census CSV)
- `List<Municipality>` **municipalities** – Municipal entities belonging to this Province
- `decimal` **unauthorised** – Sum of municipal unauthorised expenditure 
- `decimal` **irregular** – Sum of municipal irregular expenditure
- `decimal` **fruitless** – Sum of municipal fruitless expenditure
- `decimal` **uifw** – Sum of municipal UIFW values
- `string` **governingParty** – The governing party of this Province (from Elections)

# Notes:
- Expenditure is never stored directly; it is always calculated
- Population is sourced from Census province CSV
- Aggregation is dynamic via LINQ Sum()


# Municipality
Represents a local or metropolitan municipality.
This is the primary financial aggregation level.

# Properties:
- `string` **code** – Demarcation code (e.g. JHB, BUF)
- `string` **name** – Municipality name (from Census CSV)
- `string` **provinceCode** – Province code (used for Province grouping)
- `int` **population** – Municipal population (from Census CSV)
- `decimal` **unauthorised** – Reported unauthorised expenditure (from UIFW API)
- `decimal` **irregular** – Reported irregular expenditure (from UIFW API)
- `decimal` **fruitless** – Reported fruitless & wasteful expenditure (from UIFW API)
- `decimal` **uifw** – Calculated total (unauthorised + irregular + fruitless)
- `string` **governingParty** – The governing party of this Province (from Elections)

# Notes:
- Municipalities are built only from UIFW data
- Census data is used strictly for enrichment (name, population, province code)
- UIFW is the primary dataset driving the system


===============================================================
                     Architectural Principles
===============================================================
- All monetary values are reported per financial year
- No new financial data is created — values are derived strictly from official public sources
- Aggregation is dynamic (no duplicated totals stored at multiple levels)
- Population is sourced from official Census datasets
- Separation of concerns:
    Raw Models → Builders → Stored Domain Models
- The system is deterministic and reproducible per financial year


===============================================================
                    Comment Structure
===============================================================
# Commenting Methods:
`//` Describe what the methd does/ purpose. A short summary
`// Input:` Describe what the input is.
`// Output:` Describe what the output is
void MyMethod(int input) {...}

# Code Blocks/ Processes:
For large or lengthy processes please group the steps with a comment and headers, e.g.: 
`// ================ HEADER ================`
`// 1.` Do xyz

# If you have Better Comments Exstension by Aaron Bond:
// * Imporatant Information
// ! Warnings
// ? Questions/ Considerations
// TODO: Things still to do OR Future features to implement
// @param for parameters



===============================================================
                    Naming Conventions
===============================================================
- Properties: Camel Case (e.g. public int WasteAmount {get; set;})
- Methods: Camel Case (e.g. BuildMunicipalities())
- Scripts: Camel Case 
- Source-specific files are prefixed with the source name (e.g. UifwSourceConfig.cs, ElectionsSourceDownloader.cs)







===============================================================
         Elections Integration (2021 Municipal Elections)
===============================================================

This section documents the integration of IEC (Independent Electoral Commission)
2021 municipal election results to determine the governing party per municipality.

## New Files Added

### Loaders
- **loaders/ElectionsSourceDownloader.cs** (306 lines)
  - Downloads 2021 election results from all 9 SA provinces
  - IEC URL pattern: `https://results.elections.org.za/home/LGEPublicReports/1091/Downloadable%20Party%20Results/{PROVINCE_CODE}.csv`
  - Consolidates all provinces into single CSV file
  - Province codes: EC, FS, GP, KN, NP, MP, NW, NC, WP
  - Output: `data/raw/elections-2021-consolidated.csv` (65.6 MB, 1,084,734 records)

### Models
- **models/raw/ElectionsRow.cs**
  - Represents single election result row from IEC CSV
  - Properties: MunicipalityCode, MunicipalityName, ProvinceCode, PartyName, Votes, VotePercentage

### Mappers
- **mappers/ElectionsCSVMapper.cs**
  - Maps consolidated elections CSV row to ElectionsRow object
  - Used by CsvLoader for deserialization
  
- **mappers/ElectionsMapper.cs**
  - Aggregates election results by municipality
  - Determines governing party (highest vote count per municipality)
  - Methods: `GetGoverningPartyByMunicipality()`, `GetDetailedGoverningPartyByMunicipality()`

### Utilities
- **utils/FileHelpers.cs**
  - Centralized directory creation utility
  - Method: `EnsureDirectoryForFile(string? filePath)`
  - Used by CsvSourceDownloader and ElectionsSourceDownloader for safe directory handling

## Modified Files

### builders/MunicipalityBuilder.cs
- Added `using Hoeveel.Aggregator.Mappers;`
- Updated `BuildMunicipalities()` signature to accept optional `List<ElectionsRow>? electionsRows`
- Implemented `ApplyElectionsData()` method:
  - Groups election rows by municipality + party
  - Sums votes per party per municipality
  - Determines winning party (highest vote count)
  - Sets `Municipality.GoverningParty` property
- Updated call flow to include elections enrichment after census data

### models/stored/Municipality.cs
- Added `public string GoverningParty { get; set; } = "";`
- Represents the winning party from 2021 elections

### Program.cs
- Load consolidated elections CSV if it exists
- Pass election data through the full pipeline
- Enhanced sanity checks to display Municipality name, ProvinceCode, and GoverningParty
- Elections download is automatic (checked on startup)

### loaders/CsvSourceDownloader.cs
- Refactored directory creation to use `FileHelpers.EnsureDirectoryForFile()`
- Safe handling of null/empty file paths

### loaders/ElectionsSourceDownloader.cs
- Uses `FileHelpers.EnsureDirectoryForFile()` for output directory creation

## Integration Architecture

**Download Flow:**
```
IEC Website (9 Provinces)
    ↓
ElectionsSourceDownloader.DownloadAndConsolidateAsync()
    ↓
data/raw/elections-2021-consolidated.csv (consolidated)
```

**Pipeline Flow:**
```
1. Load UIFW facts (JSON from Treasury API)
2. Load Census municipality data (CSV)
3. Load Elections data (CSV) — optional, gracefully skipped if missing
4. Build Municipalities:
   - Step 1: Create shells from UIFW codes
   - Step 2: Apply UIFW financial aggregation
   - Step 3: Apply Census enrichment (name, population, province code)
   - Step 4: Apply Elections data (governing party per municipality)
5. Build Provinces (pending: add elections aggregation)
6. Build Nation (pending: add elections aggregation)
```

## Data Discovery

IEC election results CSV structure (per province):
```
Province,Municipality,Ward,VotingDistrict,VotingStationName,RegisteredVoters,BallotType,SpoiltVotes,PartyName,TotalValidVotes,DateGenerated
```

Municipality codes in IEC CSV format: "FS161 - Letsemeng" (code extracted as "FS161")

Consolidated CSV output structure:
```
MunicipalityCode,MunicipalityName,ProvinceCode,PartyName,Votes,VotePercentage
```

## Pending Work

1. **Province Elections Aggregation:**
   - Aggregate municipality governing parties to determine provincial winning party
   - Update `ProvinceBuilder` to apply elections data
   - Add `Province.GoverningParty` property

2. **National Elections Aggregation:**
   - Aggregate provincial governing parties for national result
   - Update `NationBuilder` to apply elections data
   - Add `Nation.GoverningParty` property

3. **Configuration Integration:**
   - Add elections source config to `sources.json` (elections-2021 entry)
   - Add `Elections2021` property mapping to `SourceConfig.cs`
   - Enable config-driven elections data loading

4. **Testing & Validation:**
   - Verify municipality code matching between UIFW, Census, and Elections datasets
   - Validate aggregation logic with sample municipalities
   - Cross-check with official IEC results

## Notes

- Elections data is optional; pipeline gracefully handles missing elections CSV
- Governing party is determined by **highest vote count** per municipality (not percentage)
- Vote percentages in consolidated CSV are set to 0m (not calculated from raw data)
- All 9 provinces download successfully; no missing province data
- Total of 1,084,734 election records consolidated into single file
- ElectionsSourceDownloader implements flexible CSV parsing to handle IEC format variations