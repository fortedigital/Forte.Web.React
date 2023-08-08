using System;
using System.Collections.Generic;

namespace Forte.Web.React.Configuration;

public class ReactConfiguration
{
    public List<string> ScriptUrls { get; set; } = new();
    public bool IsServerSideDisabled { get; set; } = false;
    public Version ReactVersion { get; set; } = null!;
    public string NameOfObjectToSaveProps { get; set; } = "__reactProps";
    public bool UseCache { get; set; } = true;
    public bool StrictMode { get; set; } = false;
}
