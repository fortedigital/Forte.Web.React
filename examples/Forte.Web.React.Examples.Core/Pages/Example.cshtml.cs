using Forte.Web.React.Examples.Core.React;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Forte.Web.React.Examples.Core.Pages;

public class ExampleModel : PageModel
{
    public ExampleComponentProps Props = new();

    public void OnGet(int initCount = 0, string? text = null)
    {
        Props = new ExampleComponentProps
        {
            InitCount = initCount,
            Text = text ?? "Use query parameters 'initCount' and 'text' to change values in the React component",
        };
    }
}