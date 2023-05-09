using System;
using System.Collections.Generic;

namespace Forte.React.AspNetCore.Configuration;

public class ReactConfiguration
{
    public List<string> ScriptUrls { get; set; } = new();
    public bool IsServerSideDisabled { get; set; } = false;
    public Version ReactVersion { get; set; } = null!;
    public string NameOfObjectToSaveProps { get; set; } = "__reactProps";
}
