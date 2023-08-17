# Forte.Web.React

Unlock the power of Server Side rendered React components within your ASP.NET Core MVC or ASP.NET MVC applicaiton.
**Forte.Web.React** can render components on server only, client only, on both - using hydration.
This library uses an out-of-process Node Service using [Javascript.NodeJS](https://github.com/JeringTech/Javascript.NodeJS) as a C# proxy.

## Examples

- [ASP.NET Core Example](examples/Forte.Web.React.Examples.Core)
- [ASP.NET (Framework) Example](examples/Forte.Web.React.Examples.Framework)

## Usage (.Net Core)
For .Net Framework usage, check the example mentioned above.

### 1. Add WebpackOptions to `appsettings`

#### For Development 
```
  "Webpack": {
    "OutputPath": "http://localhost:8080"
  }
```

#### For Production
```
  "Webpack": {
    "OutputPath": "./path/to/output"
  }
```

### 2. Modify Startup.cs

```
  services.AddReact(nodeJsProcessOptions => {...}, configureOutOfProcessNodeJS => {...})
```
```
  app.UseReact(filesToInclude, new Version("x.x.x"));
```

#### Optional `ReactService` decoration
To give client code possibility to decorate with custom behavior rendering to string done by `React` to log or send some metrics optional parameter `reactServiceFactory` is exposed in `AddReact` method. Sample usage can look as follow:

```
  var customReactServiceFactory = new CustomReactServiceFactory();
  services.AddReact(
                nodeJsProcessOptions => { },
                configureOutOfProcessNodeJS => { },
                customReactServiceFactory);
```

```
  internal class CustomReactServiceFactory : IReactServiceFactory
  {
      public IReactService Create(IServiceProvider serviceProvider)
      {
          return new CustomReactServiceDecorator(ReactService.Create(serviceProvider));
      }
  }

  internal class CustomReactServiceDecorator : IReactService
  {
    private readonly IReactService _reactService;

    public CustomReactServiceDecorator(IReactService reactService)
    {
      _reactService = reactService;
    }

    public Task<string> RenderToStringAsync(string componentName, object props)
    {
      // decorator logic 
      return _reactService.RenderToStringAsync(componentName, props);
    }

    public string GetInitJavascript()
    {
      // decorator logic
      return _reactService.GetInitJavascript();
    }
  }
```