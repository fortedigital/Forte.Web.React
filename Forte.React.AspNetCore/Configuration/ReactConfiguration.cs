using System;
using System.Collections.Generic;

namespace Forte.React.AspNetCore.Configuration;

internal class ReactConfiguration
{
    public List<string> ScriptUrls { get; set; } = new();
    public bool IsServerSideDisabled { get; set; } = false;
    public Version ReactVersion { get; set; } = null!;
}
