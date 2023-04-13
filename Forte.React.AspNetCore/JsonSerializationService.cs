using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;

namespace Forte.React.AspNetCore;

internal class JsonSerializationService : IJsonService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonSerializationService(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public ValueTask<T?> DeserializeAsync<T>(Stream stream,
        CancellationToken cancellationToken = new CancellationToken())
        => JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions, cancellationToken);

    public Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = new CancellationToken())
        => JsonSerializer.SerializeAsync(stream, value, _jsonSerializerOptions, cancellationToken);
}
