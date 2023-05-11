using System;

namespace Forte.React.AspNetCore.React;

internal class Component : IReactComponent
{
    public string Path { get; }
    public object? Props { get; }
    public string ContainerId { get; }
    public string JsonContainerId { get; }
    public bool ClientOnly { get; }

    public Component(string path, object? props = null, bool clientOnly = false)
    {
        Path = path;
        Props = props;
        ClientOnly = clientOnly;
        ContainerId = Guid.NewGuid().ToString("n").Substring(0, 8);
        JsonContainerId = ContainerId + "-json";
    }
}
