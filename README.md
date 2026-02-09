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
│  ├─ MunicipalityBuilder.cs                          `Creates Municipality entities from UIFW fact rows`
│  ├─ NationBuilder.cs                                `Builds the Nation entity by aggregating provinces`
│  └─ ProvinceBuilder.cs                              `Builds Province entities from municipalities`
│
├─ config/                                            **Runtime configuration files**
│  └─ sources.json                                    `Defines external data sources (UIFW API, file paths, years)`
│
├─ data/                                              **Local data storage (not source code)**
│  ├─ raw/                                            **Downloaded source data exactly as received**
│  │  └─ uifw-facts_2022.json                          `Raw UIFW expenditure facts for 2022 from Treasury API`
│  │
│  ├─ stored/                                         **Future output location for processed domain data**
│  
├─ depreciated/                                       **Legacy CSV-based data and models (no longer used), aka script graveyard**
│
├─ loaders/                                           **Infrastructure for downloading and loading data**
│  ├─ CsvLoader.cs                                    `Generic CSV file loader (legacy / future use)`
│  ├─ CsvSourceDownloader.cs                          `Downloads CSV files from remote sources`
│  ├─ JsonLoader.cs                                   `Generic JSON deserializer from file to typed objects`
│  ├─ JsonSourceDownloader.cs                         `Downloads JSON files from APIs or URLs`
│  └─ SourceConfigLoader.cs                           `Loads and parses config/sources.json into typed config`
│
├─ mappers/                                           **Legacy row-to-model mappers (CSV era)**
│
├─ models/                                            **All strongly-typed data models**
│  ├─ config/                                         `Typed representations of source configuration`
│  │  ├─ SourceConfig.cs                              `Root config model mapping sources.json`
│  │  └─ UifwSourceConfig.cs                          `UIFW-specific source config and URL builders`
│  │
│  ├─ raw/                                            **Models that match external source data exactly**
│  │  ├─ TreasuryFactsResponse.cs                     `Envelope for Treasury /facts API responses`
│  │  └─ UifwRow.cs                                   `Single UIFW fact row (municipality, item, amount)`
│  │
│  └─ stored/                                         **Internal stored domain entities used by the system**
│     ├─ Municipality.cs                              `Municipality with aggregated UIFW values`
│     ├─ Nation.cs                                    `National roll-up entity aggregating provinces`
│     └─ Province.cs                                  `Province entity aggregating municipalities`
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
0. Build source URL and file path from source.json
1. Download raw CSV/JSON (**CsvSourceDownloader**/**JsonSourceDownloader**)
2. Load and deserialize CSV/JSON into strongly typed rows (**CsvLoader**/**CsvLoader**)
3. Aggregate?
4. `Map to source objects/ Build objects` (**UifwSourceMapper**/**MunicipalityBuilder**)
5. Write JSON (**data/stored/*.json**)
6. Commit to GitHub (**Nightly via GitHub Actions later**)


===============================================================
                    Basic Logic (Current)
===============================================================
1. Load source configuration from `sources.json`
2. Build Treasury API URL from config
3. Download UIFW facts JSON (JsonSourceDownloader)
4. Deserialize facts into strongly typed rows (JsonLoader)
5. Aggregate facts into Municipality entities (MunicipalityBuilder)
6. (Planned) Aggregate municipalities into provinces and nation
7. (Planned) Export structured JSON per financial year


===============================================================
                     Data Entity Structure
===============================================================
This project aggregates official public finance and population data into a hierarchical structure that mirrors South Africa’s system of government.
All data is generated programmatically from source CSV files and stored as JSON (one file per financial year).

# Nation
Represents South Africa as a whole.
# Properties:
- `string` **name** – Country name (e.g. South Africa)
- `string` **code** – National code (currently ZA)
- `int` **population** – Total national population
- `decimal` **unauthorised** – Total reported unauthorised expenditure
- `decimal` **irregular** – Total reported irregular expenditure
- `decimal` **fruitless** – Total reported fruitless & wasteful expenditure
- `decimal` **uifw** – Sum of unauthorised + irregular + fruitless expenditure
- `string` **governingParty** – Governing party/coalition at financial year end
- `List<Province>` **provinces** – List of provincial entities

# Province
Represents a South African province.
# Properties:
- `string` **name** – Province name (e.g. Gauteng)
- `string` **code** – Province code (e.g. GP)
- `int` **population** – Total provincial population
- `decimal` **unauthorised** – Total reported unauthorised expenditure
- `decimal` **irregular** – Total reported irregular expenditure
- `decimal` **fruitless** – Total reported fruitless & wasteful expenditure
- `decimal` **uifw** – Sum of unauthorised + irregular + fruitless expenditure
- `string` **governingParty** – Governing party/coalition at financial year end
- `List<Municipality>` **municipalities** – List of municipal entities

# Municipality
Represents a local or metropolitan municipality.
# Properties:
- `string` **name** – Municipality name (e.g. City of Johannesburg)
- `string` **code** – Municipality code (e.g. JHB, BUF)
- `string` **type** – Municipality type (Metro / Local / District)
- `int` **population** – Total municipal population
- `decimal` **unauthorised** – Reported unauthorised expenditure
- `decimal` **irregular** – Reported irregular expenditure
- `decimal` **fruitless** – Reported fruitless & wasteful expenditure
- `decimal` **uifw** – Sum of unauthorised + irregular + fruitless expenditure
- `string` **governingParty** – Governing party/coalition at financial year end

# Notes
- All monetary values are reported per financial year
- No new data is created; all values are derived from official public sources
- Aggregation flows municipality → province → national
- Raw source files are stored separately from generated outputs
