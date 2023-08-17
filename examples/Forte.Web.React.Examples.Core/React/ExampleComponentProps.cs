using System.Text.Json.Serialization;
using Forte.Web.React.React;

namespace Forte.Web.React.Examples.Core.React
{
    public class ExampleComponentProps : IReactComponentProps
    {
        [JsonPropertyName("initCount")]
        public int InitCount { get; set; }
        
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}