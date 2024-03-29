﻿#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Forte.Web.React.Configuration;
using Forte.Web.React.React;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Forte.Web.React;

public static class ReactForteExtensions
{
    public static void AddReact(this IServiceCollection services,
        Action<NodeJSProcessOptions> configureNodeJs,
        Action<OutOfProcessNodeJSServiceOptions> configureOutOfProcessNodeJs,
        IReactServiceFactory? reactServiceFactory = null,
        Action<JsonSerializerOptions>? configureJsonSerializerOptions = null)
    {
        services.AddNodeJS();
        services.AddSingleton<IJsonService, JsonSerializationService>(
            sp => new JsonSerializationService(sp.GetRequiredService<IOptions<ReactJsonSerializerOptions>>().Value
                .Options));
        services.AddSingleton<IJsonSerializationService, JsonSerializationService>(
            sp => new JsonSerializationService(sp.GetRequiredService<IOptions<ReactJsonSerializerOptions>>().Value
                .Options));

        services.Configure<NodeJSProcessOptions>(options => { configureNodeJs?.Invoke(options); });
        services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
        {
            configureOutOfProcessNodeJs?.Invoke(options);
        });
        services.Configure<ReactJsonSerializerOptions>(options =>
            configureJsonSerializerOptions?.Invoke(options.Options));

        services.AddSingleton<ReactConfiguration>();
        if (reactServiceFactory == null)
        {
            services.AddScoped(ReactService.Create);
        }
        else
        {
            services.AddScoped(reactServiceFactory.Create);
        }
    }

    public static void UseReact(this IApplicationBuilder app, IEnumerable<string> scriptUrls, Version reactVersion,
        bool disableServerSideRendering = false, string? nameOfObjectToSaveProps = null, bool? useCache = null, bool? strictMode = null)
    {
        var config = app.ApplicationServices.GetService<ReactConfiguration>();

        if (config is null)
        {
            throw new InvalidOperationException("Remember to add AddReact to your code");
        }

        config.IsServerSideDisabled = disableServerSideRendering;
        config.ScriptUrls = scriptUrls.ToList();
        config.ReactVersion = reactVersion;
        config.NameOfObjectToSaveProps = nameOfObjectToSaveProps ?? config.NameOfObjectToSaveProps;
        config.UseCache = useCache ?? true;
        config.StrictMode = strictMode ?? false;
    }
}
#endif
