// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
namespace BlazorStatic.Blog;

using System.Net;
using Services;

public class BlogOptions<TBlogFrontMatter> : IBlazorStaticContentOptions<TBlogFrontMatter>
where TBlogFrontMatter: class, IBlogFrontMatter
{
    public BlogOptions()
    {
        AfterContentParsedAndAddedAction = blazorStaticService => {

            //add tags pages
            if(TagsPageUrl is null) return;
            // blazorStaticService.Options.PagesToGenerate.Add(new($"{options.TagsPageUrl}", Path.Combine(options.TagsPageUrl, "index.html")));
            foreach(var tag in Posts.SelectMany(x => x.FrontMatter.Tags).Distinct())//gather all unique tags from all posts
            {
                var encodedTag = TagEncodeFunc(tag);
                blazorStaticService.Options.PagesToGenerate.Add(new PageToGenerate($"{TagsPageUrl}/{encodedTag}",
                Path.Combine(TagsPageUrl, $"{encodedTag}.html")));
            }
        };
    }


    /// <summary>
    ///     Should correspond to @page "/tags" (here in relative path: "tags")
    ///     Useful for avoiding magic strings in .razor files
    ///     If null, no tag pages will be generated
    /// </summary>
    public string? TagsPageUrl { get; set; } = "tags";

    /// <summary>
    /// Func to convert tag string to file-name/url.
    /// You might want to change this if you don't like non-alphanumerical chars in your url (like tags/.net%2FC%23)
    /// The default is WebUtility.UrlEncode, which makes changes, like:
    /// "nice tag" -> "nice+tag", "ci/cd" -> "ci%2Fcd", ".net/C# " -> ".net%2FC%23".
    /// Also don't forget to use the same encoder while creating tag links
    /// </summary>
    public Func<string,string> TagEncodeFunc { get; set; } = WebUtility.UrlEncode;


    /// <inheritdoc />
    public string ContentPath { get; set; } = Path.Combine("Content", "Blog");
    /// <inheritdoc />
    public string? MediaFolderRelativeToContentPath { get; set; } = "media";
    /// <inheritdoc />
    public string PostFilePattern { get; set; } = "*.md";

    /// <inheritdoc />
    public List<Post<TBlogFrontMatter>> Posts { get; } = [];
    /// <inheritdoc />
    public string PageUrl { get; set; } = "blog";
    /// <inheritdoc />
    public Action<BlazorStaticService>? AfterContentParsedAndAddedAction { get; set; }
}


