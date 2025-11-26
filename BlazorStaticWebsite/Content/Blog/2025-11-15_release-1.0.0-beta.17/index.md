---
title: "1.0.0-beta 17 realease, default front matter update"
lead: ""
tags: [realease, tips]
authors:
    - name: "Jan Tesař"
      gitHubUserName: "tesar-tech"
      xUserName: "tesar-tech"
---

## Update to .net10

- no issues so far

## Default FrontMatter update

Default `BlogFrontMatter` now supports `Updated` in addition to the existing `Published`.  
This provides a way to indicate how current a post is, helping readers understand its age and relevance.  
While it was previously possible to implement this manually, it is now built into the default front matter.

## Tip: keep publish date in one place (ther filename)

The DRY principle has me hard. In the [last realease](blog/release-1.0.0-beta.16) of BlazorStatic I intruduced the option to store you md files in a structure where
every post has its own folder.

```
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

Now - I like to call the posts (the folders here) by their title. But i also like to keep them chronologically.
The solution is to prefix the folders with the date - which is the same as publish date.

```
Blog/
  2025-01-20_post1/
     index.md
     media/
       image.png
  2025-03-25_post1/
     index.md
     media/
       image.png
  2025-05-28_post1/
     index.md
```

Now the WET issue - the publish date is on two places. Within the file name and within the front matter metadata.
Its ok to put it there, but it is not ok to change it multiple times as poblirh date is postponed.

## The solution

The solution is quite easy with BlazorStatic.\
The `AfterContentParsedAndAddedAction` is the hero here. As the name suggest it runs after all the parsing is done and we have the post, its url and metadata
at our disposal. The url is taken from the folder name, so we just parse it and trim it (or keep it if you want your url to start with the date)

```cs
//Program.cs

builder.Services.AddBlazorStaticService()
       .AddBlazorStaticContentService<BlogFrontMatter>(opt => {
           opt.AfterContentParsedAndAddedAction = (service, contentService) => {
               contentService.Posts.ForEach(post => {
                   if( post.Url.Split('_', 2) is [var datePart, var rest]
                       && DateTime.TryParseExact(datePart, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out var published) )
                   {
                       post.Url = rest;
                       post.FrontMatter.Published = published;
                   }
               });
           };
       });
```

And that's the magic. Now the date is on one place only.

## Feedback

Share feedback by [creating an issue](https://github.com/BlazorStatic/BlazorStatic/issues/new)\
or join the [Discord server](https://discord.gg/DsAXsMuEbx).
