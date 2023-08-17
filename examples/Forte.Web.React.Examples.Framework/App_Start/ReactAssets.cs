using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Forte.Web.React.Configuration;

namespace Forte.Web.React.Examples.Framework
{
    public static class ReactAssets
    {
        private const string ClientVirtualPath = "~/Client/dist/assets";

        public static readonly ReactConfiguration ReactConfig = new ReactConfiguration
        {
            ReactVersion = new Version(18, 2, 0),
            ScriptUrls = new List<string> { GetReactJsPath() },
            StrictMode = true,
            UseCache = true,
            IsServerSideDisabled = false,
            NameOfObjectToSaveProps = "__reactProps",
        };
        
        public static string GetReactJsVirtualPath()
        {
            return Path.Combine(GetReactVirtualDirectory(), Path.GetFileName(GetReactJsPath()));
        }
        
        public static string GetReactCssVirtualPath()
        {
            return Path.Combine(GetReactVirtualDirectory(), Path.GetFileName(GetReactCssPath()));
        }

        private static string GetReactJsPath()
        {
            var distPath = GetReactDirectory();
            return Directory.GetFiles(distPath).First(f => f.EndsWith(".js"));
        }

        private static string GetReactCssPath()
        {
            var distPath = GetReactDirectory();
            return Directory.GetFiles(distPath).First(f => f.EndsWith(".css"));
        }

        private static string GetReactDirectory()
        {
            return HostingEnvironment.MapPath(ClientVirtualPath) ?? throw new FileNotFoundException("Build client assets first.");
        }

        private static string GetReactVirtualDirectory()
        {
            var virtualRoot = HostingEnvironment.MapPath("~") ?? throw new InvalidOperationException("Hosting environment is not working properly.");
            return GetReactDirectory().Replace(virtualRoot, "~/");
        }
    }
}