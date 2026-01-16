namespace BlazorStatic;


/// <summary>
///     Interface for front matter. FrontMatter is the metadata of a post.
/// </summary>
public interface IFrontMatter
{
    /// <summary>
    ///     If true, the blog post will not be generated.
    /// </summary>
    bool IsDraft => false;

    /// <summary>
    ///     Optional data for configuring certain parts of the generation process.
    ///     Currently used for passing the Date of creation to site.xml.
    ///     (generation process isn't aware of IFrontMatter implementation)
    /// </summary>
    AdditionalInfo? AdditionalInfo => null;

}

/// <summary>
/// If your FrontMatter uses Tags you need to implement this interface to process the tags.
/// </summary>
public interface IFrontMatterWithTags
{
    /// <summary>
    ///     Tags for the post.
    ///     If you have a different name for tags, or tags in complex objects, expose tags as a list of strings here.
    /// This is just front matter, tags will be process with proper encoder.
    /// </summary>
    List<string> Tags { get; set; }
}



/// <summary>
/// Tags for BlazorStatic, contains Name (original string from FrontMatter)
/// and EncodedName - used for urls and file names
/// </summary>
public class Tag
{
    /// <summary>
    /// Original string from FrontMatter. Can contain any characters.
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Encoded name, used for urls and file names
    /// </summary>
    public required string EncodedName { get; set; }
}

/// <summary>
///     Keeps metadata and html content of a post (parsed from md).
/// </summary>
/// <typeparam name="TFrontMatter"></typeparam>
public class Post<TFrontMatter>
    where TFrontMatter : class, IFrontMatter, new()
{

    /// <summary>
    ///     Front matter of the post.
    /// </summary>
    public required TFrontMatter FrontMatter { get; set; }
    /// <summary>
    ///     The url where the post will be generated.
    ///     Processed from the file path (Content/Blog/subfolder/post-in-subfolder.md => blog/subfolder/post-in-subfolder).
    ///     Used as url param e.g.: "blog/{Url}".
    /// </summary>
    public required string Url { get; set; }
    /// <summary>
    ///     HTML content of the post. Parsed from md. Without front matter part.
    /// </summary>
    public required string HtmlContent { get; set; }

    /// <summary>
    /// Tag for the post.
    /// Works only when FrontMatter implements IFrontMatterWithTags
    /// </summary>
    public List<Tag> Tags { get; set; } = [];


}

/// <summary>
///     Additional AdditionalInfo related to the page. This info is typically not bounded to FrontMatter, but rather
///     "computed" additionaly.
///     Currently, it is used to pass LastMod to the node in xml sitemap
/// </summary>
public class AdditionalInfo
{
    /// <summary>
    ///     The date of last modification of the page.
    /// </summary>
    public DateTime? LastMod { get; init; }
}

/// <summary>
///     Class for keeping the content to copy properties together.
/// </summary>
/// <param name="SourcePath"></param>
/// <param name="TargetPath"></param>
public record ContentToCopy(string SourcePath, string TargetPath);

/// <summary>
///     Class for keeping the page to generate properties together.
/// </summary>
/// <param name="Url"></param>
/// <param name="OutputFile"></param>
/// <param name="AdditionalInfo">Additional file properties.</param>
public record PageToGenerate(string Url, string OutputFile, AdditionalInfo? AdditionalInfo = null);
