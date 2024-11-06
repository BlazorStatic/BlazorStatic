// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
namespace BlazorStatic.Blog;
using Services;

/// <summary>
/// Simplification of BlazorStaticContentService for Blog usage.
/// </summary>
/// <param name="options"></param>
/// <param name="helpers"></param>
/// <param name="blazorStaticService"></param>
/// <typeparam name="TBlogFrontMatter"></typeparam>
public class BlogService<TBlogFrontMatter>(BlogOptions<TBlogFrontMatter> options, BlazorStaticHelpers helpers, BlazorStaticService blazorStaticService)
    : BlazorStaticContentService<TBlogFrontMatter, BlogOptions<TBlogFrontMatter>>(options, helpers, blazorStaticService)
    where TBlogFrontMatter : class, IBlogFrontMatter, new();

