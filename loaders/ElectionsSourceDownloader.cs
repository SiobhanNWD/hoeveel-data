using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hoeveel.Aggregator.Models.Raw;

namespace Hoeveel.Aggregator.Loaders;

/// <summary>
/// ElectionsSourceDownloader handles downloading municipal election results from the IEC (Independent Electoral Commission)
/// for all provinces and consolidating them into a single CSV file.
/// 
/// NOTE: This script is designed to be flexible and work with the IEC elections portal.
/// Once you determine the actual download URL pattern (especially for province selection),
/// update the BuildProvinceUrl() method with the correct endpoint.
/// 
/// Currently supports: 2021 Municipal Elections (matching 2022 governance data)
/// </summary>
public static class ElectionsSourceDownloader
{
    // South African provinces
    // Province codes as provided by user - based on actual IEC download links
    // CHANGE LOG: 2025-02-09 Updated with actual province codes from user
    private static readonly string[] Provinces = 
    { 
        "FS",  // Free State (tested working)
        "EC",  // Eastern Cape
        "GP",  // Gauteng
        "KN",  // KwaZulu-Natal (KN per user's links)
        "NP",  // Limpopo region (NP per user's links)
        "MP",  // Mpumalanga
        "NW",  // North West
        "NC",  // Northern Cape
        "WP"   // Western Cape (WP per user's links)
    };

    /// <summary>
    /// Downloads election results for all provinces and consolidates them into a single CSV file.
    /// </summary>
    /// <param name="outputPath">The file path where the consolidated CSV will be saved</param>
    /// <param name="electionYear">The election year (default: 2021)</param>
    public static async Task DownloadAndConsolidateAsync(string outputPath, int electionYear = 2021)
    {
        Console.WriteLine($"Starting election data download for {electionYear}...");
        
        var allResults = new List<ElectionsRow>();
        
        foreach (var province in Provinces)
        {
            try
            {
                Console.WriteLine($"Downloading {province} election results...");
                var provinceData = await DownloadProvinceAsync(province, electionYear);
                
                if (provinceData.Count > 0)
                {
                    allResults.AddRange(provinceData);
                    Console.WriteLine($"  ✓ {province}: {provinceData.Count} records");
                }
                else
                {
                    Console.WriteLine($"  ⊘ {province}: No data returned");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error downloading {province}: {ex.Message}");
            }
        }
        
        // Ensure output directory exists
        Hoeveel.Aggregator.Utils.FileHelpers.EnsureDirectoryForFile(outputPath);
        
        // Write consolidated CSV to disk
        WriteConsolidatedCsv(outputPath, allResults);
        Console.WriteLine($"Consolidated election data written to {outputPath} ({allResults.Count} total records)");
    }

    /// <summary>
    /// Downloads election results for a specific province.
    /// 
    /// TODO: CUSTOMIZE THIS METHOD
    /// The IEC portal requires selecting a province via form submission or query parameters.
    /// Once you determine the correct URL format, update BuildProvinceUrl() below.
    /// 
    /// Examples of what the URL might look like:
    ///   - https://results.elections.org.za/api/municipality/2021?province=GP
    ///   - https://results.elections.org.za/download/csv/2021/GP
    ///   - https://results.elections.org.za/reports/municipal/2021/province/{GP}/results.csv
    /// </summary>
    private static async Task<List<ElectionsRow>> DownloadProvinceAsync(string provinceCode, int electionYear)
    {
        var url = BuildProvinceUrl(provinceCode, electionYear);
        var results = new List<ElectionsRow>();
        
        using var httpClient = new HttpClient();
        try
        {
            var csvContent = await httpClient.GetStringAsync(url);
            results = ParseCsvContent(csvContent, provinceCode);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request failed for {provinceCode}: {ex.Message}");
        }
        
        return results;
    }

    /// <summary>
    /// Builds the download URL for a specific province.
    /// 
    /// Pattern discovered: https://results.elections.org.za/home/LGEPublicReports/{reportId}/Downloadable%20Party%20Results/{provinceCode}.csv
    /// Report ID 1091 corresponds to 2021 Municipal Elections
    /// </summary>
    private static string BuildProvinceUrl(string provinceCode, int electionYear)
    {
        // IEC LGE (Local Government Elections) public reports endpoint
        // Report ID mapping:
        // 1091 = 2021 Municipal Elections
        int reportId = electionYear switch
        {
            2021 => 1091,
            2016 => 1091, // TODO: Verify correct report ID for 2016
            2011 => 1091, // TODO: Verify correct report ID for 2011
            _ => 1091     // Default to 2021
        };

        return $"https://results.elections.org.za/home/LGEPublicReports/{reportId}/Downloadable%20Party%20Results/{provinceCode}.csv";
    }

    /// <summary>
    /// Parses CSV content into ElectionsRow objects.
    /// 
    /// IMPORTANT: The actual IEC CSV column order matters since we're using positional parsing.
    /// Expected CSV format from IEC (based on typical election result format):
    /// Column 0: Municipality Code (demarcation code)
    /// Column 1: Municipality Name
    /// Column 2: Party Name
    /// Column 3: Votes
    /// Column 4: Vote Percentage
    /// 
    /// If the actual CSV column order differs, adjust the index positions below.
    /// The debug output will show you the actual headers for verification.
    /// </summary>
    private static List<ElectionsRow> ParseCsvContent(string csvContent, string provinceCode)
    {
        var results = new List<ElectionsRow>();
        
        try
        {
            var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            if (lines.Length < 2)
            {
                Console.WriteLine($"  Warning: {provinceCode} CSV has less than 2 lines");
                return results;
            }
            
            // Parse header line
            var headers = lines[0].Split(',');
            Console.WriteLine($"  CSV Headers for {provinceCode}: {string.Join(", ", headers)}");
            
            // Find column indices (try to auto-detect) - updated for actual IEC CSV structure
            int municipalityIdx = FindColumnIndex(headers, "Municipality");
            int partyNameIdx = FindColumnIndex(headers, "Party Name", "PartyName", "Party");
            int votesIdx = FindColumnIndex(headers, "TotalValidVotes", "Votes", "Vote");
            
            // If we couldn't find critical columns, log a warning
            if (municipalityIdx < 0 || partyNameIdx < 0)
            {
                Console.WriteLine($"  ⚠️  Warning: Could not auto-detect all columns for {provinceCode}");
                Console.WriteLine($"      Municipality: {(municipalityIdx >= 0 ? "Found" : "NOT FOUND")}");
                Console.WriteLine($"      Party Name: {(partyNameIdx >= 0 ? "Found" : "NOT FOUND")}");
            }
            
            // Parse data rows
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                
                try
                {
                    var columns = lines[i].Split(',');
                    
                    if (municipalityIdx < 0 || partyNameIdx < 0) continue; // Skip if critical columns not found
                    
                    var municipality = municipalityIdx < columns.Length ? columns[municipalityIdx].Trim() : "";
                    var partyName = partyNameIdx < columns.Length ? columns[partyNameIdx].Trim() : "";
                    var votesStr = votesIdx >= 0 && votesIdx < columns.Length ? columns[votesIdx].Trim() : "0";
                    
                    int.TryParse(votesStr, out var votes);
                    
                    // Extract municipality code from "FS161 - Letsemeng" format
                    var municipalityCode = ExtractMunicipalityCode(municipality);
                    
                    if (!string.IsNullOrWhiteSpace(municipalityCode) && !string.IsNullOrWhiteSpace(partyName))
                    {
                        var row = new ElectionsRow
                        {
                            MunicipalityCode = municipalityCode,
                            MunicipalityName = municipality,
                            ProvinceCode = provinceCode,
                            PartyName = partyName,
                            Votes = votes,
                            VotePercentage = 0m // Not provided in IEC CSV
                        };
                        
                        results.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Warning: Error parsing row {i} in {provinceCode}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error reading CSV content for {provinceCode}: {ex.Message}");
        }
        
        return results;
    }

    /// <summary>
    /// Helper method to find a column index by searching through possible column names.
    /// Returns -1 if none of the names are found.
    /// </summary>
    private static int FindColumnIndex(string[] headers, params string[] possibleNames)
    {
        foreach (var possibleName in possibleNames)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Trim().Equals(possibleName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
        }
        return -1;
    }
    /// <summary>
    /// Extracts municipality code from IEC format like "FS161 - Letsemeng" -> "FS161"
    /// </summary>
    private static string ExtractMunicipalityCode(string municipalityString)
    {
        if (string.IsNullOrEmpty(municipalityString))
            return "";
        
        // Format is typically "XXnnn - MunicipalityName" where XXnnn is the code
        var parts = municipalityString.Split(new[] { " - " }, StringSplitOptions.None);
        return parts.Length > 0 ? parts[0].Trim() : municipalityString;
    }
    /// <summary>
    /// Writes the consolidated election results to a CSV file.
    /// </summary>
    private static void WriteConsolidatedCsv(string outputPath, List<ElectionsRow> results)
    {
        try
        {
            string directory = Path.GetDirectoryName(outputPath) ?? "";
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                writer.WriteLine("MunicipalityCode,MunicipalityName,ProvinceCode,PartyName,Votes,VotePercentage");
                
                foreach (var row in results)
                {
                    writer.WriteLine($"{EscapeCsvField(row.MunicipalityCode)}," +
                                   $"{EscapeCsvField(row.MunicipalityName)}," +
                                   $"{EscapeCsvField(row.ProvinceCode)}," +
                                   $"{EscapeCsvField(row.PartyName)}," +
                                   $"{row.Votes}," +
                                   $"{row.VotePercentage}");
                }
                
                writer.Flush();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing consolidated CSV: {ex.Message}");
        }
    }

    /// <summary>
    /// Escapes CSV fields to handle commas and quotes in data.
    /// </summary>
    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return "";
        
        // If field contains comma, newline, or quote, wrap in quotes and escape quotes
        if (field.Contains(",") || field.Contains("\n") || field.Contains("\""))
        {
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }
        
        return field;
    }
}
