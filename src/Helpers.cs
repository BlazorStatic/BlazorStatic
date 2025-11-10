using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using BlazorStatic.Dtos;
using YamlDotNet.Serialization;

namespace BlazorStatic;

/// <summary>
///     Helpers for BlazorStatic
/// </summary>
/// <param name="options"></param>
/// <param name="logger"></param>
public class BlazorStaticHelpers(BlazorStaticOptions options, ILogger<BlazorStaticHelpers> logger)
{
    /// <summary>
    ///     Parses a markdown file and returns the HTML content.
    ///     Uses the options.MarkdownPipeline to parse the markdown (set this in BlazorStaticOptions).
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="contentPath">Usually blazorStaticContentOptions.ContentPath</param>
    /// <returns></returns>
    public async Task<string> ParseMarkdownFile(string filePath, string? contentPath = null)
    {
        var markdownContent = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
        var htmlContent = Markdown.ToHtml(markdownContent, options.MarkdownPipeline);
        var finalHtmlContentAndFolders =
        HtmlMediaRewriter.RewriteImageSources(htmlContent, filePath, contentPath, logger);
        return finalHtmlContentAndFolders.Html;
    }

    /// <summary>
    ///     Parses a markdown file and returns the HTML content and the front matter.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="contentPath"></param>
    /// <param name="yamlDeserializer"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<MarkdownParsingResult<T>> ParseMarkdownFile<T>(string filePath, string? contentPath = null,
    IDeserializer? yamlDeserializer = null) where T : new()
    {
        yamlDeserializer ??= options.FrontMatterDeserializer;
        var markdownContent = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
        var document = Markdown.Parse(markdownContent, options.MarkdownPipeline);

        var yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
        T frontMatter;
        if( yamlBlock == null )
        {
            logger.LogWarning("No YAML front matter found in {file}. The default one will be used!", filePath);
            frontMatter = new T();
        }
        else
        {
            var frontMatterYaml = yamlBlock.Lines.ToString();

            try
            {
                frontMatter = yamlDeserializer.Deserialize<T>(frontMatterYaml);
            }
            catch(Exception e)
            {
                frontMatter = new T();
                logger.LogWarning(
                "Cannot deserialize YAML front matter in {file}. The default one will be used! Error: {exceptionMessage}",
                filePath, e.Message + e.InnerException?.Message);
            }
        }

        var contentWithoutFrontMatter = markdownContent[(yamlBlock == null? 0: yamlBlock.Span.End + 1)..];
        var htmlContent = Markdown.ToHtml(contentWithoutFrontMatter, options.MarkdownPipeline);
        var (finalHtmlContent, folders) =
        HtmlMediaRewriter.RewriteImageSources(htmlContent, filePath, contentPath, logger);
        return new MarkdownParsingResult<T>
               { HtmlContent = finalHtmlContent, FrontMatter = frontMatter, MediaFolders = folders };
    }


    /// <summary>
    ///     Copies content from sourcePath to targetPath.
    ///     For example wwwroot to output folder.
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="targetPath"></param>
    /// <param name="ignoredPaths">Target (full)paths that gets ignored.</param>
    public void CopyContent(string sourcePath, string targetPath, List<string> ignoredPaths)
    {
        if( File.Exists(sourcePath) ) // source path is a file
        {
            var dir = Path.GetDirectoryName(targetPath);
            if( dir == null )
            {
                logger.LogError("Target directory path is null for the file: {sourcePath}", sourcePath);
                return;
            }

            Directory.CreateDirectory(dir);
            File.Copy(sourcePath, targetPath, true);
            return;
        }

        if( !Directory.Exists(sourcePath) )
        {
            logger.LogError("Source path ({sourcePath}) does not exist.", sourcePath);
            return;
        }

        if( !Directory.Exists(targetPath) )
        {
            Directory.CreateDirectory(targetPath);
        }

        var ignoredPathsWithTarget = ignoredPaths.Select(x => Path.Combine(targetPath, x)).ToList();

        // Now Create all of the directories
        foreach(var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            var newDirPath = ChangeRootFolder(dirPath);
            if( ignoredPathsWithTarget
               .Contains(newDirPath) ) // folder is mentioned in ignoredPaths, don't create it
            {
                continue;
            }

            Directory.CreateDirectory(newDirPath);
        }

        // Copy all the files & replace any files with the same name
        foreach(var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            var newPathWithNewDir = ChangeRootFolder(newPath);
            if( ignoredPathsWithTarget.Contains(newPathWithNewDir) // file is mentioned in ignoredPaths
                || !Directory.Exists(
                Path.GetDirectoryName(
                newPathWithNewDir)) ) // folder where this file resides is mentioned in ignoredPaths
            {
                continue;
            }

            File.Copy(newPath, newPathWithNewDir, true);
        }

        return;

        string
        ChangeRootFolder(string dirPath) // for example from "wwwroot/imgs" to "output/imgs" (safer string.Replace)
        {
            var relativePath = dirPath[sourcePath.Length..].TrimStart(Path.DirectorySeparatorChar);
            return Path.Combine(targetPath, relativePath);
        }
    }
}