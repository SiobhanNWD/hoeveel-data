# hoeveel-data

# Please ensure you have .NET 8 SDK installed

# Required Exstension (Publisher):
- .NET Install Tool (Microsoft)
- C/C++ (Microsoft)
- C# (Microsoft)
- C# Dev Kit (Microsoft)
- Edit CSV (janisdd)
- GitHub Pull Requests (GitHub)
- JSON Tools (Erik Lynd)
- REST Client (Huachao Mao)

===============================================================
                        File Structure
===============================================================
//config files used so that they dont have to be hardcoded later

===============================================================
                     Data Entity Structure
===============================================================
This project aggregates official public finance and population data into a hierarchical structure that mirrors South Africa’s system of government.
All data is generated programmatically from source CSV files and stored as JSON (one file per financial year).

# National
Represents South Africa as a whole.
# Properties:
- `string` **name** – Country name (e.g. South Africa)
- `string` **code** – National code (currently ZA)
- `int` **population** – Total national population
- `decimal` **unauthorized** – Total reported unauthorised expenditure
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
- `decimal` **unauthorized** – Total reported unauthorised expenditure
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
- `decimal` **unauthorized** – Reported unauthorised expenditure
- `decimal` **irregular** – Reported irregular expenditure
- `decimal` **fruitless** – Reported fruitless & wasteful expenditure
- `decimal` **uifw** – Sum of unauthorised + irregular + fruitless expenditure
- `string` **governingParty** – Governing party/coalition at financial year end

# Notes
- All monetary values are reported per financial year
- No new data is created; all values are derived from official public sources
- Aggregation flows municipality → province → national
- Raw source files are stored separately from generated outputs


===============================================================
                    Basic Aggregation Logic
===============================================================
The system aggregates financial and population data bottom-up.

1. Load raw CSV data for municipalities
2. Parse and validate financial and population values
3. Aggregate municipality totals (population, expenditure, UIFW)
4. Group municipalities by province and sum provincial totals
5. Aggregate all provinces to calculate national totals
6. Compute UIFW as unauthorised + irregular + fruitless
7. Determine governing party at each level from aggregated votes
8. Output aggregated results as structured JSON

===============================================================
                         Basic Logic
===============================================================
1. Download raw csv (**UifwSourceDownloader**)
2. Load CSV into rows (**CsvLoader**)
3. Map to source objects (**UifwSourceMapper**)
4. Aggregate (**UifwAggregator**)
5. Write JSON (**data/stored/*.json**)
6. Commit to GitHub (**Nightly via GitHub Actions later**)