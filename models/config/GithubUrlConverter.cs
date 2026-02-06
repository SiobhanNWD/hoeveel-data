namespace Hoeveel.Aggregator.Models.Config;

/// <summary>
/// Utility for converting GitHub "blob" page URLs into raw content URLs.
///
/// Example:
/// https://github.com/owner/repo/blob/branch/path/to/file.csv
///   -> https://raw.githubusercontent.com/owner/repo/branch/path/to/file.csv
///
/// Note: This helper only rewrites GitHub URLs that contain the
/// "blob" segment. Non-GitHub or already-raw URLs are returned unchanged.
/// Use this explicitly from caller code when you want automatic GitHub -> raw conversion.
/// </summary>
public static class GithubUrlConverter
{
    public static string NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;

        try
        {
            var uri = new Uri(url);
            if (!uri.Host.EndsWith("github.com", StringComparison.OrdinalIgnoreCase)) return url;

            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            // Expecting: owner / repo / blob / branch / path / ...
            if (segments.Length >= 4 && segments[2].Equals("blob", StringComparison.OrdinalIgnoreCase))
            {
                var owner = segments[0];
                var repo = segments[1];
                var branch = segments[3];
                var pathParts = segments.Length > 4 ? segments[4..] : Array.Empty<string>();
                var rawPath = string.Join('/', pathParts);

                return $"https://raw.githubusercontent.com/{owner}/{repo}/{branch}/{rawPath}";
            }
        }
        catch
        {
            // fall through and return original URL on any parsing error
        }

        return url;
    }
}

/*
Usage:

This helper converts GitHub page ("blob") URLs to the corresponding raw content URL.
Call it explicitly before passing URLs to downloaders when you expect users to provide
GitHub repository links (for example from config/sources.json).

Example:
    var blob = "https://github.com/afrith/census-2022-muni-stats/blob/main/person-indicators-province.csv";
    var raw = GithubUrlConverter.NormalizeUrl(blob);
    // raw == "https://raw.githubusercontent.com/afrith/census-2022-muni-stats/main/person-indicators-province.csv"
    await Hoeveel.Aggregator.Loaders.CsvSourceDownloader.DownloadAsync(raw, "data/raw/person-indicators-province.csv");

Notes:
- The function returns the original URL unchanged if it's not a GitHub blob URL.
- It does not perform any network requests; it only rewrites the URL string.
*/
