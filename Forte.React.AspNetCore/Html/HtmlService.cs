using System.Threading.Tasks;
using Forte.React.AspNetCore.React;
using Microsoft.AspNetCore.Html;

namespace Forte.React.AspNetCore.Html;

internal interface IHtmlService
{
    Task<IHtmlContent> ReactAsync(string componentName, object props);
    IHtmlContent InitJavascript();
}

internal class HtmlService : IHtmlService
{
    private readonly IReactService _reactService;

    public HtmlService(IReactService reactService)
    {
        _reactService = reactService;
    }


    public async Task<IHtmlContent> ReactAsync(string componentName, object props)
    {
        var renderedComponent = await _reactService.RenderToStringAsync(componentName, props);

        return new HtmlString(renderedComponent);
    }

    public IHtmlContent InitJavascript()
    {
        var renderedComponent = _reactService.GetInitJavascript();

        return new HtmlString(renderedComponent);
    }
}
