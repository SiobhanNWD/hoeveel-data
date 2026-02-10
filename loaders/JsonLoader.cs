using System.Text.Json;

namespace Hoeveel.Aggregator.Loaders;

public static class JsonLoader
{
    //Input: path to JSON file on disk & type to deserialize to T
    //Output: deserialized object of type
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
