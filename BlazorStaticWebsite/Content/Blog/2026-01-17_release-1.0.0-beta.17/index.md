---
title: "1.0.0-beta 17 release: .NET 10, FrontMatter with 'Updated' prop"
lead: "The newest dotnet is supported, default FrontMatter has been slightly updated, and a few changes have been made to BlazorStatic.Templates."
tags: [release]
authors:
    - name: "Jan Tesař"
      gitHubUserName: "tesar-tech"
      xUserName: "tesar-tech"
---

## Update to .net10

The entire BlazorStatic project - including [BlazorStatic.Templates](https://github.com/BlazorStatic/BlazorStatic.Templates/) and [BlazorStaticMinimalBlog](https://github.com/BlazorStatic/BlazorStaticMinimalBlog) - has been updated to .net10.  
There were no major issues during the upgrade process.

## Default FrontMatter update

The default `BlogFrontMatter` now supports an `Updated` property in addition to the existing `Published`.  
This allows you to indicate how current a post is, helping readers better understand its relevance.
While this was previously achievable through custom implementation, it is now included by default.

## Template update

Currently, there is just one template. You can use it after installing with:

```
dotnet new install BlazorStatic.Templates
```

Then create a new project with:

```
dotnet new BlazorStaticMinimalBlog -o MyBlazorStaticApp
```

The result will match what you see in the [BlazorStaticMinimalBlog repo](https://github.com/BlazorStatic/BlazorStaticMinimalBlog).

The template has been updated and is now much more convenient to use.  
With it, you can have your blog up and running in no time.

## Feedback

Share feedback by [creating an issue](https://github.com/BlazorStatic/BlazorStatic/issues/new) or join the [Discord server](https://discord.gg/DsAXsMuEbx).
