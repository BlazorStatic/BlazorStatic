using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace BlazorStatic.Services;

// borrowed code with minor changes
// https://andrewlock.net/finding-all-routable-components-in-a-webassembly-app/
internal static class RoutesHelper
{
    /// <summary>
    ///     Gets the static routes of a blazor app
    /// </summary>
    /// <param name="assembly">assembly of the blazor app</param>
    /// <returns>a list of static routes</returns>
    public static string[] GetRoutesToRender(Assembly assembly)
    {
        return assembly.ExportedTypes
            .Where(t => t.IsSubclassOf(typeof(ComponentBase)))
            .SelectMany(GetRouteFromComponent)
            .OfType<string>()
            .ToArray();
    }

    /// <summary>
    ///     Gets the routes from a blazor component
    /// </summary>
    /// <param name="component"></param>
    /// <returns>The routes of the component.</returns>
    private static string[] GetRouteFromComponent(Type component)
    {
        var attributes = component.GetCustomAttributes(typeof(RouteAttribute), inherit: false);
        var routes = new string[attributes.Length];
        for(int i = 0; i < attributes.Length; i++)
        {
            var attr = (RouteAttribute)attributes[i];
            if(!attr.Template.Contains('{'))
            {
                routes[i] = attr.Template;
            }
        }
        return routes;
    }
}
