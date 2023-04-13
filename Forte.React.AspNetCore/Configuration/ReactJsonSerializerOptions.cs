using System.Text.Json;

namespace Forte.React.AspNetCore.Configuration;

public class ReactJsonSerializerOptions
{
    public JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
