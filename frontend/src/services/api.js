import { apiConfig, apiRequest } from '../authConfig';

async function getToken(msalInstance) {
  const accounts = msalInstance.getAllAccounts();
  if (!accounts.length) return null;
  const request = { ...apiRequest, account: accounts[0] };
  try {
    const response = await msalInstance.acquireTokenSilent(request);
    return response.accessToken;
  } catch {
    // Silent renewal failed (expired, consent required) — fall back to interactive
    const response = await msalInstance.acquireTokenPopup(request);
    return response.accessToken;
  }
}

async function apiFetch(msalInstance, path, options = {}) {
  const token = await getToken(msalInstance);
  const headers = {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  };
  const response = await fetch(`${apiConfig.baseUrl}${path}`, { ...options, headers });
  if (!response.ok) {
    const text = await response.text();
    throw new Error(`API error ${response.status}: ${text}`);
  }
  if (response.status === 204) return null;
  return response.json();
}

// Catalog public API (through gateway: /catalog/* → strips /catalog → catalog service /api/v1/*)
export const catalogApi = {
  getCategories: (msal) => apiFetch(msal, '/catalog/api/v1/category'),
  getItems: (msal, categoryId) =>
    apiFetch(msal, `/catalog/api/v1/category/${categoryId}/items`),
};

// Admin CRUD for catalog
export const adminApi = {
  // Categories
  getCategories: (msal) => apiFetch(msal, '/catalog/api/v1/category'),
  createCategory: (msal, data) =>
    apiFetch(msal, '/catalog/api/v1/category', { method: 'POST', body: JSON.stringify(data) }),
  updateCategory: (msal, id, data) =>
    apiFetch(msal, `/catalog/api/v1/category/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  deleteCategory: (msal, id) =>
    apiFetch(msal, `/catalog/api/v1/category/${id}`, { method: 'DELETE' }),

  // Enums
  getCurrencies: (msal) => apiFetch(msal, '/catalog/api/v1/enums/currencies'),

  // Items (nested under category)
  getItems: (msal, categoryId) =>
    apiFetch(msal, `/catalog/api/v1/category/${categoryId}/items`),
  createItem: (msal, categoryId, data) =>
    apiFetch(msal, `/catalog/api/v1/category/${categoryId}/items`, { method: 'POST', body: JSON.stringify(data) }),
  updateItem: (msal, categoryId, data) =>
    apiFetch(msal, `/catalog/api/v1/category/${categoryId}/items`, { method: 'PUT', body: JSON.stringify(data) }),
  deleteItem: (msal, categoryId, itemId) =>
    apiFetch(msal, `/catalog/api/v1/category/${categoryId}/items/${itemId}`, { method: 'DELETE' }),
};

export const cartApi = {
  getCart: (msal, cartId) => apiFetch(msal, `/cart/${cartId}`, { headers: { 'api-version': '1.0' } }),
  addItem: (msal, cartId, item) =>
    apiFetch(msal, `/cart/${cartId}`, { method: 'POST', body: JSON.stringify(item) }),
  removeItem: (msal, cartId, itemId) =>
    apiFetch(msal, `/cart/${cartId}/items/${itemId}`, { method: 'DELETE' }),
};

export const ordersApi = {
  getOrders: (msal) => apiFetch(msal, '/orders'),
  placeOrder: (msal, cartId) =>
    apiFetch(msal, `/orders?cartId=${cartId}`, { method: 'POST' }),
};
