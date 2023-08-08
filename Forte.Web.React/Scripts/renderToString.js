module.exports = (
    callback,
    componentName,
    jsonContainerId,
    props = {},
    scriptFiles,
    nameOfObjectToSaveProps
) => {
    scriptFiles.forEach((scriptFile) => {
        require(scriptFile);
    });

    const ReactDOMServer = global["ReactDOMServer"];
    const React = global["React"];
    const componentRepository = global["__react"] || {};

    const path = componentName.split(".");
    let component = componentRepository[path[0]];

    for (let segment = 1; segment < path.length; segment++) {
        component = component[path[segment]];
    }

    const element = React.createElement(component, props);

    const componentHtml = `${ReactDOMServer.renderToString(element)}`;
    const bootstrapScriptContent = `(window.${nameOfObjectToSaveProps} = window.${nameOfObjectToSaveProps} || {})['${jsonContainerId}'] = ${JSON.stringify(
        props
    )};`;
    const jsonHtml = `<script>${bootstrapScriptContent}</script>`;
    const result = componentHtml + jsonHtml;

    callback(null /* error */, result /* result */);
};
