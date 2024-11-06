using BlazorStatic.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorStatic;

/// <summary>
///     Extension methods for configuring BlazorStatic services in an <see cref="IServiceCollection" />.
/// </summary>
public static class BlazorStaticExtensions
{
    private static readonly Dictionary<Type, Action> s_actionsToConfigureOptions = new();

    private static WebApplication? s_app;
    /// <summary>
    ///     holds the actions that will run when UseBlazorStaticGenerator is called.
    ///     Will manage content for every BlazorStaticContentService added.
    ///     This makes sure the BlazorStaticService has some connection to BlazorStaticContentService
    ///     We use dictionary for type, because it removes the need for hassling with duplicate (in case of hot reload)
    /// </summary>
    private static Dictionary<Type, Action<WebApplication>> staticContentUse { get; } = [];


    /// <summary>
    ///     Adds a BlazorStaticContentService to the specified IServiceCollection. The BlazorStaticContentService service uses
    ///     a generic type
    ///     for front matter, allowing customization of the metadata format used in posts.
    /// </summary>
    /// <typeparam name="TFrontMatter">The type of front matter used in the posts. Must implement IFrontMatter.</typeparam>
    /// <typeparam name="TBlazorStaticContentOptions">If you have your own IBlazorStaticContentOptions, here is the place. Otherwise
    /// (if you are fine with BlazorStaticContentOptions) use the other overload (AddBlazorStaticContentService`TFrontMatter)
    /// </typeparam>
    /// <param name="services">The IServiceCollection to add the BlazorStaticContentService to.</param>
    /// <param name="configureOptions">
    ///     An optional action to configure the BlazorStaticContentOptions for the BlazorStaticContentService.
    ///     Default values are set for Blog content.
    /// </param>
    /// <returns>The IServiceCollection, with the BlazorStaticContentService added, allowing for method chaining.</returns>
    /// <remarks>
    ///     The method configures and registers a singleton instance of BlazorStaticContentOptions`TFrontMatter` and
    ///     BlazorStaticContentService`TFrontMatter` in the service collection.
    /// </remarks>
    public static IServiceCollection AddBlazorStaticContentService<TFrontMatter, TBlazorStaticContentOptions>(this IServiceCollection services,
        Action<TBlazorStaticContentOptions>? configureOptions)
        where TFrontMatter : class, IFrontMatter, new()
        where TBlazorStaticContentOptions : class, IBlazorStaticContentOptions<TFrontMatter>, new()
    {
        TBlazorStaticContentOptions options = new();
        ConfigureOptions();

        services.AddSingleton(options);
        services.AddSingleton<BlazorStaticContentService<TFrontMatter, TBlazorStaticContentOptions>>();

        staticContentUse[typeof(TFrontMatter)] = UseBlazorStaticContent<TFrontMatter, TBlazorStaticContentOptions>;
        s_actionsToConfigureOptions[typeof(TFrontMatter)] = ConfigureOptions;
        return services;

        void ConfigureOptions()
        {
            configureOptions?.Invoke(options);
            options.CheckOptions();
        }
    }


    /// <summary>
    ///     Adds a BlazorStaticContentService to the specified IServiceCollection. The BlazorStaticContentService service uses
    ///     a generic type
    ///     for front matter, allowing customization of the metadata format used in posts.
    /// </summary>
    /// <typeparam name="TFrontMatter">The type of front matter used in the posts. Must implement IFrontMatter.</typeparam>
    /// <param name="services">The IServiceCollection to add the BlazorStaticContentService to.</param>
    /// <param name="configureOptions">
    ///     An optional action to configure the BlazorStaticContentOptions for the BlazorStaticContentService.
    ///     Default values are set for Blog content.
    /// </param>
    /// <returns>The IServiceCollection, with the BlazorStaticContentService added, allowing for method chaining.</returns>
    /// <remarks>
    ///     The method configures and registers a singleton instance of BlazorStaticContentOptions`TFrontMatter` and
    ///     BlazorStaticContentService`TFrontMatter` in the service collection.
    /// </remarks>
    public static IServiceCollection AddBlazorStaticContentService<TFrontMatter>(this IServiceCollection services,
        Action<BlazorStaticContentOptions<TFrontMatter>>? configureOptions)
        where TFrontMatter : class, IFrontMatter, new()
    {
        AddBlazorStaticContentService<TFrontMatter, BlazorStaticContentOptions<TFrontMatter>>(services, configureOptions);
        services.AddSingleton<BlazorStaticContentService<TFrontMatter>>();

        return services;
    }




    /// <summary>
    ///     Adds the Blazor static generation service to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the Blazor static service to.</param>
    /// <param name="configureOptions">An optional action to configure the BlazorStaticOptions for the service.</param>
    /// <returns>The IServiceCollection, with the Blazor static service added, allowing for method chaining.</returns>
    /// <remarks>
    ///     The method registers a singleton instance of BlazorStaticHelpers and BlazorStaticService in the service collection.
    ///     Additionally, it creates and configures an instance of BlazorStaticOptions, which is used to control the behavior
    ///     of the static generation process. This setup is essential for applications leveraging Blazor for static site
    ///     generation,
    ///     providing customizable options for the generation process.
    /// </remarks>
    public static IServiceCollection AddBlazorStaticService(this IServiceCollection services,
        Action<BlazorStaticOptions>? configureOptions = null)
    {
        services.AddSingleton<BlazorStaticHelpers>();
        var options = new BlazorStaticOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<BlazorStaticService>();
        s_actionsToConfigureOptions[typeof(BlazorStaticService)] = () => configureOptions?.Invoke(options);

        return services;
    }


    /// <summary>
    ///     Runs the actions necessary to generating static content by settings defined in options
    ///     Runs the actions necessary to generating static content by settings defined in options
    /// </summary>
    /// <param name="app"></param>
    /// <typeparam name="TFrontMatter"></typeparam>
    /// <typeparam name="TBlazorStaticContentOptions"></typeparam>
    private static void UseBlazorStaticContent<TFrontMatter, TBlazorStaticContentOptions>(WebApplication app)
        where TFrontMatter : class, IFrontMatter, new()
        where TBlazorStaticContentOptions : class, IBlazorStaticContentOptions<TFrontMatter>, new()

    {
        var contentService = app.Services.GetRequiredService<BlazorStaticContentService<TFrontMatter, TBlazorStaticContentOptions>>();
        contentService.Posts.Clear();//need to do it here in case of hot reload event
        var options = app.Services.GetRequiredService<TBlazorStaticContentOptions>();
        var blazorStaticService = app.Services.GetRequiredService<BlazorStaticService>();


        //Add static files for media files to be accessible while running the app

        //StaticFileOptions doesn't like ".." parent dir (e.g "Content/Blog/en/../media")
        //This converts it to "/Content/Blog/media"...

        if(options.MediaRequestPath is not null && options.MediaFolderRelativeToContentPath is not null)
        {
            var requestPath = "/" + Path.GetFullPath(options.MediaRequestPath)[Directory.GetCurrentDirectory().Length..]
                .TrimStart(Path.DirectorySeparatorChar)
                .Replace("\\", "/");


            var realPath = Path.Combine(Directory.GetCurrentDirectory(), options.ContentPath, options.MediaFolderRelativeToContentPath);
            if(!Directory.Exists(realPath))
            {
                app.Logger.LogWarning("folder for media path ({Folder}) doesn't exist", realPath);
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(realPath),
                    RequestPath = requestPath
                });
            }
        }
        //

        blazorStaticService.Options.AddBeforeFilesGenerationAction(contentService.ParseAndAddPosts);//will run later in GenerateStaticPages
    }

    internal static void UseBlazorStaticGeneratorOnHotReload()
    {
        if(s_app == null)
        {
            return;
        }

        var blazorStaticService = s_app.Services.GetRequiredService<BlazorStaticService>();
        //basic clean up
        blazorStaticService.Options.ClearBeforeFilesGenerationActions();
        blazorStaticService.Options.PagesToGenerate.Clear();
        blazorStaticService.Options.ContentToCopyToOutput.Clear();

        //go through the options (from Program.cs)
        foreach(var action in s_actionsToConfigureOptions)
        {
            action.Value.Invoke();
        }

        s_app.UseBlazorStaticGenerator();
    }

    /// <summary>
    ///     Adds the Blazor static generation service to the specified IServiceCollection.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="shutdownApp"></param>
    public static void UseBlazorStaticGenerator(this WebApplication app, bool shutdownApp = false)
    {
        foreach(var use in staticContentUse)
        {
            use.Value.Invoke(app);
        }

        var blazorStaticService = app.Services.GetRequiredService<BlazorStaticService>();
        if(s_app is null && blazorStaticService.Options.HotReloadEnabled)
        {
            s_app = app;
        }

        HotReloadManager.HotReloadEnabled = blazorStaticService.Options.HotReloadEnabled;

        //adds wwwroot files (or any other files that has been added as static content) to the output
        AddStaticWebAssetsToOutput(app.Environment.WebRootFileProvider, string.Empty, blazorStaticService);

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

        var logger = app.Services.GetRequiredService<ILogger<BlazorStaticService>>();

        lifetime.ApplicationStarted.Register(
        // ReSharper disable once AsyncVoidLambda
        async () => {
            try
            {
                await blazorStaticService.GenerateStaticPages(app.Urls.First()).ConfigureAwait(false);
                if(shutdownApp)
                {
                    lifetime.StopApplication();
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred while generating static pages: {ErrorMessage}", ex.Message);
            }
        }
        );
    }

    /// <summary>
    ///     Takes the provider, search it recursively and add all the files found.
    /// </summary>
    /// <param name="fileProvider"></param>
    /// <param name="subPath"></param>
    /// <param name="blazorStaticService"></param>
    private static void AddStaticWebAssetsToOutput(IFileProvider fileProvider, string subPath, BlazorStaticService blazorStaticService)
    {
        var contents = fileProvider.GetDirectoryContents(subPath);

        foreach(var item in contents)
        {
            var fullPath = $"{subPath}{item.Name}";
            if(item.IsDirectory)
            {
                AddStaticWebAssetsToOutput(fileProvider, $"{fullPath}/", blazorStaticService);
            }
            else
            {
                if(item.PhysicalPath is not null)
                {
                    blazorStaticService.Options.ContentToCopyToOutput.Add(new ContentToCopy(item.PhysicalPath, fullPath));
                }
            }
        }
    }
}
