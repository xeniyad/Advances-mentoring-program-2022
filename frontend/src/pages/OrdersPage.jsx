import React, { useEffect, useState } from 'react';
import { useMsal } from '@azure/msal-react';
import { Link } from 'react-router-dom';
import { ordersApi } from '../services/api';

export default function OrdersPage() {
  const { instance } = useMsal();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    ordersApi.getOrders(instance)
      .then(data => setOrders(data || []))
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [instance]);

  return (
    <div className="page">
      <h1 className="page__title">My Orders</h1>

      {error && <div className="alert alert-error">{error}</div>}

      {loading ? (
        <div className="spinner" />
      ) : orders.length === 0 ? (
        <div className="empty-state">
          <div className="empty-state__icon">📋</div>
          <div className="empty-state__title">No orders yet</div>
          <div className="empty-state__desc">Place your first order from the catalog.</div>
          <Link to="/catalog" className="btn btn-primary">Shop Now</Link>
        </div>
      ) : (
        orders.map(order => (
          <div key={order.id} className="order-card">
            <div className="order-card__header">
              <span className="order-card__id">#{order.id}</span>
              <span className="order-badge">{order.status || 'Placed'}</span>
            </div>
            <div className="order-card__meta">
              <div className="order-card__meta-item">
                <span className="order-card__meta-label">Total</span>
                <span className="order-card__meta-value">
                  ${Number(order.totalAmount ?? 0).toFixed(2)}
                </span>
              </div>
              <div className="order-card__meta-item">
                <span className="order-card__meta-label">Date</span>
                <span className="order-card__meta-value">
                  {order.createdAt ? new Date(order.createdAt).toLocaleDateString() : '—'}
                </span>
              </div>
              <div className="order-card__meta-item">
                <span className="order-card__meta-label">Items</span>
                <span className="order-card__meta-value">{order.items?.length ?? 0}</span>
              </div>
            </div>
            {order.items?.length > 0 && (
              <details className="order-items">
                <summary>View items</summary>
                <ul>
                  {order.items.map((item, i) => (
                    <li key={i}>{item.name} × {item.quantity} — ${item.price}</li>
                  ))}
                </ul>
              </details>
            )}
          </div>
        ))
      )}
    </div>
  );
}
