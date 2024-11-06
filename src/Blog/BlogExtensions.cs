// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
namespace BlazorStatic.Blog;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// BlazorStatic blog extensions for registering the services.
/// </summary>
public static class BlogExtensions
{
    /// <summary>
    /// Simplification of AddBlazorStaticContentService with BlogOptions:IBlazorStaticContentOptions
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions">Options to configure your blog</param>
    /// <typeparam name="TBlogFrontMatter">Front matter type. Use any type that matches your front matter and inherits from IBlogFrontMatter</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddBlogService<TBlogFrontMatter>(this IServiceCollection services,
        Action<BlogOptions<TBlogFrontMatter>>? configureOptions = null)
        where TBlogFrontMatter : class, IBlogFrontMatter, new()
    {
        services.AddBlazorStaticContentService<TBlogFrontMatter, BlogOptions<TBlogFrontMatter>>(configureOptions);
        services.AddSingleton<BlogService<TBlogFrontMatter>>();
        return services;
    }
}
