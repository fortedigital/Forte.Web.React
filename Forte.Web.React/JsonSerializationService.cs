using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;

namespace Forte.Web.React;

public interface IJsonSerializationService : IJsonService
{
    string Serialize<TValue>(TValue value);
}

internal class JsonSerializationService : IJsonSerializationService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonSerializationService(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public ValueTask<T?> DeserializeAsync<T>(Stream stream,
        CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions, cancellationToken);

    public Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
        => JsonSerializer.SerializeAsync(stream, value, _jsonSerializerOptions, cancellationToken);

    public string Serialize<TValue>(TValue value)
        => JsonSerializer.Serialize(value, _jsonSerializerOptions);
}
