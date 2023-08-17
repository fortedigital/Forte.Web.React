using Forte.Web.React.React;

namespace Forte.Web.React.Examples.Framework.React
{
    public class ExampleComponent : IReactComponent<ExampleComponentProps>
    {
        /// <summary>
        /// Path to component exposed in the global object by your React script.
        /// </summary>
        public string Path => "Example";
        
        public RenderingMode RenderingMode { get; set; }
        public ExampleComponentProps Props { get; set; }
    }
}