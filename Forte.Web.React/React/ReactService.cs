using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Forte.Web.React.Configuration;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;

namespace Forte.Web.React.React;

public interface IReactService
{
    string GetInitJavascript();
    Task<IReadOnlyCollection<string>> GetAvailableComponentNames();

    Task RenderAsync(TextWriter writer, string componentName, object? props = null, RenderOptions? options = null);
    Task<string> RenderToStringAsync(string componentName, object? props = null, RenderingMode renderingMode = RenderingMode.ClientAndServer);
}

public class ReactService : IReactService
{
    private List<Component> Components { get; } = new();

    private readonly INodeJSService _nodeJsService;
    private readonly ReactConfiguration _config;
    private readonly IJsonSerializationService _jsonService;

    private static readonly Dictionary<Type, string> MethodToNodeJsScriptName = new()
    {
        { typeof(HttpResponseMessage), "renderToPipeableStream.js" }, { typeof(string), "renderToString.js" }
    };

#if NET6_0_OR_GREATER
    public static IReactService Create(IServiceProvider serviceProvider)
    {
        return new ReactService(
            serviceProvider.GetRequiredService<INodeJSService>(),
            serviceProvider.GetRequiredService<ReactConfiguration>(),
            serviceProvider.GetRequiredService<IJsonSerializationService>());
    }

    private ReactService(INodeJSService nodeJsService, ReactConfiguration config, IJsonSerializationService jsonService)
    {
        _nodeJsService = nodeJsService;
        _config = config;
        _jsonService = jsonService;
    }
#endif

#if NET48
    public ReactService(INodeJSService nodeJsService, ReactConfiguration config)
    {
        _nodeJsService = nodeJsService;
        _config = config;
        _jsonService = new JsonSerializationService(new ReactJsonSerializerOptions().Options);
    }

    public ReactService(INodeJSService nodeJsService, IJsonSerializationService jsonService, ReactConfiguration config)
    {
        _nodeJsService = nodeJsService;
        _config = config;
        _jsonService = jsonService;
    }
#endif

    private async Task<T> InvokeRenderTo<T>(Component component, object? props = null, params object[] args)
    {
        var allArgs = new List<object>()
        {
            component.Path,
            component.JsonContainerId,
            props,
            _config.ScriptUrls,
            _config.NameOfObjectToSaveProps,
        };
        allArgs.AddRange(args);

        var type = typeof(T);
        var nodeJsScriptName = MethodToNodeJsScriptName[type];

        if (_config.UseCache)
        {
            var (success, cachedResult) = await _nodeJsService
                .TryInvokeFromCacheAsync<T>(nodeJsScriptName, args: allArgs.ToArray())
                .ConfigureAwait(false);

            if (success)
            {
                return cachedResult!;
            }
        }

        using var stream = GetStreamFromEmbeddedScript(nodeJsScriptName);

        var result = await _nodeJsService.InvokeFromStreamAsync<T>(stream,
                nodeJsScriptName,
                args: allArgs.ToArray())
            .ConfigureAwait(false);

        return result!;
    }


    public async Task<string> RenderToStringAsync(string componentName, object? props = null, RenderingMode renderingMode = RenderingMode.ClientAndServer)
    {
        var component = new Component(componentName, props, renderingMode);
        Components.Add(component);

        if (_config.IsServerSideDisabled || renderingMode == RenderingMode.Client)
        {
            return WrapRenderedStringComponent(string.Empty, component);
        }

        var result = await InvokeRenderTo<string>(component, props).ConfigureAwait(false);

        return WrapRenderedStringComponent(result, component);
    }

    public async Task RenderAsync(TextWriter writer, string componentName, object? props = null, RenderOptions? options = null)
    {
        options ??= new RenderOptions();
        var component = new Component(componentName, props, options.RenderingMode);
        Components.Add(component);

        await writer.WriteAsync($"<div id=\"{component.ContainerId}\">").ConfigureAwait(false);

        if (_config.IsServerSideDisabled || component.RenderingMode == RenderingMode.Client)
        {
            await writer.WriteAsync("</div>").ConfigureAwait(false);
            return;
        }

        var streamingOptions = new
        {
            options.EnableStreaming,
            ServerOnly = component.RenderingMode == RenderingMode.Server,
            IdentifierPrefix = _config.UseIdentifierPrefix ? component.ContainerId : null,
        };

        var result = await InvokeRenderTo<HttpResponseMessage>(component, props, streamingOptions).ConfigureAwait(false);

        using var reader = new StreamReader(await result.Content.ReadAsStreamAsync().ConfigureAwait(false));

        char[] buffer = new char[1024];
        int numChars;

        while ((numChars = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
        {
            await writer.WriteAsync(buffer, 0, numChars).ConfigureAwait(false);
        }

        await writer.WriteAsync("</div>").ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<string>> GetAvailableComponentNames()
    {
        const string getAvailableComponentNames = "getAvailableComponentNames.js";

        if (_config.UseCache)
        {
            var (success, cachedResult) = await _nodeJsService
                .TryInvokeFromCacheAsync<string[]>(getAvailableComponentNames, args: new [] { _config.ScriptUrls })
                .ConfigureAwait(false);

            if (success)
            {
                return cachedResult!;
            }
        }

        using var stream = GetStreamFromEmbeddedScript(getAvailableComponentNames);

        var result = await _nodeJsService.InvokeFromStreamAsync<string[]>(stream,
                getAvailableComponentNames, args: new [] { _config.ScriptUrls })
            .ConfigureAwait(false);

        return result!;
    }

    private static Stream GetStreamFromEmbeddedScript(string scriptName)
    {
        var currentAssembly = typeof(ReactService).Assembly;

        var manifestResourceName = $"Forte.Web.React.Scripts.{scriptName}";
        var stream = currentAssembly.GetManifestResourceStream(manifestResourceName) ??
                     throw new InvalidOperationException($"Could not get manifest resource with name - {manifestResourceName}");

        return stream;
    }

    private static string WrapRenderedStringComponent(string? renderedStringComponent, Component component)
    {
        if (renderedStringComponent is null)
        {
            throw new ArgumentNullException(nameof(renderedStringComponent));
        }

        return $"<div id=\"{component.ContainerId}\">{renderedStringComponent}</div>";
    }

    public string GetInitJavascript()
    {
        var componentInitiation = Components.Where(IsInitRequired).Select(GetInitJavascriptSource);

        return $"<script>{string.Join("", componentInitiation)}</script>";
    }

    private string GetInitJavascriptSource(Component c)
    {
        var shouldHydrate = !_config.IsServerSideDisabled && c.RenderingMode == RenderingMode.ClientAndServer;
        return shouldHydrate ? Hydrate(c) : Render(c);
    }

    private static bool IsInitRequired(Component c) => c.RenderingMode.HasFlag(RenderingMode.Client);

    private static string GetElementById(string containerId) => $"document.getElementById(\"{containerId}\")";

    private string CreateElement(Component component)
    {
        var element = $"React.createElement(window.__react.{component.Path}, window.{_config.NameOfObjectToSaveProps}[\"{component.JsonContainerId}\"])";

        return _config.StrictMode ? $"React.createElement(React.StrictMode, null, {element})" : element;
    }

    private string Render(Component component)
    {
        var bootstrapScript =
            $"(window.{_config.NameOfObjectToSaveProps} = window.{_config.NameOfObjectToSaveProps} || {{}})[\"{component.JsonContainerId}\"] = {_jsonService.Serialize(component.Props)};";

        var elementById = GetElementById(component.ContainerId);
        var element = CreateElement(component);
        var options = GetIdentifierPrefix(component);

        return bootstrapScript + (_config.ReactVersion.Major < 18
            ? $"ReactDOM.render({element}, {elementById});"
            : $"ReactDOMClient.createRoot({elementById}{options}).render({element});");
    }

    private string Hydrate(Component component)
    {
        var elementById = GetElementById(component.ContainerId);
        var element = CreateElement(component);
        var options = GetIdentifierPrefix(component);

        return _config.ReactVersion.Major < 18
            ? $"ReactDOM.hydrate({element}, {elementById});"
            : $"ReactDOMClient.hydrateRoot({elementById}, {element}{options});";
    }

    private string GetIdentifierPrefix(Component component) => _config.UseIdentifierPrefix
        ? $", {{ identifierPrefix: '{component.ContainerId}' }}"
        : string.Empty;
}

public class RenderOptions
{
    public RenderOptions() : this(RenderingMode.ClientAndServer, true)
    {
    }
    
    public RenderOptions(bool serverOnly, bool enableStreaming = true) : this(serverOnly ? RenderingMode.Server : RenderingMode.ClientAndServer, enableStreaming)
    {
    }
    
    public RenderOptions(RenderingMode renderingMode, bool enableStreaming = true)
    {
        RenderingMode = renderingMode;
        EnableStreaming = enableStreaming;
    }

    public RenderingMode RenderingMode { get; }
    public bool EnableStreaming { get; }
}
