using System;

namespace Forte.React.AspNetCore.React;

[Flags]
public enum RenderingMode
{
    /// <summary>
    /// Component will only be rendered on the client.
    /// </summary>
    Client = 1,
    
    /// <summary>
    /// Component will only be rendered as static markup on the server without hydration on the client.
    /// </summary>
    Server = 2,
    
    /// <summary>
    /// Component will be rendered on the server and hydrated on the client.
    /// </summary>
    ClientAndServer = Client | Server,
}
