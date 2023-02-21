# ForteReact

Library to render React library components on the server-side with C# as well as on the client.
Library is a wrapper for [Javascript.NodeJS](https://github.com/JeringTech/Javascript.NodeJS)

## Usage

### 1. Add WebpackOptions to appsettings

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
services.AddReact(_configuration, _webHostingEnvironment.ContentRootPath, options => {...})
```
```
app.UseReact(FilesToInclude, new Version("x.x.x"));
```
