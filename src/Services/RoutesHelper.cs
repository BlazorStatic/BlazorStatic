using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace BlazorStatic.Services;

/// <summary>
/// RoutesHelper is responsible for extracting static, non-parameterized routes from Blazor components within a given assembly. <br />
/// code is borrowed with some changes from:
/// https://andrewlock.net/finding-all-routable-components-in-a-webassembly-app/
/// </summary>
public static class RoutesHelper
{
    /// <summary>
    ///     Gets the static routes of a blazor app
    /// </summary>
    /// <param name="assembly">assembly of the blazor app</param>
    /// <returns>An array of static routes</returns>
    public static string[] GetRoutesToRender(Assembly assembly)
    {
        return assembly.ExportedTypes
            .Where(t => t.IsSubclassOf(typeof(ComponentBase)))
            .SelectMany(GetRoutesFromComponent)
            .OfType<string>()
            .ToArray();
    }

    /// <summary>
    ///     Gets the static routes of a blazor component
    /// </summary>
    /// <param name="component"></param>
    /// <returns>
    ///     An array of static routes. <br />
    ///     Array values can contain null. <br />
    ///     Returns an empty array if the component doesn't have `@page` directive.
    /// </returns>
    private static IEnumerable<string> GetRoutesFromComponent(Type component)
    {
        return component
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Where(attr => !attr.Template.Contains('{')) // Ignore parameterized routes (e.g /{Id}) because we can't generate them.
            .Select(attr => attr.Template);
    }
}
