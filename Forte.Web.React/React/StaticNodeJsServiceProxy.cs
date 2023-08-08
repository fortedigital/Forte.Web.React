#if NET48
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;

namespace Forte.Web.React.React;

public sealed class StaticNodeJsServiceProxy : INodeJSService
{
    public void Dispose()
    {
        StaticNodeJSService.DisposeServiceProvider();
    }

    public Task<T?> InvokeFromFileAsync<T>(string modulePath, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromFileAsync<T>(modulePath, exportName, args, cancellationToken);
    }

    public Task InvokeFromFileAsync(string modulePath, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromFileAsync(modulePath, exportName, args, cancellationToken);
    }

    public Task<T?> InvokeFromStringAsync<T>(string moduleString, string? cacheIdentifier = null, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromStringAsync<T>(moduleString, cacheIdentifier, exportName, args, cancellationToken);
    }

    public Task InvokeFromStringAsync(string moduleString, string? cacheIdentifier = null, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromStringAsync(moduleString, cacheIdentifier, exportName, args, cancellationToken);
    }

    public Task<T?> InvokeFromStringAsync<T>(Func<string> moduleFactory, string cacheIdentifier, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromStringAsync<T>(moduleFactory, cacheIdentifier, exportName, args, cancellationToken);
    }

    public Task InvokeFromStringAsync(Func<string> moduleFactory, string cacheIdentifier, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromStringAsync(moduleFactory, cacheIdentifier, exportName, args, cancellationToken);
    }

    public Task<T?> InvokeFromStreamAsync<T>(Stream moduleStream, string? cacheIdentifier = null, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromStreamAsync<T>(moduleStream, cacheIdentifier, exportName, args, cancellationToken);
    }

    public Task InvokeFromStreamAsync(Stream moduleStream, string? cacheIdentifier = null, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromStreamAsync(moduleStream, cacheIdentifier, exportName, args, cancellationToken);
    }

    public Task<T?> InvokeFromStreamAsync<T>(Func<Stream> moduleFactory, string cacheIdentifier, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromStreamAsync<T>(moduleFactory, cacheIdentifier, exportName, args, cancellationToken);
    }

    public Task InvokeFromStreamAsync(Func<Stream> moduleFactory, string cacheIdentifier, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.InvokeFromStreamAsync(moduleFactory, cacheIdentifier, exportName, args, cancellationToken);
    }

    public Task<(bool, T?)> TryInvokeFromCacheAsync<T>(string cacheIdentifier, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.TryInvokeFromCacheAsync<T>(cacheIdentifier, exportName, args, cancellationToken);
    }

    public Task<bool> TryInvokeFromCacheAsync(string cacheIdentifier, string? exportName = null, object?[]? args = null,
        CancellationToken cancellationToken = new())
    {
        return StaticNodeJSService.TryInvokeFromCacheAsync(cacheIdentifier, exportName, args, cancellationToken);
    }

    public ValueTask MoveToNewProcessAsync()
    {
        return StaticNodeJSService.MoveToNewProcessAsync();
    }
}
#endif
