﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Forte.React.AspNetCore.Configuration;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Forte.React.AspNetCore.React;

public interface IReactService
{
    Task<string> RenderToStringAsync(string componentName, object props);

    Task WriteOutputHtmlToAsync(TextWriter writer, string componentName, object props,
        WriteOutputHtmlToOptions? writeOutputHtmlToOptions = null);

    string GetInitJavascript();
}

public class ReactService : IReactService
{
    private List<Component> Components { get; } = new();

    private readonly INodeJSService _nodeJsService;
    private readonly ReactConfiguration _config;

    private static readonly Dictionary<Type, string> MethodToNodeJsScriptName = new()
    {
        { typeof(HttpResponseMessage), "renderToPipeableStream.js" },
        { typeof(string), "renderToString.js" }
    };

    public static IReactService Create(IServiceProvider serviceProvider)
    {
        return new ReactService(
            serviceProvider.GetRequiredService<INodeJSService>(),
            serviceProvider.GetRequiredService<ReactConfiguration>());
    }

    private ReactService(INodeJSService nodeJsService, ReactConfiguration config)
    {
        _nodeJsService = nodeJsService;
        _config = config;
    }

    private async Task<T> ASD<T>(Component component, object props, params object[] args)
    {
        var allArgs = new List<object>() { component.Name, component.JsonContainerId, props, _config.ScriptUrls, _config.ObjectToSavePropsName };
        allArgs.AddRange(args);

        var type = typeof(T);
        var nodeJsScriptName = MethodToNodeJsScriptName[type];

        var (success, cachedResult) =
            await _nodeJsService.TryInvokeFromCacheAsync<T>(nodeJsScriptName, args: allArgs.ToArray());

        if (success)
        {
            return cachedResult!;
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

        await using var stream = ModuleFactory();
        var result = await _nodeJsService.InvokeFromStreamAsync<T>(stream,
            nodeJsScriptName,
            args: allArgs.ToArray());

        return result!;
    }

    public async Task<string> RenderToStringAsync(string componentName, object props)
    {
        var component = new Component(componentName, props);
        Components.Add(component);

        if (_config.IsServerSideDisabled)
        {
            return WrapRenderedStringComponent(string.Empty, component);
        }

        var result = await ASD<string>(component, props);

        return WrapRenderedStringComponent(result, component);
    }

    public async Task WriteOutputHtmlToAsync(TextWriter writer, string componentName, object props,
        WriteOutputHtmlToOptions? writeOutputHtmlToOptions)
    {
        var component = new Component(componentName, props);
        Components.Add(component);

        await writer.WriteAsync($"<div id=\"{component.ContainerId}\">");

        if (_config.IsServerSideDisabled)
        {
            return;
        }

        var result = await ASD<HttpResponseMessage>(component, props,
            writeOutputHtmlToOptions ?? new WriteOutputHtmlToOptions());

        using var reader = new StreamReader(await result.Content.ReadAsStreamAsync());
        int character;

        while ((character = reader.Read()) != -1)
        {
            await writer.WriteAsync((char)character);
        }

        await writer.WriteAsync("</div>");
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
        Func<Component, string> initFunction = _config.IsServerSideDisabled ? Render : Hydrate;
        var componentInitiation = Components.Select(initFunction);

        return $"<script>{string.Join("", componentInitiation)}</script>";
    }

    private static string GetElementById(string containerId)
        => $"document.getElementById(\"{containerId}\")";


    private string CreateElement(Component component)
        =>
            $"React.createElement({component.Name}, window.{_config.ObjectToSavePropsName}[\"{component.JsonContainerId}\"])";


    private string Render(Component component)
    {
        return _config.ReactVersion.Major < 18
            ? $"ReactDOM.render({CreateElement(component)}, {GetElementById(component.ContainerId)});"
            : $"ReactDOMClient.createRoot({GetElementById(component.ContainerId)}).render({CreateElement(component)});";
    }

    private string Hydrate(Component component)
    {
        return _config.ReactVersion.Major < 18
            ? $"ReactDOM.hydrate({CreateElement(component)}, {GetElementById(component.ContainerId)});"
            : $"ReactDOMClient.hydrateRoot({GetElementById(component.ContainerId)}, {CreateElement(component)});";
    }
}

public record WriteOutputHtmlToOptions(bool ServerOnly = false, bool EnableStreaming = true);
