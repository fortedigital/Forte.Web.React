
const callbackPipe = (callback, pipe, error) => {
  callback(error, null, (res) => {
    pipe(res);
    return true;
  });
};

module.exports = (
  callback,
  componentName,
  jsonContainerId,
  props = {},
  scriptFiles,
  objectToSavePropsName,
  options,
) => {
  try {
    scriptFiles.forEach((scriptFile) => {
      require(scriptFile);
    });

    const ReactDOMServer = global["ReactDOMServer"];
    const React = global["React"];

    const component = global[componentName];

    if (options.serverOnly) {
      const res = ReactDOMServer.renderToStaticNodeStream(
        React.createElement(
          component,
          props
        )
      );

      callback(null, res);
      return;
    }

    let error;
    const bootstrapScriptContent = `(window.${objectToSavePropsName} = window.${objectToSavePropsName} || {})['${jsonContainerId}'] = ${JSON.stringify(
      props
    )};`;

    const { pipe } = ReactDOMServer.renderToPipeableStream(
      React.createElement(
        component,
        props
      ),
      {
        bootstrapScriptContent: bootstrapScriptContent,
        onShellReady() {
          if (options.enableStreaming) {
            callbackPipe(callback, pipe, error);
          }
        },
        onShellError(error) {
          callback(error, null);
        },
        onAllReady() {
          if (!options.enableStreaming) {
            callbackPipe(callback, pipe, error);
          }
        },
        onError(err) {
          error = err;
          console.error(err);
        },
      }

    );
  } catch (err) {
    callback(err, null);
  }
  // const componentHtml = `${ReactDOMServer.renderToString(element)}`;
  // const jsonHtml = `<script id="${jsonContainerId}" type="json">${JSON.stringify(
  //   props
  // )}</script>`;
  // const result = componentHtml + jsonHtml;
};
