import React from 'react';
import ReactDOM from 'react-dom/client';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from './authConfig';
import './index.css';
import App from './App';

const msalInstance = new PublicClientApplication(msalConfig);

msalInstance.initialize().then(() => {
    const root = ReactDOM.createRoot(document.getElementById('root'));
    root.render(<React.StrictMode><App msalInstance={msalInstance} /></React.StrictMode>);
});