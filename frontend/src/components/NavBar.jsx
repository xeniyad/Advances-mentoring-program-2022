import React from 'react';
import { NavLink } from 'react-router-dom';
import { useMsal } from '@azure/msal-react';
import { loginRequest } from '../authConfig';

export default function NavBar() {
  const { instance, accounts } = useMsal();
  const isAuthenticated = accounts.length > 0;
  const user = accounts[0]?.username || accounts[0]?.name;

  const handleLogin = () => instance.loginPopup(loginRequest).catch(console.error);
  const handleLogout = () => instance.logoutPopup().catch(console.error);

  return (
    <nav className="navbar">
      <div className="navbar__inner">
        <NavLink to="/" className="navbar__brand">🛒 ShopApp</NavLink>
        <ul className="navbar__links">
          <li>
            <NavLink to="/catalog" className={({ isActive }) => 'navbar__link' + (isActive ? ' active' : '')}>
              Catalog
            </NavLink>
          </li>
          {isAuthenticated && (
            <li>
              <NavLink to="/cart" className={({ isActive }) => 'navbar__link' + (isActive ? ' active' : '')}>
                Cart
              </NavLink>
            </li>
          )}
          {isAuthenticated && (
            <li>
              <NavLink to="/orders" className={({ isActive }) => 'navbar__link' + (isActive ? ' active' : '')}>
                Orders
              </NavLink>
            </li>
          )}
          <li>
            <NavLink to="/admin" className={({ isActive }) => 'navbar__link' + (isActive ? ' active' : '')}>
              Admin
            </NavLink>
          </li>
        </ul>
        <div className="navbar__right">
          {isAuthenticated ? (
            <>
              <span className="navbar__user" title={user}>{user}</span>
              <button className="btn btn-ghost btn-sm" onClick={handleLogout}>Logout</button>
            </>
          ) : (
            <button className="btn btn-primary btn-sm" onClick={handleLogin}>Login</button>
          )}
        </div>
      </div>
    </nav>
  );
}
