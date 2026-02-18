using System.Text.Json;
using System.Text.Json.Serialization;
using Hoeveel.Aggregator.Models.Stored;
using Hoeveel.Aggregator.Models.Config;   // ExportOptionsConfig

namespace Hoeveel.Aggregator.Exporters
{
    public static class JsonExportService
    {
        // ================== EXPORT NATION TO JSON ==================
        // Serializes the fully built Nation object (including Provinces and Municipalities)
        // and writes it to disk in formatted camelCase JSON structure
        public static void ExportNation(string outputPath, Nation nation, JsonSettingsConfig settings)
        {
            // 1. Guard clauses to prevent invalid input
            if (nation == null)
                throw new ArgumentNullException(nameof(nation), "Nation cannot be null.");

            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentNullException(nameof(outputPath), "Output path cannot be null or empty.");

            // 2. Configure JSON serializer options
            var options = new JsonSerializerOptions
            {
                WriteIndented = settings.WriteIndented,             // Makes the JSON human-readable
                PropertyNamingPolicy = settings.UseCamelCase        // Converts C# PascalCase → camelCase
                    ? JsonNamingPolicy.CamelCase 
                    : null,
                DefaultIgnoreCondition = settings.IgnoreNulls       // If property value is null, it will be omitted from the JSON output
                    ? JsonIgnoreCondition.WhenWritingNull
                    : JsonIgnoreCondition.Never
            };

            // 3. Serialize Nation object (includes nested Provinces & Municipalities)
            var json = JsonSerializer.Serialize(nation, options);       //converts the Nation object into a JSON string using the specified options

            // 4. Ensure the output directory exists before writing
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))                       // If directory is not null or empty, create it
                Directory.CreateDirectory(directory);

            // 5. Write JSON string to file
            File.WriteAllText(outputPath, json);        // Creates a file and writes the JSON string to it. If the file already exists, it will be overwritten with the new content.
        }

        public static void ExportNation(string outputPath, Nation nation, JsonExportInfo exportInfo,JsonSettingsConfig settings)
        {
            if (nation == null)
                throw new ArgumentNullException(nameof(nation), "Nation cannot be null.");

            if (exportInfo == null)
                throw new ArgumentNullException(nameof(exportInfo), "Export info cannot be null.");

            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentNullException(nameof(outputPath), "Output path cannot be null or empty.");

            var options = new JsonSerializerOptions
            {
                WriteIndented = settings.WriteIndented,
                PropertyNamingPolicy = settings.UseCamelCase
                    ? JsonNamingPolicy.CamelCase
                    : null,
                DefaultIgnoreCondition = settings.IgnoreNulls
                    ? JsonIgnoreCondition.WhenWritingNull
                    : JsonIgnoreCondition.Never
            };

            // 🔹 Wrap Nation + ExportInfo
            var exportPackage = new ExportPackage
            {
                ExportInfo = exportInfo,
                Nation = nation
            };

            var json = JsonSerializer.Serialize(exportPackage, options);

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(outputPath, json);
        }
    }

    public class ExportPackage
    {
        public required JsonExportInfo ExportInfo { get; set; }
        public required Nation Nation { get; set; }
    }
}

    


