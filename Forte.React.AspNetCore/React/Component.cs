using System;

namespace Forte.React.AspNetCore.React;

internal class Component
{

    public string Name { get; }
    public object Props { get; }
    public string ContainerId { get; }
    public string JsonContainerId { get; }
    public bool ClientOnly { get; }

    public Component(string name, object props, bool clientOnly = false)
    {
        Name = name;
        Props = props;
        ContainerId = Guid.NewGuid().ToString("n").Substring(0, 8);
        JsonContainerId = ContainerId + "-json";
    }
}
