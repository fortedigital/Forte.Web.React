module.exports = (
  callback,
  componentName,
  jsonContainerId,
  props = {},
  scriptFiles
) => {
  scriptFiles.forEach((scriptFile) => {
    require(scriptFile);
  });
  const component = global[componentName];

  const ReactDOMServer = global["ReactDOMServer"];
  const React = global["React"];
  const element = React.createElement(component, props);

  const componentHtml = `${ReactDOMServer.renderToString(element)}`;
  const jsonHtml = `<script id="${jsonContainerId}" type="json">${JSON.stringify(
    props
  )}</script>`;
  const result = componentHtml + jsonHtml;

  callback(null /* error */, result /* result */);
};
