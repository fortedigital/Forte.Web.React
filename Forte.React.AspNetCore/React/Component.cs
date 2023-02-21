using System;

namespace Forte.React.AspNetCore.React;

internal class Component
{
    public string Name { get; }
    public object Props { get; }
    public string ContainerId { get; } = Guid.NewGuid().ToString("n")[..8];

    public Component(string name, object props)
    {
        Name = name;
        Props = props;
    }
}
