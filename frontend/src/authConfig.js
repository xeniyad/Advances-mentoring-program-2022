export const msalConfig = {
  auth: {
    clientId: '32486baf-e4f4-4937-85bd-622bd4a709b7',
    authority: 'https://login.microsoftonline.com/cc0890f9-759c-4f92-b85c-a632a36232bf',
    redirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: 'sessionStorage',
    storeAuthStateInCookie: false,
  },
};

export const loginRequest = {
  scopes: ['openid', 'profile', 'email'],
};

// Scopes for acquiring an access token targeting the ApiGateway.
// Must match a scope exposed by the app registration in Azure AD.
export const apiRequest = {
  scopes: [`api://${msalConfig.auth.clientId}/access_as_user`],
};

export const apiConfig = {
  // Empty in dev — Vite proxy (vite.config.js) forwards /catalog, /cart, /orders to the gateway.
  // Set VITE_API_URL at build time for production Docker builds.
  baseUrl: import.meta.env.VITE_API_URL || '',
};
