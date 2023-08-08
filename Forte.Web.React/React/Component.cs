using System;

namespace Forte.Web.React.React;

internal class Component : IReactComponent
{
    public string Path { get; }
    public object? Props { get; }
    public string ContainerId { get; }
    public string JsonContainerId { get; }
    public RenderingMode RenderingMode { get; }

    public Component(string path, object? props = null, RenderingMode renderingMode = RenderingMode.ClientAndServer)
    {
        Path = path;
        Props = props;
        RenderingMode = renderingMode;
        ContainerId = Guid.NewGuid().ToString("n").Substring(0, 8);
        JsonContainerId = ContainerId + "-json";
    }
}
