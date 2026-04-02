import React, { useEffect } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { MsalProvider, useMsal, useIsAuthenticated } from '@azure/msal-react';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig, loginRequest } from './authConfig';
import MockMsalProvider from './auth/MockMsalProvider';
import NavBar from './components/NavBar';
import CatalogPage from './pages/CatalogPage';
import CartPage from './pages/CartPage';
import OrdersPage from './pages/OrdersPage';
import AdminPage from './pages/AdminPage';

const msalInstance = new PublicClientApplication(msalConfig);
const useMockAuth = import.meta.env.VITE_AUTH_MOCK === 'true';

const AuthProvider = useMockAuth
  ? MockMsalProvider
  : ({ children }) => <MsalProvider instance={msalInstance}>{children}</MsalProvider>;

function RequireAuth({ children }) {
  const { instance } = useMsal();
  const isAuthenticated = useIsAuthenticated();

  useEffect(() => {
    if (!isAuthenticated) {
      instance.loginPopup(loginRequest).catch(console.error);
    }
  }, [isAuthenticated, instance]);

  if (!isAuthenticated) return null;
  return children;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <NavBar />
        <Routes>
          <Route path="/" element={<Navigate to="/catalog" />} />
          <Route path="/catalog" element={<CatalogPage />} />
          <Route path="/cart" element={<RequireAuth><CartPage /></RequireAuth>} />
          <Route path="/orders" element={<RequireAuth><OrdersPage /></RequireAuth>} />
          <Route path="/admin" element={<AdminPage />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
