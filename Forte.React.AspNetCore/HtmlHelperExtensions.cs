using System.Threading.Tasks;
using Forte.React.AspNetCore.React;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Forte.React.AspNetCore;

public static class HtmlHelperExtensions
{
    public static async Task<IHtmlContent> ReactAsync<T>(this IHtmlHelper htmlHelper, string componentName, T props)
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return new HtmlString(await reactService.RenderToStringAsync(componentName, props));
    }

    public static IHtmlContent InitJavascript(this IHtmlHelper htmlHelper)
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return new HtmlString(reactService.GetInitJavascript());
    }
}
