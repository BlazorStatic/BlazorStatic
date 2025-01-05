---
title: "1.0.0-beta.14 - Enhanced Tag Support, New blazorStatic.net Domain, Cloudflare support and more"
lead: "The beta.14 release brings major updates to BlazorStatic, with enhanced tag encoding, reorganized options, and a dedicated new GitHub organization and domain."
published: 2025-01-05
tags: [release, tag encoder, blazorstatic.net , BlazorStatic organization]
authors:
  - name: "Jan Tesař"
    gitHubUserName: "tesar-tech"
    xUserName: "tesar_tech"
  - name: "Patrick T Coakley"
    gitHubUserName: "patricktcoakley"
---

## Changes and Breaking Changes

- In `BlazorStaticContentOptions`, tag-related options have been moved to a new class, `TagsOptions`:

    ```csharp
    opt.AddTagPagesFromPosts = true // old
    opt.Tags.AddTagPagesFromPosts = true // new
    ```

- **Tags now work with encoding**, ensuring URLs function correctly with tags like `c#`, `something with spaces`, etc.
    - A new option has been added: `opt.Tags.TagEncodeFunc`, which defaults to `WebUtility.UrlEncode`.
    - A new `Tag` class has been introduced with properties `Name` (the plain string as presented in front matter) and `EncodedName` (used in URLs and filenames).
    - Tags are now a property within the `Post` class, with `BlazorStaticContentService` handling encoding. Use this property instead of `Tags` in `FrontMatter`.

- Tags are no longer enforced through `IFrontMatter`. To use tags, your `FrontMatter` class must implement `IFrontMatterWithTags`.
    - `BlazorStatic` will use the `List<string> Tags` in your `FrontMatter` and encode them accordingly. Each `Post` now contains a `List<Tag>`.
    - Use `Post.Tags` instead of `Post.FrontMatter.Tags`, as the former provides both `Name` and `EncodedName` properties.

- To access all unique tags, use the `BlazorStaticContentService.AllTags` property.


## New Domain and GitHub Organization

BlazorStatic has moved to its own GitHub organization. The repository links are now:

- [github.com/BlazorStatic/BlazorStatic](https://github.com/BlazorStatic/BlazorStatic) (instead of the previous user-based URL)

The project homepage also has its own second-level domain:

- [BlazorStatic.net](https://blazorstatic.net) — the .net domain is a fitting choice for a .NET-based project.

## Cloudflare Pages support

Check the result on https://blazorstatic.pages.dev/ and the guide [here](docs/deployment).

## Readme.md updates

Now it explains better what is BlazorStatic about.

## Feedback

Share your feedback by [creating an issue](https://github.com/BlazorStatic/BlazorStatic/issues/new) or join the conversation on the [Discord server](https://discord.gg/DsAXsMuEbx).

