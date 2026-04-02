import React, { useState } from 'react';
import { MsalContext } from '@azure/msal-react';

const MOCK_ACCOUNT = {
  homeAccountId: 'local-user',
  localAccountId: 'local-user',
  username: 'admin@local.dev',
  name: 'Local Admin',
  tenantId: 'local',
};

// Base64URL encoding (RFC 7515) — required by the JWT spec.
// Standard btoa() produces Base64 with +, / and = padding which .NET's
// JsonWebToken parser can reject.
function base64url(str) {
  return btoa(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
}

// Build a fake JWT (unsigned) that the API Gateway accepts in Development mode
// (appsettings.Development.json removes AuthorizationPolicy from all routes).
// CatalogService reads roles from the payload for [Authorize(Roles=...)] checks.
function buildMockJwt() {
  const header = base64url(JSON.stringify({ alg: 'none', typ: 'JWT' }));
  const payload = base64url(JSON.stringify({
    sub: MOCK_ACCOUNT.localAccountId,
    name: MOCK_ACCOUNT.name,
    preferred_username: MOCK_ACCOUNT.username,
    roles: ['catalog/create', 'catalog/update', 'catalog/delete'],
    aud: '32486baf-e4f4-4937-85bd-622bd4a709b7',
    iss: 'https://login.microsoftonline.com/local/v2.0',
    iat: Math.floor(Date.now() / 1000),
    nbf: Math.floor(Date.now() / 1000),
    exp: Math.floor(Date.now() / 1000) + 86400,
  }));
  return `${header}.${payload}.`;
}

function makeMockInstance(setAccounts) {
  return {
    getAllAccounts: () => JSON.parse(sessionStorage.getItem('mockAccounts') || 'null') ?? [],
    acquireTokenSilent: async () => ({ accessToken: buildMockJwt() }),
    loginPopup: async () => {
      sessionStorage.setItem('mockAccounts', JSON.stringify([MOCK_ACCOUNT]));
      setAccounts([MOCK_ACCOUNT]);
    },
    logoutPopup: async () => {
      sessionStorage.removeItem('mockAccounts');
      setAccounts([]);
    },
  };
}

export default function MockMsalProvider({ children }) {
  const [accounts, setAccounts] = useState(() => {
    // Auto-login: if no mock session exists yet, create one automatically
    const stored = JSON.parse(sessionStorage.getItem('mockAccounts') || 'null');
    if (stored && stored.length) return stored;
    sessionStorage.setItem('mockAccounts', JSON.stringify([MOCK_ACCOUNT]));
    return [MOCK_ACCOUNT];
  });

  const instance = makeMockInstance(setAccounts);

  return (
    <MsalContext.Provider value={{ instance, accounts, inProgress: 'none' }}>
      {children}
    </MsalContext.Provider>
  );
}
