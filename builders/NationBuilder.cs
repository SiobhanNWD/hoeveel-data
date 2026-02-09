using Hoeveel.Aggregator.Models.Stored;

namespace Hoeveel.Aggregator.Builders;

public static class NationBuilder
{
    // Takes a list of Province objects and assembles a Nation
    public static Nation BuildFromProvinces(List<Province> provinces)
    {
        return new Nation
        {
            Provinces = provinces        // Nation totals are computed automatically via properties, name is hardcoded
        };
    }
}
