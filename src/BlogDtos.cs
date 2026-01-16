// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace BlazorStatic;

/// <summary>
/// For blog front matter that has Tags
/// </summary>
/// <summary>
///     Showcase of a IFrontMatter implementation. If you have a different front matter format, implement your own class.
/// </summary>
public class BlogFrontMatter : IFrontMatter, IFrontMatterWithTags
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
    ///     Date of the most recent update to the blog post, if any.
    /// </summary>
    public DateTime? Updated { get; set; }

    /// <summary>
    ///     Authors of the blog post.
    /// </summary>
    public List<Author> Authors { get; set; } = [];

    /// <inheritdoc />
    public bool IsDraft { get; set; }


    /// <summary>
    /// Tags - yeah.
    /// </summary>
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
