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
  const component = global[componentName];

  const ReactDOMServer = global["ReactDOMServer"];
  const React = global["React"];
  const element = React.createElement(component, props);

  const componentHtml = `${ReactDOMServer.renderToString(element)}`;
  const bootstrapScriptContent = `(window.${nameOfObjectToSaveProps} = window.${nameOfObjectToSaveProps} || {})['${jsonContainerId}'] = ${JSON.stringify(
    props
  )};`;
  const jsonHtml = `<script>${bootstrapScriptContent}</script>`;
  const result = componentHtml + jsonHtml;

  callback(null /* error */, result /* result */);
};
