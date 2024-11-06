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
/// For blog front matter that has Tags
/// </summary>
public interface IBlogFrontMatter : IFrontMatter
{
    /// <summary>
    ///     Tags for the blog post. When no tags are specified, implement empty list.
    ///     If you have a different name for tags, or tags in complex objects, expose tags as a list of strings here.
    ///     Useful for generating tag pages.
    /// </summary>
     List<string> Tags { get; set; }
}

/// <summary>
///     Showcase of a IBlogFrontMatter implementation. If you have a different front matter format, implement your own class.
/// </summary>
public class BlogFrontMatter : IBlogFrontMatter
{
    /// <summary>
    ///     Title of the blog post.
    /// </summary>
    public string Title { get; set; } = "Empty title";
    /// <summary>
    ///     Lead or description of the blog post.
    /// </summary>
    public string Lead { get; set; } = "";
    /// <summary>
    ///     Date of publishing the blog post.
    /// </summary>
    public DateTime Published { get; set; } = DateTime.Now;

    /// <summary>
    ///     Authors of the blog post.
    /// </summary>
    public List<Author> Authors { get; set; } = [];




    /// <inheritdoc />
    public bool IsDraft { get; set; }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    public AdditionalInfo? AdditionalInfo => new() { LastMod = Published };


    /// <inheritdoc />
    public List<string> Tags { get; set; } = [];
}

/// <summary>
///     Author of a blog post.
/// </summary>
public class Author
{
    /// <summary>
    ///     Name of the author.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    ///     GitHub username of the author.
    /// </summary>
    public string? GitHubUserName { get; set; }
    /// <summary>
    ///     X username of the author.
    /// </summary>
    public string? XUserName { get; set; }
}

/// <summary>
///     Keeps metadata and html content of a post (parsed from md).
/// </summary>
/// <typeparam name="TFrontMatter"></typeparam>
public class Post<TFrontMatter>
    where TFrontMatter : class
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
