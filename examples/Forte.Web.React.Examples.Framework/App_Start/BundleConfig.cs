using System.IO;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Forte.Web.React.Examples.Framework
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterReactBundle(bundles);
        }

        private static void RegisterReactBundle(BundleCollection bundles)
        {
            var jsBundle = new ScriptBundle("~/bundles/React")
                .Include(ReactAssets.GetReactJsVirtualPath());
            
            var cssBundle = new StyleBundle("~/Content/React")
                .Include(ReactAssets.GetReactCssVirtualPath());
            
            // React Assets are already optimized and minified by bundling tool such as Vite or Webpack.
            jsBundle.Transforms.Clear();
            cssBundle.Transforms.Clear();
            
            bundles.Add(jsBundle);
            bundles.Add(cssBundle);
        }
    }
}