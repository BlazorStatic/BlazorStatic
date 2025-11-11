using Moq;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BlazorStatic.Tests;

public class BlazorStaticHelpersTests
{
    readonly BlazorStaticHelpers _blazorStaticHelpers;
    readonly Mock<ILogger<BlazorStaticHelpers>> _mockLogger;

    public BlazorStaticHelpersTests()
    {
        var options = new BlazorStaticOptions();
        _mockLogger = new Mock<ILogger<BlazorStaticHelpers>>();
        _blazorStaticHelpers = new BlazorStaticHelpers(options, _mockLogger.Object);
    }

    [Fact]
    public async Task ParseMarkdownFile_WithMediaPaths_ReturnCorrectHtmlAndFrontMatterObject()
    {
        var contentPath = "Content/Blog";
        var fileAbsolutePath = Path.GetFullPath(Path.Combine(contentPath, "test1.md"));
        // Act
        var res = await _blazorStaticHelpers.ParseMarkdownFile<TestFrontMatter>(fileAbsolutePath, contentPath);

        // Assert
        Assert.Contains(
        """
        <h2 id="hello-markdown">Hello Markdown</h2>
        """,
        res.HtmlContent
        );

        // md image
        Assert.Contains(Path.Combine(contentPath, "media/test.png"), res.HtmlContent);

        // hlml image
        Assert.Contains(
        $"""
         <img src="{Path.Combine(contentPath, "media/test.png")}" alt="something"/>
         """,
        res.HtmlContent
        );
        // doesnt care about external images
        Assert.Contains(
        $"""
         <img src="https://blazorstatic.net/img.png" alt="something"/>
         """,
        res.HtmlContent
        );
        Assert.Equal("Test Post", res.FrontMatter.Title);
    }

    [Fact]
    public async Task ParseMarkdownFile_LogsWarningOnNonExistingImage()
    {
        var contentPath = Path.GetFullPath("Content/Blog");
        var fileAbsolutePath = Path.GetFullPath(Path.Combine(contentPath, "test2.md"));

        // Act
        var res = await _blazorStaticHelpers
                  .ParseMarkdownFile<TestFrontMatter>(fileAbsolutePath, contentPath);

        // Assert: existing assertion
        Assert.Equal("Test Post", res.FrontMatter.Title);

        // Assert: warning was logged once
        _mockLogger.Verify(
        logger => logger.Log(
        LogLevel.Warning,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Image file not found")),
        null,
        (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
        Times.Once);
    }

    [Fact]
    public async Task
    ParseMarkdownFile_WithMediaPathsInPostsInSeparateFolders_ReturnCorrectHtmlAndFrontMatterObject()
    {
        // Act
        var contentPath = "Content/Blog2";
        string folderForPost = "folder-for-post";
        var fileAbsolutePath = Path.GetFullPath(Path.Combine(contentPath, folderForPost, "index.md"));
        // Act
        var res = await _blazorStaticHelpers.ParseMarkdownFile<TestFrontMatter>(fileAbsolutePath, contentPath);

        // Assert
        Assert.Contains(
        """
        <h2 id="hello-markdown">Hello Markdown</h2>
        """,
        res.HtmlContent
        );

        Assert.Contains($""" src="{Path.Combine(contentPath, folderForPost, "media/test.png")}" """,
        res.HtmlContent);
        Assert.Equal("Test Post", res.FrontMatter.Title);
    }

    [Fact]
    public async Task ParseMarkdownFile_WithoutMediaPaths_ReturnCorrectHtml()
    {
        // Arrange
        string fakeFilePath = "test-no-paths.md";
        string fakeMarkdownContent = $"""
                                      ## Example Markdown with Image

                                      ![example](media/sample.jpg)
                                      """;

        // Write a fake Markdown file
        await File.WriteAllTextAsync(fakeFilePath, fakeMarkdownContent);

        // Act
        string htmlContent = await _blazorStaticHelpers.ParseMarkdownFile(fakeFilePath);

        // Assert
        Assert.Contains("<h2 id=\"example-markdown-with-image\">Example Markdown with Image</h2>",
        htmlContent); // Markdown converted
        Assert.Contains("media/sample.jpg", htmlContent); // The media path should remain unchanged

        // Cleanup
        File.Delete(fakeFilePath);
    }

    [Fact]
    public async Task ParseMarkdownFile_WithPascalCaseYamlDeserializer_ParsesCorrectly()
    {
        // Arrange
        string filePath = "test-pascalcase.md";
        string markdownContent = $"""
                                  ---
                                  Title: Pascal Case Test
                                  Description: Test YAML with PascalCase naming convention
                                  IsDraft: true
                                  ---
                                  ## Pascal Case Example
                                  Content goes here.
                                  """;

        var customYamlDeserializer = new DeserializerBuilder()
                                     .WithNamingConvention(PascalCaseNamingConvention.Instance)
                                     .IgnoreUnmatchedProperties()
                                     .Build();

        // Save the temporary Markdown file to disk
        await File.WriteAllTextAsync(filePath, markdownContent);

        // Act
        var res =
        await _blazorStaticHelpers.ParseMarkdownFile<TestFrontMatter>(filePath,
        yamlDeserializer: customYamlDeserializer);

        // Assert
        Assert.NotNull(res.HtmlContent);
        Assert.Contains("<h2 id=\"pascal-case-example\">Pascal Case Example</h2>",
        res.HtmlContent); // Confirms Markdown converted to HTML

        Assert.NotNull(res.FrontMatter); // Ensure YAML was deserialized
        Assert.Equal("Pascal Case Test", res.FrontMatter.Title); // Verify PascalCase key "Title" is parsed
        Assert.Equal("Test YAML with PascalCase naming convention",
        res.FrontMatter.Description); // Verify PascalCase key "Description"
        Assert.True(res.FrontMatter.IsDraft); // Verify PascalCase key "IsDraft"

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void CopyContent_WithIgnoredPaths_HandlesPathsCorrectly()
    {
        // Arrange: Create temp test directories
        var sourcePath = Path.Combine(Path.GetTempPath(), "source");
        var targetFolder = Path.Combine(Path.GetTempPath(), "target");

        var ignoredPaths = new List<string>
                           {
                               "ignoredDir", // Whole directory should be ignored
                               "dirWithIgnoredFile/ignoredFile.txt" // Specific file in a non-ignored dir
                           };

        CreateTestSourceStructure(sourcePath);

        // Act: Call CopyContent
        _blazorStaticHelpers.CopyContent(
        sourcePath,
        targetFolder,
        ignoredPaths.ConvertAll(path => Path.Combine(targetFolder, path))
        );

        try
        {
            // Assert: Check if files and folders are correctly copied or ignored
            // File in an ignored directory should not exist
            Assert.False(Directory.Exists(Path.Combine(targetFolder, "ignoredDir")),
            "Ignored directory should not exist");
            Assert.False(File.Exists(Path.Combine(targetFolder, "ignoredDir", "fileInIgnoredDir.txt")),
            "File in ignored directory should not exist");

            // File in a non-ignored directory should exist
            Assert.True(Directory.Exists(Path.Combine(targetFolder, "normalDir")), "Normal directory should exist");
            Assert.True(File.Exists(Path.Combine(targetFolder, "normalDir", "fileInNormalDir.txt")),
            "File in normal directory should exist");

            // File that is specifically ignored should not exist
            Assert.True(Directory.Exists(Path.Combine(targetFolder, "dirWithIgnoredFile")),
            "Directory containing ignored file should exist");
            Assert.False(File.Exists(Path.Combine(targetFolder, "dirWithIgnoredFile", "ignoredFile.txt")),
            "Specifically ignored file should not exist");

            // File not ignored in the same directory should exist
            Assert.True(File.Exists(Path.Combine(targetFolder, "dirWithIgnoredFile", "fileNotIgnored.txt")),
            "Non-ignored file should exist");
        }
        finally
        {
            // Cleanup
            Directory.Delete(sourcePath, true);
            Directory.Delete(targetFolder, true);
        }
    }

    [Fact]
    public void CopyContent_WithSingleFile_CopiesFile()
    {
        // Arrange
        var sourceFilePath = Path.Combine(Path.GetTempPath(), "sourceFile.txt");
        var targetFilePath = Path.Combine(Path.GetTempPath(), "targetFile.txt");

        // Create a single source file
        File.WriteAllText(sourceFilePath, "This is a test file.");

        var helpers = new BlazorStaticHelpers(new BlazorStaticOptions(), null!);

        // Act
        helpers.CopyContent(sourceFilePath, targetFilePath, new List<string>());

        try
        {
            // Assert: The target file should be created and contain the same content
            Assert.True(File.Exists(targetFilePath), "The target file should exist.");
            Assert.Equal("This is a test file.", File.ReadAllText(targetFilePath));
        }
        finally
        {
            // Cleanup
            File.Delete(sourceFilePath);
            File.Delete(targetFilePath);
        }
    }

    [Fact]
    public void CopyContent_WithNonExistentSource_LogsError()
    {
        // Arrange
        var sourcePath = Path.Combine(Path.GetTempPath(), "nonexistentSource");
        var targetPath = Path.Combine(Path.GetTempPath(), "target");

        // Act
        _blazorStaticHelpers.CopyContent(sourcePath, targetPath, new List<string>());

        try
        {
            // Assert: Target folder should not be created
            Assert.False(Directory.Exists(targetPath),
            "Target directory should not exist if source path does not exist.");

            // Verify that an error was logged
            _mockLogger.Verify(
            logger => logger.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("does not exist")),
            null,
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
            ), Times.Once);
        }
        finally
        {
            // Cleanup
            if( Directory.Exists(targetPath) )
            {
                Directory.Delete(targetPath, true);
            }
        }
    }

    [Fact]
    public void CopyContent_WithNullTargetDirectory_LogsError()
    {
        // Arrange
        var sourceFilePath = Path.Combine(Path.GetTempPath(), "someFile.txt");
        string? targetPath = null; // This will ensure Path.GetDirectoryName returns null

        // Create a file at the source path
        File.WriteAllText(sourceFilePath, "This is a test file.");

        // Act
        _blazorStaticHelpers.CopyContent(sourceFilePath, targetPath, new List<string>());

        try
        {
            // Assert: Verify that an error was logged
            _mockLogger.Verify(logger =>
            logger.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) =>
            v.ToString().Contains($"Target directory path is null for the file: {sourceFilePath}")),
            null,
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once
            );
        }
        finally
        {
            // Cleanup: Delete the source file
            if( File.Exists(sourceFilePath) )
            {
                File.Delete(sourceFilePath);
            }
        }
    }

    [Fact]
    public async Task ParseMarkdownFile_WithNoYamlFrontMatter_LogsWarning()
    {
        // Arrange
        string fakeFilePath = "test-no-yaml.md";
        string fakeMarkdownContent = """
                                     ## Example Markdown without Front Matter
                                     This file has no YAML front matter.
                                     """;

        // Write the fake Markdown file
        await File.WriteAllTextAsync(fakeFilePath, fakeMarkdownContent);

        // Act
        var res = await _blazorStaticHelpers.ParseMarkdownFile<TestFrontMatter>(fakeFilePath);

        try
        {
            // Assert: Verify the content was parsed correctly
            Assert.NotNull(res.HtmlContent); // Content should still parse to HTML
            Assert.Contains(
            "<h2 id=\"example-markdown-without-front-matter\">Example Markdown without Front Matter</h2>",
            res.HtmlContent);
            Assert.NotNull(res.FrontMatter); // A default front matter object should be created
            Assert.IsType<TestFrontMatter>(res.FrontMatter);

            // Verify that a warning log was triggered for the missing YAML front matter
            _mockLogger.Verify(logger =>
            logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) =>
            v.ToString().Contains("No YAML front matter found in") && v.ToString().Contains(fakeFilePath)),
            null,
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once
            );
        }
        finally
        {
            // Cleanup
            if( File.Exists(fakeFilePath) )
            {
                File.Delete(fakeFilePath);
            }
        }
    }

    [Fact]
    public async Task ParseMarkdownFile_WithInvalidYaml_LogsWarningAndReturnsDefault()
    {
        // Arrange
        string fakeFilePath = "test-invalid-yaml.md";
        string markdownContentWithInvalidYaml = $"""
                                                 ---
                                                 title: :::: Invalid YAML here ::::
                                                 ---
                                                 ## Example Markdown
                                                 This content is valid Markdown.
                                                 """;

        // Write the invalid Markdown content to a file
        await File.WriteAllTextAsync(fakeFilePath, markdownContentWithInvalidYaml);

        // Act
        var res = await _blazorStaticHelpers.ParseMarkdownFile<TestFrontMatter>(fakeFilePath);

        try
        {
            // Assert: Ensure a warning log is triggered for YAML deserialization failure
            _mockLogger.Verify(
            logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) =>
            v.ToString().Contains("Cannot deserialize YAML front matter in")
            && v.ToString().Contains(fakeFilePath)
            && v.ToString().Contains("Error:")),
            null,
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once
            );

            // Assert: Ensure the HTML content was parsed correctly
            Assert.NotNull(res.HtmlContent);
            Assert.Contains("<h2 id=\"example-markdown\">Example Markdown</h2>", res.HtmlContent);
            Assert.Contains("This content is valid Markdown.", res.HtmlContent);

            // Assert: Ensure the front matter defaults to a new instance of TestFrontMatter
            Assert.NotNull(res.FrontMatter);
            Assert.IsType<TestFrontMatter>(res.FrontMatter);
        }
        finally
        {
            // Cleanup: Delete the test Markdown file
            if( File.Exists(fakeFilePath) )
            {
                File.Delete(fakeFilePath);
            }
        }
    }


    static void CreateTestSourceStructure(string sourcePath)
    {
        // Clean up if the directory already exists
        if( Directory.Exists(sourcePath) )
            Directory.Delete(sourcePath, true);

        // Create test folder structure
        Directory.CreateDirectory(sourcePath);
        Directory.CreateDirectory(Path.Combine(sourcePath, "ignoredDir"));
        Directory.CreateDirectory(Path.Combine(sourcePath, "normalDir"));
        Directory.CreateDirectory(Path.Combine(sourcePath, "dirWithIgnoredFile"));

        // Add files
        File.WriteAllText(Path.Combine(sourcePath, "ignoredDir", "fileInIgnoredDir.txt"),
        "This file is in an ignored directory");
        File.WriteAllText(Path.Combine(sourcePath, "normalDir", "fileInNormalDir.txt"),
        "This file is in a normal directory");
        File.WriteAllText(Path.Combine(sourcePath, "dirWithIgnoredFile", "ignoredFile.txt"),
        "This file is specifically ignored");
        File.WriteAllText(Path.Combine(sourcePath, "dirWithIgnoredFile", "fileNotIgnored.txt"),
        "This file is not ignored");
    }
}

// Example implementation of IFrontMatter
public class TestFrontMatter : IFrontMatter
{
    public bool IsDraft { get; set; }
    public string Title { get; set; } = "";
    public AdditionalInfo AdditionalInfo { get; set; }
    public string Description { get; set; } = "";
}