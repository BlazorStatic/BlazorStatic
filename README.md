# <img id="imglogo" src="./BlazorStaticWebsite/wwwroot/imgs/logo.png" alt="blazor static">



[![Discord](https://img.shields.io/discord/798312431893348414?style=flat&logo=discord&logoColor=white&label=Blazor%20Community%2F%23BlazorStatic&labelColor=5865f2&color=gray)
](https://discord.gg/DsAXsMuEbx)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BlazorStatic)](https://www.nuget.org/packages/BlazorStatic/)
[![Build, publish to gh pages anN nuget](https://github.com/BlazorStatic/BlazorStatic/actions/workflows/publish-to-nuget.yml/badge.svg)](https://github.com/BlazorStatic/BlazorStatic/actions/workflows/publish-to-nuget.yml)
[![Netlify Status](https://api.netlify.com/api/v1/badges/4fa2c17a-6385-4cc6-9919-e32c134175d9/deploy-status)](https://app.netlify.com/sites/blazorstatic/deploys)


Harness the power of [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) to craft static websites.

Transform your Blazor app into a static site.

# 🔧 How does it work?

**BlazorStatic** aims to be intuitive and easy to use for most developers who have worked with any kind of front-end framework, especially if you already work with Blazor.

> The site for BlazorStatic is a feature-rich example of the library. Check the [source code](https://github.com/BlazorStatic/BlazorStatic/tree/master/BlazorStaticWebsite).

Upon running your app, **BlazorStatic** fetches the pages' HTML with `HttpClient`, outputs HTML files and assets to the `output` folder, and from there you can deploy to any standard HTTP server or hosting service.

There are many defaults to keep usage simple, but you can configure it extensively. For example, you can match your YAML front matter in markdown files (which can be reused from other static site generators).



# 🚀 Getting Started

You can start using BlazorStatic in three ways, depending on your setup and intended use. You'll need [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or higher to get started.
## 👑 Recommended Way

Install the BlazorStatic templates:

```sh
dotnet new install BlazorStatic.Templates
```

This adds new options to your `dotnet new` template list. Then create a new project using:

```sh
dotnet new BlazorStaticMinimalBlog -o MyBlazorStaticApp
```

This creates a `MyBlazorStaticApp` folder and project. It's a standard .NET Blazor app with the BlazorStatic package installed, including a few predefined components, styles, and a blog post with example content.

Run the app using `dotnet watch`. This will launch the app, allowing you to view it live, while also generating static HTML files in the `output` folder.

From here, you're ready to explore the project, add content, create new pages, and more.

- Markdown files are located in the `Content/Blog` folder.
- A `.gitignore` file is included with standard .NET recommendations and ignores the `output` folder.
- The template also includes a `.github` folder with a GitHub action for building and deploying to GitHub Pages. See the Deploying section below.
- You can customize anything. For further details, see [this document](./BlazorStaticWebsite/Content/Docs/new-start.md), which explains everything in more depth.

## 🏃‍➡️ Quickest Way

Use [BlazorStaticMinimalBlog](https://github.com/BlazorStatic/BlazorStaticMinimalBlog) as template (the big green button).

It will copy all the content and run the GitHub action for you. After it runs, go to settings and turn on GitHub pages:

![github pages](.github/media/README/img-1.png)

Your page is ready now!

## 🛠️ Custom Way

If you want to fully understand how BlazorStatic works or if you have an existing project where you'd like to integrate the BlazorStatic library, start from scratch. Check out [this document](./BlazorStaticWebsite/Content/Docs/new-start.md) for guidance.

# 🧩 Features

- Leverage your existing Blazor experience.
- Easily parse and consume markdown files.
- Support for custom YAML front matters.
- Automatically discovers pages among Razor files.
- Ability to add/remove pages as needed.
- Works with all CSS frameworks and themes.
- Easy to deploy with CI/CD pipeline.
- Supports HotReload
- Has `dotnet new` template


# 📦 Deploying

You can deploy the `output` folder to any static hosting service, such as GitHub Pages, Netlify, Vercel, Azure Static Web Apps, etc.

Detailed instructions are [here](./BlazorStaticWebsite/Content/Docs/deployment.md).

# 📊 Samples

| Description                                                  | Source                                                                                 | Live                                                            |
| ------------------------------------------------------------ |----------------------------------------------------------------------------------------|-----------------------------------------------------------------|
| Page about BlazorStatic (this repo contains the code itself) | [source](https://github.com/BlazorStatic/BlazorStatic/tree/master/BlazorStaticWebsite) | [live](https://blazorstatic.github.io/BlazorStatic/)            |
| Minimal blog                                                 | [source](https://github.com/BlazorStatic/BlazorStaticMinimalBlog)                      | [live](https://blazorstatic.github.io/BlazorStaticMinimalBlog/) |
| Zodoc - image processing and deep learning sample            | [source](https://github.com/tesar-tech/zodoc/)                                         | [live](https://zodoc.tech/)                                     |
| Bipin - Personal blog                                                | [source](https://github.com/iAmBipinPaul/bipinpaul.com)                                | [live](https://bipinpaul.com/)|
| ❓ Add your page here!!!                                      |                                                                                        |

# 🤝 Contributions

Contributions are highly encouraged and appreciated. If you find something missing, unclear, or encounter an issue with the code, I warmly welcome your input. Feel free to:

- Create a new [issue](https://github.com/BlazorStatic/BlazorStatic/issues) or submit a PR.
- Reach out via email: info@blazorstatic.net
- Contact [me](https://github.com/tesar-tech/) directly for any queries or suggestions.
- Ask questions or start a discussion on the [Blazor Community Discord server](https://discord.gg/DsAXsMuEbx).
