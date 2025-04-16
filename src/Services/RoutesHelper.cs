using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace BlazorStatic.Services;

/// <summary>
/// RoutesHelper is responsible for extracting static, non-parameterized routes from Blazor components within a given assembly. <br />
/// code is borrowed with some changes from:
/// https://andrewlock.net/finding-all-routable-components-in-a-webassembly-app/
/// </summary>
internal static class RoutesHelper
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
    private static string[] GetRoutesFromComponent(Type component)
    {
        var attributes = component.GetCustomAttributes(typeof(RouteAttribute), inherit: false);
        var routes = new string[attributes.Length];
        for(int i = 0; i < attributes.Length; i++)
        {
            var attr = (RouteAttribute)attributes[i];
            // Ignore parameterized routes (e.g /{Id}) because we can't generate them.
            if(!attr.Template.Contains('{'))
            {
                routes[i] = attr.Template;
            }
        }
        return routes;
    }
}
