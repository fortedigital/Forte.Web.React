using System;

namespace Forte.React.AspNetCore.React;

public interface IReactServiceFactory
{
    IReactService Create(IServiceProvider serviceProvider);
}
