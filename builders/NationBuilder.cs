using Hoeveel.Aggregator.Models.Stored;
using Hoeveel.Aggregator.Models.Raw;   // ElectionsRow

namespace Hoeveel.Aggregator.Builders;

public static class NationBuilder
{
    // Takes a list of Province objects and assembles a Nation
    public static Nation BuildFromProvinces(List<Province> provinces, List<ElectionsRow>? electionsRows = null)
    {
        var nation = new Nation
        {
            Provinces = provinces        // Nation totals are computed automatically via properties, name is hardcoded
        };

        // Apply elections data if available
        if (electionsRows != null && electionsRows.Count > 0)
        {
            ApplyElectionsData(nation, electionsRows);
        }

        return nation;
    }

    // DONE Elections: GoverningParty = electionsProvRows.FirstOrDefault(p => p.ProvinceCode == group.Key)?.GoverningParty ?? "Unknown" Or something like that
    // ================== APPLY ELECTIONS DATA ==================
    // Aggregates election results to determine the governing party (highest vote count) at national level
    private static void ApplyElectionsData(Nation nation, List<ElectionsRow> electionsRows)
    {
        if (electionsRows == null || electionsRows.Count == 0)
            return;

        // Aggregate votes by party at national level
        var electionsByParty = electionsRows
            .GroupBy(e => e.PartyName)
            .Select(g => new
            {
                PartyName = g.Key,
                TotalVotes = g.Sum(e => e.Votes)
            })
            .ToList();

        // Determine the winning party at national level
        var winningParty = electionsByParty
            .OrderByDescending(e => e.TotalVotes)
            .FirstOrDefault();

        if (winningParty != null)
        {
            nation.GoverningParty = winningParty.PartyName;
        }
    }
}
