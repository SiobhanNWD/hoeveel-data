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
Hoeveel.Aggregator/                                   **Main aggregation service for municipal finance data**

├─ builders/                                          **Builds stored domain entities from raw data**
│  ├─ MunicipalityBuilder.cs                          `Creates Municipality entities from UIFW & Census rows`
│  ├─ ProvinceBuilder.cs                              `Builds Province entities from municipalities & uses Census data`
│  └─ NationBuilder.cs                                `Builds the Nation entity by aggregating provinces`
│
├─ config/                                            **Runtime configuration files**
│  └─ sources.json                                    `Defines external data sources (UIFW API, Census CSVs, file paths, years)`
│
├─ data/                                              **Local data storage (not source code)**
│  ├─ raw/                                            **Downloaded source data exactly as received**
│  │  ├─ uifw-facts_2022.json                         `Raw UIFW expenditure facts from Treasury API`
│  │  ├─ census_municipalities_2022.csv               `Raw Census municipality population dataset`
│  │  └─ census_provinces_2022.csv                    `Raw Census province population dataset`
│  │
│  └─ stored/                                         **Future output location for processed domain data**
│
├─ loaders/                                           **Infrastructure for downloading and loading data**
│  ├─ FileSourceDownloader.cs                         `Generic file downloader (supports JSON & CSV)`
│  ├─ JsonLoader.cs                                   `Generic JSON deserializer from file to typed objects`
│  ├─ CsvLoader.cs                                    `Generic CSV file loader mapping rows via delegate`
│  └─ SourceConfigLoader.cs                           `Loads and parses config/sources.json into typed config`
│
├─ mappers/                                           **Row-to-model mappers for CSV-based datasets**
│  ├─ CensusMuniMapper.cs                             `Maps municipality census CSV row to CensusMuniRow`
│  └─ CensusProvMapper.cs                             `Maps province census CSV row to CensusProvRow`
│
├─ models/                                            **All strongly-typed data models**
│  ├─ config/                                         `Typed representations of source configuration`
│  │  ├─ SourceConfig.cs                              `Root config model mapping sources.json`
│  │  ├─ UifwSourceConfig.cs                          `UIFW-specific source config and URL builders`
│  │  └─ CensusSourceConfig.cs                        `Census CSV source configuration`
│  │
│  ├─ raw/                                            **Models that match external source data exactly**
│  │  ├─ TreasuryFactsResponse.cs                     `Envelope for Treasury /facts API responses`
│  │  ├─ UifwRow.cs                                   `Single UIFW fact row (municipality, item, amount)`
│  │  ├─ CensusMuniRow.cs                             `Single municipality census row`
│  │  └─ CensusProvRow.cs                             `Single province census row`
│  │
│  └─ stored/                                         **Internal stored domain entities used by the system**
│     ├─ Municipality.cs                              `Municipality with aggregated UIFW values`
│     ├─ Province.cs                                  `Province aggregating municipalities`
│     └─ Nation.cs                                    `National roll-up entity aggregating provinces`
│
├─ Program.cs                                         `Execution entry point and current integration test harness`
├─ README.md                                          `Project documentation and architecture overview`
├─ Hoeveel.Aggregator.csproj                          `C# project definition`
├─ Hoeveel.Aggregator.sln                             `Visual Studio solution file`
├─ .gitignore                                         `Ignored files and folders`
└─ obj/                                               **Build artefacts (generated)**


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
- Methods: Camel Case (e.g. )
- Scripts: Camel Case & if Source-specific, add the source name at the beginning (e.g. UifwSourceConfig.cs)


===============================================================
                    Basic Logic (Planned)
===============================================================
0. Load source configuration from config/sources.json
1. Build source URLs and output file paths from configuration
2. Download raw source files (UIFW JSON + Census CSVs + `Elections`)
   (**JsonSourceDownloader** / **FileSourceDownloader**)
3. Load and deserialize JSON/CSV into strongly typed raw rows
   (**JsonLoader** / **CsvLoader** + Mappers)
4. Build domain entities from raw data
   - Municipalities (UIFW + Census enrichment + `Elections data`)
   - Provinces (aggregate municipalities + Census enrichment + `Elections data`)
   - Nation (aggregate provinces + `Elections data`)
   (**MunicipalityBuilder** / **ProvinceBuilder** / **NationBuilder**)
5. Export structured domain JSON to data/stored/
6. Commit updated output (Nightly via GitHub Actions)


===============================================================
                    Basic Logic (Current)
===============================================================
1. Load source configuration and sources from `sources.json`
    - Build Treasury API URL from UIFW config
    - Read Census CSV URLs from config
2. Download:
   - UIFW facts JSON (JsonSourceDownloader)
   - Census municipality CSV (FileSourceDownloader)
   - Census province CSV (FileSourceDownloader)
3. Deserialize:
   - UIFW JSON → TreasuryFactsResponse<UifwRow>
   - Census municipality CSV → List<CensusMuniRow>
   - Census province CSV → List<CensusProvRow>
4. Build Municipalities from UIFW facts and enrich with Census data
5. Build Provinces from Municipalities and enrich with Census data
6. Build Nation from Provinces
7. (Next) Export structured JSON per financial year


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
- `string` **governingParty** – The governing party of this Province (from `Elections`)

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
- `string` **governingParty** – The governing party of this Province (from `Elections`)

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
- `string` **governingParty** – The governing party of this Province (from `Elections`)

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