---
title: "1.0.0-beta 16 - Islands of content, image checks, auto image path resolver, test coverage"
lead: "This release improves content organization, simplifies image handling, and adds the first set of automated tests."
published: 2025-11-15
tags: [release]
authors:
    - name: "Jan Tesař"
      gitHubUserName: "tesar-tech"
      xUserName: "tesar-tech"
---

## Breaking Changes

If your blog posts are in subfolders and use media paths that are **not relative to the file**, be aware that this might break.

The good news? Relative paths are now resolved correctly.\
So if your IDE displays images properly when working with markdown files, you can be confident that **BlazorStatic** will do the same.

## Posts in folders

I often start a new blog post by creating a Markdown file that includes a publish date in the front matter.\
I also put the date in the filename to sort posts chronologically. When I paste an image, a folder is created in the shared media directory named after the file.\
That means the publish date appears in multiple places: the filename, the front matter, and every image path.

It seems minor, but these small frictions add up. They discourage writing altogether.\
This update reduces that friction by simplifying the content structure.

Old structure:

```
Content/
  Blog/
      post1.md
      post2.md
      post3.md
    media/
      image.png
      image2.png
```

New structure:

```
Content/
  Blog/
    post1/
      index.md
      media/
        image.png
    post2/
      index.md
      media/
        image.png
    post3/
      index.md
```

In BlazorStatic, you can now choose your preferred folder structure or even mix both approaches.
The new "islands of content" approach does require _more content_ in the sense that each post now lives in its own folder, along with its media.

But surprisingly, I find it nicer to work with.
There's better separation of concerns: each post is self-contained, and nothing is scattered across unrelated folders.

But in the end — it's your choice now.

## Auto Image Resolver

The `MediaFolderRelativeToContentPath` path is no longer needed.
Previously, you had to manually set the folder where media for posts was stored-typically a `media` folder (which was the default).
Now, BlazorStatic automatically finds the media folder by scanning all image tags and resolving the correct paths.
For example, if a post is located at `Content/Blog/my-post/index.md` and contains an image reference like `![alt](media/image.png)`,
the actual image should be located at `Content/Blog/my-post/media/image.png`. BlazorStatic will automatically rewrite the image path accordingly.

It also removes the trailing `/index` from the final URLs. This results in cleaner, shorter URLs.
While this behavior is currently hardcoded in the library, it could be made optional if there's a good reason.

These changes eliminate several bootstrapping steps and make content organization simpler.

However, there is one potential **"security issue"**. It's in quotes because it only exists during development and disappears after the site is generated to static files.

The middleware now exposes the entire `ContentPath` directory under `/{ContentPath}` (typically `Content/Blog`).
This means raw content files are accessible during development. Again, this is only temporary and doesn't expose your source code.

If you see any issues with this approach, please share your feedback. There may be alternative solutions.
For now, using `app.UseStaticFiles` for the entire content path was the simplest approach.

---

Also, if removing the `MediaFolderRelativeToContentPath` option (which guaranteed its content would be copied) is a concern, you can still use the manual copy method:

```csharp
builder.Services.AddBlazorStaticService(opt => {
    opt.ContentToCopyToOutput.Add(new ContentToCopy("Content/Docs/media", "Content/Docs/media"));
});
```

## Image File Checker

The auto image resolver updates paths **only for relative images**, and **only if the image file actually exists**.
If an image is missing, a warning is logged to help you catch the issue during development.

![what a beautiful warning](media/20251111_001137.png)

## Test Coverage

Not exactly TDD, but this release includes the first batch of automated tests.  
As coverage increases, the structure will evolve accordingly.  
This is one reason why BlazorStatic is still in beta.  
Version 1.0 will ship with solid test coverage.

## Feedback

Share feedback by [creating an issue](https://github.com/BlazorStatic/BlazorStatic/issues/new)\
or join the [Discord server](https://discord.gg/DsAXsMuEbx).
