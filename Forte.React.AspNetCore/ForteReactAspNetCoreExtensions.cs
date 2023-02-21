using System;
using System.Collections.Generic;
using System.Linq;
using Forte.React.AspNetCore.Configuration;
using Forte.React.AspNetCore.Html;
using Forte.React.AspNetCore.React;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Forte.React.AspNetCore;

public static class ReactForteExtensions
{
    public static void AddReact(this IServiceCollection services,
        Action<NodeJSProcessOptions> configureNodeJs,
        Action<OutOfProcessNodeJSServiceOptions> configureOutOfProcessNodeJS,
        IReactServiceFactory? reactServiceFactory = null)
    {
        services.AddNodeJS();

        services.Configure<NodeJSProcessOptions>(options => { configureNodeJs?.Invoke(options); });
        services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
        {
            configureOutOfProcessNodeJS?.Invoke(options);
        });

        services.AddSingleton<ReactConfiguration>();
        if (reactServiceFactory == null)
        {
            services.AddScoped(ReactService.Create);
        }
        else
        {
            services.AddScoped(reactServiceFactory.Create);
        }
        services.AddScoped<IHtmlService, HtmlService>();
    }

    public static void UseReact(this IApplicationBuilder app, IEnumerable<string> scriptUrls, Version reactVersion,
        bool disableServerSideRendering = false)
    {
        var config = app.ApplicationServices.GetService<ReactConfiguration>();

        if (config is null)
        {
            throw new InvalidOperationException("Remember to add AddReact to your code");
        }

        config.IsServerSideDisabled = disableServerSideRendering;
        config.ScriptUrls = scriptUrls.ToList();
        config.ReactVersion = reactVersion;
    }
}
