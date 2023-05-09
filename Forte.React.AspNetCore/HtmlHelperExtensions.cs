using Forte.React.AspNetCore.React;
using Microsoft.Extensions.DependencyInjection;

#if NET48
using System.Web.Mvc;
using System.Web;
#endif

#if NET6_0_OR_GREATER
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#endif

namespace Forte.React.AspNetCore;

public static class HtmlHelperExtensions
{

#if NET48
    public static IHtmlString React<T>(this HtmlHelper<dynamic> htmlHelper, string componentName, T props)
    {
        var reactService = htmlHelper.ViewContext.HttpContext.GetRequiredService<IReactService>();
        var renderedComponent = reactService.RenderToStringAsync(componentName, props).Result;

        return new HtmlString(renderedComponent);
    }
    
    public static IHtmlString React<TComponent>(this HtmlHelper<dynamic> htmlHelper, TComponent component) where TComponent : IReactComponent
    {
        var reactService = htmlHelper.ViewContext.HttpContext.GetRequiredService<IReactService>();
        var renderedComponent = reactService.RenderToStringAsync(component.Path, null, component.ClientOnly).Result;

        return new HtmlString(renderedComponent);
    }
    
    public static IHtmlString React<TComponent, TProps>(this HtmlHelper<dynamic> htmlHelper, TComponent component) where TComponent : IReactComponent<TProps> where TProps : IReactComponentProps
    {
        var reactService = htmlHelper.ViewContext.HttpContext.GetRequiredService<IReactService>();
        var renderedComponent = reactService.RenderToStringAsync(component.Path, component.Props, component.ClientOnly).Result;

        return new HtmlString(renderedComponent);
    }

    public static IHtmlString InitJavascript(this HtmlHelper<dynamic> htmlHelper)
    {
        var reactService = htmlHelper.ViewContext.HttpContext.GetRequiredService<IReactService>();

        return new HtmlString(reactService.GetInitJavascript());
    }

#endif

#if NET6_0_OR_GREATER
    public static async Task<IHtmlContent> ReactAsync<T>(this IHtmlHelper htmlHelper, string componentName, T props)
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return new HtmlString(await reactService.RenderToStringAsync(componentName, props));
    }
    
    public static async Task<IHtmlContent> ReactAsync<TComponent>(this IHtmlHelper htmlHelper, TComponent component) where TComponent : IReactComponent
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return new HtmlString(await reactService.RenderToStringAsync(component.Path, null, component.ClientOnly));
    }
    
    public static async Task<IHtmlContent> ReactAsync<TComponent, TProps>(this IHtmlHelper htmlHelper, TComponent component) where TComponent : IReactComponent<TProps> where TProps : IReactComponentProps
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return new HtmlString(await reactService.RenderToStringAsync(component.Path, component.Props, component.ClientOnly));
    }

    public static IHtmlContent InitJavascript(this IHtmlHelper htmlHelper)
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return new HtmlString(reactService.GetInitJavascript());
    }
#endif
}
