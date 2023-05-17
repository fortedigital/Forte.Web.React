using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Forte.React.AspNetCore.Configuration;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;

namespace Forte.React.AspNetCore.React;

public interface IReactService
{
    Task<string> RenderToStringAsync(string componentName, object? props = null, RenderingMode renderingMode = RenderingMode.ClientAndServer);

    Task WriteOutputHtmlToAsync(TextWriter writer, string componentName, object? props = null,
        WriteOutputHtmlToOptions? writeOutputHtmlToOptions = null);

    string GetInitJavascript();
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

        var currentAssembly = typeof(ReactService).Assembly;
        var renderToStringScriptManifestName = currentAssembly.GetManifestResourceNames()
            .Single(s => s == $"Forte.React.AspNetCore.{nodeJsScriptName}");

        Stream ModuleFactory()
        {
            return currentAssembly.GetManifestResourceStream(renderToStringScriptManifestName) ??
                   throw new InvalidOperationException(
                       $"Can not get manifest resource stream with name - {renderToStringScriptManifestName}");
        }

        using var stream = ModuleFactory();
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

    public async Task WriteOutputHtmlToAsync(TextWriter writer, string componentName, object? props = null,
        WriteOutputHtmlToOptions? writeOutputHtmlToOptions = null)
    {
        var component = new Component(componentName, props);
        Components.Add(component);

        await writer.WriteAsync($"<div id=\"{component.ContainerId}\">").ConfigureAwait(false);

        if (_config.IsServerSideDisabled)
        {
            return;
        }

        var result = await InvokeRenderTo<HttpResponseMessage>(component, props,
            writeOutputHtmlToOptions ?? new WriteOutputHtmlToOptions()).ConfigureAwait(false);

        using var reader = new StreamReader(await result.Content.ReadAsStreamAsync().ConfigureAwait(false));

        char[] buffer = new char[1024];
        int numChars;

        while ((numChars = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
        {
            await writer.WriteAsync(buffer, 0, numChars).ConfigureAwait(false);
        }

        await writer.WriteAsync("</div>").ConfigureAwait(false);
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
        var shouldHydrate = !_config.IsServerSideDisabled && c.RenderingMode.HasFlag(RenderingMode.Server);
        return shouldHydrate ? Hydrate(c) : Render(c);
    }

    private static bool IsInitRequired(Component c) => c.RenderingMode.HasFlag(RenderingMode.Client);

    private static string GetElementById(string containerId) => $"document.getElementById(\"{containerId}\")";

    private string CreateElement(Component component)
    {
        var element = $"React.createElement({component.Path}, window.{_config.NameOfObjectToSaveProps}[\"{component.JsonContainerId}\"])";

        return _config.StrictMode ? $"React.createElement(React.StrictMode, null, {element})" : element;
    }

    private string Render(Component component)
    {
        var bootstrapScript = $"(window.{_config.NameOfObjectToSaveProps} = window.{_config.NameOfObjectToSaveProps} || {{}})[\"{component.JsonContainerId}\"] = {_jsonService.Serialize(component.Props)};";

        return bootstrapScript + (_config.ReactVersion.Major < 18
            ? $"ReactDOM.render({CreateElement(component)}, {GetElementById(component.ContainerId)}, {{ identifierPrefix: '{component.ContainerId}' }});"
            : $"ReactDOMClient.createRoot({GetElementById(component.ContainerId)}).render({CreateElement(component)}, {{ identifierPrefix: '{component.ContainerId}' }});");
    }

    private string Hydrate(Component component)
    {
        return _config.ReactVersion.Major < 18
            ? $"ReactDOM.hydrate({CreateElement(component)}, {GetElementById(component.ContainerId)}, {{ identifierPrefix: '{component.ContainerId}' }});"
            : $"ReactDOMClient.hydrateRoot({GetElementById(component.ContainerId)}, {CreateElement(component)}, {{ identifierPrefix: '{component.ContainerId}' }});";
    }
}

public class WriteOutputHtmlToOptions
{
    public WriteOutputHtmlToOptions(bool serverOnly = false, bool enableStreaming = true)
    {
        this.ServerOnly = serverOnly;
        this.EnableStreaming = enableStreaming;
    }

    public bool ServerOnly { get; }
    public bool EnableStreaming { get; }
}
