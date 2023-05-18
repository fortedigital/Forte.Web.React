
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
  nameOfObjectToSaveProps,
  options,
) => {
  try {
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
    const bootstrapScriptContent = `(window.${nameOfObjectToSaveProps} = window.${nameOfObjectToSaveProps} || {})['${jsonContainerId}'] = ${JSON.stringify(
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
};
