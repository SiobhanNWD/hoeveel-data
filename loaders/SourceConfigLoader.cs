using System.Text.Json;
using Hoeveel.Aggregator.Models.Config;

namespace Hoeveel.Aggregator.Loaders;

public static class SourceConfigLoader
{
    // Loads source configuration values (URLs, file paths, formats) from a JSON file
    // Input: Default path is "config/sources.json" but can be overridden if needed
    public static SourceConfig Load(string path = "config/sources.json")
    {
        var json = File.ReadAllText(path);                          // Read the entire JSON config file into a string
        return JsonSerializer.Deserialize<SourceConfig>(json)!;     // Deserialize the JSON string into a SourceConfig object (The null-forgiving operator (!) is safe here because: this file is required for the app to run so we want it to throw loudly)
    }
}

//Example use in Program.cs
//  using Hoeveel.Aggregator.Loaders;
//  var config = SourceConfigLoader.Load();
//  config.jsonFieldName (eg config.UifwSourceUrl)