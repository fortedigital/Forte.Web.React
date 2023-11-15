import React from 'react'
import ReactDOM from 'react-dom/client'
import ReactDOMServer from 'react-dom/server'
import Example from './Example.tsx'
import './index.css'

declare let global: any;

const getGlobalObject = () => {
    if (typeof globalThis !== 'undefined') { return globalThis; }
    if (typeof self !== 'undefined') { return self; }
    if (typeof window !== 'undefined') { return window; }
    if (typeof global !== 'undefined') { return global; }
    throw new Error('cannot find the global object');
};

(() => {
    const globalObject = getGlobalObject();
    globalObject.React = React;
    globalObject.ReactDOMClient = ReactDOM;
    globalObject.ReactDOMServer = ReactDOMServer;
    
    globalObject["__react"] = { Example };
})();


if(import.meta.env.MODE === "development") {
    ReactDOM.createRoot(document.getElementById('root')!).render(
        <React.StrictMode>
            <h1>Development mode preview</h1>
            <Example initCount={0} text={"Text from props."}/>
        </React.StrictMode>,
    )
}
