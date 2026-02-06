using System.Text.Json;

namespace Hoeveel.Aggregator.Loaders;

public static class JsonLoader
{
    public static T Load<T>(string path)        // Loads JSON from disk and deserialises into the requested type T
    {
        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<T>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true      //underscores won't risk breaking the code
            }
        )!;
    }
}
