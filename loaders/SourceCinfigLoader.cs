using System.Text.Json;
using Hoeveel.Aggregator.Models.Config;

namespace Hoeveel.Aggregator.Loaders;

public static class SourceConfigLoader
{
    public static SourceConfig Load(string path = "config/sources.json")
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SourceConfig>(json)!;
    }
}


//Example use in Program.cs
//  using Hoeveel.Aggregator.Loaders;
//  var config = SourceConfigLoader.Load();
//  config.jsonFieldName (eg config.UifwSourceUrl)