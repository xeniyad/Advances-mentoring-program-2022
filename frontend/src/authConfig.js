export const msalConfig = {
  auth: {
    clientId: 'bb61d1fa-e902-4c6b-b9ed-38e694f34cbb',
    authority: 'https://mentoringecommerce.b2clogin.com/mentoringecommerce.onmicrosoft.com/B2C_1_signupsignin1',
    redirectUri: 'https://calm-pond-0fdf51900.7.azurestaticapps.net',
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
