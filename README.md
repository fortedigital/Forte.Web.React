# ForteReact

Library to render React library components on the server-side with C# as well as on the client.
Library is a wrapper for [Javascript.NodeJS](https://github.com/JeringTech/Javascript.NodeJS)

## Usage

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