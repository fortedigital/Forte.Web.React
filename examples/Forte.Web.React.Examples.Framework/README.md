# Project Structure

This project is a demo of `Forte.Web.React` library hosted in ASP.NET MVC 5 application. It consists of a minimum code necessary to render a React component from a web application using SSR (Server Sider Rendering), CSR (Client Side Rendering) or both.

- `App_Start`  
Routing and static assets configuration. Check `ReactAssets` and `BundleConfig` to understand how to include React assets to the application.
- `Client`  
React application built with Vite. It contains one component - `src/Example.tsx` and all necessary configuration in `main.tsx` .
- `Controllers`
Single controller serving the example.
- `React`
C# models for `Example` component.
- `Views`
Single view rendering the example.
- `Global.asax`
Single point of all necessary web application configuration. Check this file to understand what dependencies are necessary.

# How to run this app?

## Prerequisites
- Node version 16+
- .NET Framework 4.8
- IIS Express

## Build

1. Build frontend app using the following commands:
```shell
cd Client
yarn install
yarn run build
```
2. Run ASP.NET application
3. Go to https://localhost:44300

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
## Configuring React for Server-side Rendering (SSR)
**Configure React**  
Set up React to enable server-side rendering and client-side usage. The following example demonstrates how to configure React:
```csharp
ReactConfiguration config = new ReactConfiguration
{
    ReactVersion = new Version(18, 2, 0),               // Specify your preferred React version
    ScriptUrls = new List<string> { GetReactJsPath() }, // Provide absolute paths to required scripts
    StrictMode = true,                                  // Choose whether React should render in strict mode
    UseCache = true,                                    // Decide whether to cache rendered components
    IsServerSideDisabled = false,                       // Globally enable or disable SSR
    NameOfObjectToSaveProps = "__reactProps",           // Optionally override the default object name for client-side props
};
```
## Setting Up Dependency Resolution
**Setup Dependency Resolver**  
If you're utilizing a dependency resolver like Autofac, follow these steps to integrate React into your application:
```csharp
// Register React configuration as a singleton ()
builder.RegisterInstance(new ReactConfiguration()).SingleInstance();

// Register Node.js service proxy (a workaround for ASP.NET 5)
builder.RegisterType<StaticNodeJsServiceProxy>().As<INodeJSService>().SingleInstance();

// Register ReactService with a new instance for each request
builder.RegisterType<ReactService>().As<IReactService>().InstancePerRequest();          
```
## Including Assets in Your Layout
Ensure that you incorporate all the required stylesheets and scripts for your React components. You can accomplish this using bundles or virtual paths.

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
@(Html.React<ExampleComponent, ExampleComponentProps>(new ExampleComponent { Props = Model, RenderingMode = RenderingMode.ClientAndServer }))
```