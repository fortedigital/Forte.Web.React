module.exports = (
    callback,
    scriptFiles
) => {
    scriptFiles.forEach((scriptFile) => {
        require(scriptFile);
    });

    const componentRepository = global["__react"];
    if (!componentRepository)
        throw Error("React component repository does not exist");
    
    const componentNames = getComponentNames("", componentRepository);
    
    callback(null /* error */, componentNames /* result */);
};

function getComponentNames(prefix, namespace) {
    let result = [];
    for(const propertyName in namespace) {
        if (isComponent(namespace[propertyName])) {
            result.push(prefix + propertyName);
        }
        else {
            result = result.concat(getComponentNames(prefix + propertyName + '.', namespace[propertyName]));
        }
    }
    return result;
}

function isComponent(o) {
    if (typeof o === "function")
        return true;
    if (typeof o === "object" && o["isReactComponent"])
        return true;
    return false;
}