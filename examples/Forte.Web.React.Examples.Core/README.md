# Project Structure

This project is a demo of `Forte.Web.React` library hosted in ASP.NET Core application. It consists of a minimum code necessary to render a React component from a web application using SSR (Server Sider Rendering), CSR (Client Side Rendering) or both.

- `wwwroot\Client`  
React application built with Vite. It contains one component - `src/Example.tsx` and all necessary configuration in `main.tsx` .
- `React`
C# models for `Example` component.
- `Pages`
Single page rendering the example.
- `Program.cs`
Single point of all necessary web application configuration. Check this file to understand what dependencies are necessary.

# How to run this app?

## Prerequisites
- Node version 16+
- .NET Core 8.0+

## Build

1. Build frontend app using the following commands:
```shell
cd wwwroot/Client
yarn install
yarn run build
```
2. Run ASP.NET Core application
3. Go to https://localhost:7193/

# How to adapt it to your application?

## Setting up a React Application with Global Exposure
1. **Create a React Application**  
   Start by creating a React application using your preferred tools.
2. **Create a Component**  
   Design and develop your React component, which will encapsulate a specific piece of your application's UI logic.
3. **Expose React Globally**  
   To make React and ReactDOM accessible throughout your application, expose them in the global object. This enables Node services to utilize them from its process. Here's an example of how to achieve this:
```js
globalObject.React = React;
globalObject.ReactDOMClient = ReactDOM;
globalObject.ReactDOMServer = ReactDOMServer;
```
4. **Expose Your Component**  
To ensure your component is globally available, expose it as a property within an object. For instance:
```js
globalObject["__react"] = { Example };
```
## Setting Up Dependency Resolution
**Add React to Services**  
Follow these steps to integrate React into your application:
```csharp
builder.Services.AddReact(nodeOptions =>
{
    // optional configuration transformations
}, nodeServiceOptions =>
{
    // optional configuration transformations
});        
```

## Configuring React for Server-side Rendering (SSR)
**Configure React**  
Set up React to enable server-side rendering and client-side usage. The following example demonstrates how to configure React:
```csharp
var dir = app.Environment.WebRootPath;
var js = Directory.GetFiles(Path.Combine(dir, "Client/dist/assets")).First(f => f.EndsWith(".js"));
app.UseReact(new[] { js }, new Version(18, 2, 0), strictMode: true);
```

## Including Assets in Your Layout
Ensure that you incorporate all the required stylesheets and scripts for your React components. For example:
```html
<head>
    <link rel="stylesheet" asp-href-include="/Client/dist/assets/index-*.css"/>
    // ... 
 </head>
<body>
...
<script type="text/javascript" asp-src-include="/Client/dist/assets/index-*.js"></script>
</body>
```

## Defining React Components
Establish your React components and their associated props. The subsequent example illustrates how to define a component and its props:
```csharp
    public class ExampleComponent : IReactComponent<ExampleComponentProps>
    {
        public string Path => "Example";        
        public RenderingMode RenderingMode { get; set; }
        public ExampleComponentProps Props { get; set; }
    }
    
    public class ExampleComponentProps : IReactComponentProps
    {
        public int InitCount { get; set; }
        public string Text { get; set; }
    }
```
## Rendering Components in Views
To render your React component in a Razor view, make use of a helper method like this:
```csharp
@await Html.ReactAsync(new ExampleComponent { Props = Model.Props, RenderingMode = RenderingMode.ClientAndServer })
```
Remember to initialize components using:
```csharp
@section scripts
{
    @Html.InitJavascript()
}
```