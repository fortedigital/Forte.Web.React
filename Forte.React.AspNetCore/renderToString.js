module.exports = (callback, componentName, props = {}, scriptFiles) => {
    scriptFiles.forEach((scriptFile) => { require(scriptFile) });

    const component = global[componentName];

    const ReactDOMServer = global["ReactDOMServer"];
    const React = global["React"];
    const element = React.createElement(component, props);

    const result = ReactDOMServer.renderToString(element);

    callback(null /* error */, result /* result */);
}
