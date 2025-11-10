namespace BlazorStatic.Dtos;

/// <summary>
///   Result of parsing a markdown file.
/// </summary>
/// <typeparam name="TFrontMatter"></typeparam>
public class MarkdownParsingResult<TFrontMatter>
{
    /// <summary>
    /// Pancake recipe...
    /// No! It's the HTML content parsed from the markdown file.
    /// </summary>
    public required string HtmlContent { get; init; }

    /// <summary>
    ///  Media folders found in the markdown file.
    ///  e.g. images like ![alt](media/image.jpg) will have "Content/Blog/FolderWhereMdFileIs/media" folder here.
    /// Thas is just an example, the actual path depends on where the md file is located relative to ContentPath.
    /// </summary>
    public required HashSet<string> MediaFolders { get; init; }

    /// <summary>
    /// Extracted front matter from the markdown file.
    /// </summary>
    public required TFrontMatter FrontMatter { get; init; }
}
