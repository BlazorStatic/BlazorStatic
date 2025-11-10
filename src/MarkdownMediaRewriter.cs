using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace BlazorStatic;

internal static partial class HtmlMediaRewriter
{
    static readonly Regex s_imageSrcRegex = ImageSrcRegex();

    public static (string Html, HashSet<string> Folders) RewriteImageSources(string html, string filePath, string? contentBaseSegment, ILogger logger)
    {
        var folders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var postDirectory = Path.GetDirectoryName(filePath) ?? string.Empty;
        if( string.IsNullOrWhiteSpace(contentBaseSegment) || string.IsNullOrEmpty(html) )
            return (html, folders);

        var rewritten = s_imageSrcRegex.Replace(html, match => {
            var before = match.Groups["before"].Value;
            var url = match.Groups["url"].Value;
            var after = match.Groups["after"].Value;

            if( IsExternal(url) ) return match.Value;

            var absolute = Path.GetFullPath(Path.Combine(postDirectory, url));
            if( !File.Exists(absolute) )
            {
                logger.LogWarning("Image file not found. Url '{Url}', resolved path '{AbsolutePath}'", url, absolute);
                return match.Value;
            }

            var newUrl = SliceFromSegment(absolute, contentBaseSegment);
            if( newUrl is null ) return match.Value;

            newUrl = newUrl.Replace(Path.DirectorySeparatorChar, '/');

            var folder = Path.GetDirectoryName(newUrl);
            if( !string.IsNullOrEmpty(folder) ) folders.Add(folder.Replace(Path.DirectorySeparatorChar, '/'));

            return $"{before}{newUrl}{after}";
        });

        return (rewritten, folders);
    }

    static bool IsExternal(string url)
    {
        if( string.IsNullOrWhiteSpace(url) ) return true;

        return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
               || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
               || url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
               || url.StartsWith('#');
    }

    static string? SliceFromSegment(string fullPath, string segmentPath)
    {
        if( string.IsNullOrWhiteSpace(segmentPath) ) return null;

        var normalizedFull = fullPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        var normalizedSegment = segmentPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        var idx = normalizedFull.IndexOf(normalizedSegment, StringComparison.Ordinal);
        return idx >= 0? normalizedFull[idx..]: null;
    }

    // matches: <img ... src="value"...> or <img ... src='value'...>, case-insensitive
    [GeneratedRegex("""
                    (?ix)
                    (?<before> <img \b [^>]* \bsrc \s* = \s* ["'] )
                    (?<url>    [^"']* )
                    (?<after>  ["'] [^>]* > )
                    """)]
    private static partial Regex ImageSrcRegex();
}
