using System.Web.Mvc;
using Forte.Web.React.Examples.Framework.React;

namespace Forte.Web.React.Examples.Framework.Controllers
{
    public class ExampleController : Controller
    {
        public ActionResult Index(int initCount = 0, string text = null)
        {
            return View(new ExampleComponentProps
            {
                InitCount = initCount,
                Text = text ?? "Use query parameters 'initCount' and 'text' to change values in the React component",
            });
        }
    }
}