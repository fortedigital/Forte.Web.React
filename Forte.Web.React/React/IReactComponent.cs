namespace Forte.Web.React.React;

public interface IReactComponent<out TProps> : IReactComponent where TProps : IReactComponentProps
{
    TProps Props { get; }
}

public interface IReactComponent
{
    string Path { get; }
    RenderingMode RenderingMode { get; }
}
