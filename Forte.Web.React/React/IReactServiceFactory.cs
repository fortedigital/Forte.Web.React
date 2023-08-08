using System;

namespace Forte.Web.React.React;

public interface IReactServiceFactory
{
    IReactService Create(IServiceProvider serviceProvider);
}
