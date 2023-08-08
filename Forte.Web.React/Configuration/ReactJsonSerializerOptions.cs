using System.Text.Json;

namespace Forte.Web.React.Configuration;

public class ReactJsonSerializerOptions
{
    public JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
