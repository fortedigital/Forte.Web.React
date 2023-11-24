using System;
using System.Collections.Generic;
using Forte.Web.React.React;

namespace Forte.Web.React.Configuration;

public class ReactConfiguration
{
    /// <summary>
    /// Collection of URLs pointing to scripts.
    /// </summary>
    public List<string> ScriptUrls { get; set; } = new();

    /// <summary>
    /// Indicates whether server-side rendering is globally disabled. Default value is "false".
    /// </summary>
    public bool IsServerSideDisabled { get; set; } = false;

    /// <summary>
    /// Version of React being used.
    /// </summary>
    public Version ReactVersion { get; set; } = null!;

    /// <summary>
    /// Name of the object used to save properties. Default value is "__reactProps".
    /// </summary>
    public string NameOfObjectToSaveProps { get; set; } = "__reactProps";

    /// <summary>
    /// Indicates whether caching is used. Default value is "true".
    /// <remarks>
    /// This property specifically controls the usage of an in-process library cache, distinct from the internal Node server cache.
    /// </remarks>
    /// </summary>
    public bool UseCache { get; set; } = true;

    /// <summary>
    /// Indicates whether strict mode is enabled. Default value is "false"
    /// </summary>
    public bool StrictMode { get; set; } = false;

    /// <summary>
    /// Ensures a unique identifier prefix for components on both client and server.
    /// It avoids conflicts when using multiple roots on the same page and enables the use of the `useId` hook without conflicts if set to `true`.
    /// Default value is "false".
    /// <remarks>IdentifierPrefix requires React in version 18 or higher and is not supported by <see cref="IReactService.RenderToStringAsync"/> method.</remarks>
    /// </summary>
    public bool UseIdentifierPrefix { get; set; } = false;
}
