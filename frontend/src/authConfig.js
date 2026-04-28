export const msalConfig = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_CLIENT_ID,
    authority: import.meta.env.VITE_AZURE_AUTHORITY,
    redirectUri: import.meta.env.VITE_REDIRECT_URI,
    knownAuthorities: [import.meta.env.VITE_AZURE_KNOWN_AUTHORITY],
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
