using System.Globalization;
using BlazorStatic;
using BlazorStaticWebsite;
using BlazorStaticWebsite.Components;
using Microsoft.Extensions.FileProviders;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddBlazorStaticService(opt => {
           opt.IgnoredPathsOnContentCopy.AddRange(["app.css"]); //pre-build version for tailwind

           opt.ContentToCopyToOutput.Add(new ContentToCopy("../.github/media", "media"));
           opt.ContentToCopyToOutput.Add(new ContentToCopy("Content/Docs/media", "Content/Docs/media"));

           // add docs pages
           var docsFiles = Directory.GetFiles(Path.Combine("Content", "Docs"), "*.md")
                                    .Where(x => !x.EndsWith(
                                                "README.md")); //ignore readme, it is handled in Pages/Docs.razor

           foreach(var fileName in docsFiles.Select(Path.GetFileNameWithoutExtension))
           {
               opt.PagesToGenerate.Add(new PageToGenerate($"/docs/{fileName}",
               Path.Combine("docs", $"{fileName}.html")));
           }

           // Must add a site url to generate the Sitemap!
           opt.ShouldGenerateSitemap = true;
           opt.SiteUrl = WebsiteKeys.SiteUrl;
           opt.HotReloadEnabled = true;
       })
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
               service.Options.PagesToGenerate.ForEach(page => { });
           };
       }) //
       .AddBlazorStaticContentService<ProjectFrontMatter>(opt => {
           opt.ContentPath = Path.Combine("Content", "Projects");
           opt.PageUrl = WebsiteKeys.ProjectsUrl;
       })
;

// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddFluentUIComponents();


var app = builder.Build();


// Configure the HTTP request pipeline.
if( !app.Environment.IsDevelopment() )
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
                   {
                       FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                       "Content", "Docs", "media")),
                       RequestPath = "/Content/Docs/media"
                   });

app.UseStaticFiles(new StaticFileOptions //for readme images
                   {
                       FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "..",
                       ".github", "media")),
                       RequestPath = "/media"
                   });


app.UseAntiforgery();

app.MapRazorComponents<App>();

app.UseBlazorStaticGenerator(!app.Environment.IsDevelopment());

app.Run();


public static class WebsiteKeys
{
    public const string BlogPostStorageAddress = $"{GitHubRepo}tree/master/BlazorStaticWebsite/Content/Blog/";

    public const string GitHubRepo = "https://github.com/BlazorStatic/BlazorStatic/";

    public const string ProjectsUrl = "projects";

    public const string SiteUrl = "https://blazorstatic.net";
}