using System;
using System.Collections.Generic;
using System.Linq;
using Hoeveel.Aggregator.Models.Raw;

namespace Hoeveel.Aggregator.Mappers;

/// <summary>
/// ElectionsMapper processes raw election data to determine the governing party for each municipality.
/// 
/// The governing party is determined as the party with the highest vote percentage in each municipality.
/// If multiple parties have the same percentage, the first encountered is selected.
/// 
/// CHANGE LOG:
/// - Created: 2025-02-09 To map 2021 election results to governing party data
/// </summary>
public static class ElectionsMapper
{
    /// <summary>
    /// Determines the governing party for each municipality based on election results.
    /// 
    /// Returns a dictionary where:
    /// - Key: Municipality code (demarcation code)
    /// - Value: The name of the party with the highest vote percentage
    /// </summary>
    public static Dictionary<string, string> GetGoverningPartyByMunicipality(List<ElectionsRow> electionRows)
    {
        if (electionRows == null || electionRows.Count == 0)
        {
            Console.WriteLine("Warning: No election data provided to ElectionsMapper");
            return new Dictionary<string, string>();
        }

        var governingParties = new Dictionary<string, string>();

        // Group election data by municipality code
        var municipalityGroups = electionRows.GroupBy(e => e.MunicipalityCode);

        foreach (var group in municipalityGroups)
        {
            var municipalityCode = group.Key;
            
            // Find the party with the highest vote percentage
            var winningParty = group.OrderByDescending(e => e.VotePercentage)
                                      .FirstOrDefault();

            if (winningParty != null)
            {
                governingParties[municipalityCode] = winningParty.PartyName;

                // Debug: Log if this is helpful
                // Console.WriteLine($"{municipalityCode}: {winningParty.PartyName} ({winningParty.VotePercentage}%)");
            }
            else
            {
                Console.WriteLine($"Warning: No election data found for municipality {municipalityCode}");
            }
        }

        return governingParties;
    }

    /// <summary>
    /// Alternative method: Gets detailed winning party information including vote percentage
    /// Useful if you need more than just the party name
    /// </summary>
    public static Dictionary<string, ElectionsRow> GetDetailedGoverningPartyByMunicipality(List<ElectionsRow> electionRows)
    {
        if (electionRows == null || electionRows.Count == 0)
        {
            return new Dictionary<string, ElectionsRow>();
        }

        var governingParties = new Dictionary<string, ElectionsRow>();
        var municipalityGroups = electionRows.GroupBy(e => e.MunicipalityCode);

        foreach (var group in municipalityGroups)
        {
            var winningParty = group.OrderByDescending(e => e.VotePercentage)
                                      .FirstOrDefault();

            if (winningParty != null)
            {
                governingParties[group.Key] = winningParty;
            }
        }

        return governingParties;
    }
}
