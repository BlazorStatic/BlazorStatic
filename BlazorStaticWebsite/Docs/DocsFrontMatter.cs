namespace BlazorStaticWebsite.Docs;

using BlazorStatic;
using BlazorStatic.Services;

public class DocsFrontMatter:IFrontMatter
{

}

public class DocsStaticContentOptions : IBlazorStaticContentOptions<DocsFrontMatter>
{
    public string ContentPath { get; set; } = Path.Combine("Content", "Docs");
    public string? MediaFolderRelativeToContentPath { get; set; } = "media";
    public string PostFilePattern { get; set; } = ".md";
    public List<Post<DocsFrontMatter>> Posts { get; } = [];
    public string PageUrl { get; set; } = "docs";
    public Action<BlazorStaticService>? AfterContentParsedAndAddedAction { get; set; }

}

