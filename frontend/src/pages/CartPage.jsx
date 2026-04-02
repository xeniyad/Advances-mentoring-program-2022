import React, { useEffect, useState } from 'react';
import { useMsal } from '@azure/msal-react';
import { Link, useNavigate } from 'react-router-dom';
import { cartApi, ordersApi } from '../services/api';

const CART_ID_KEY = 'cartId';

function getItemPrice(item) {
  if (item.price == null) return 0;
  if (typeof item.price === 'object' && item.price.amount != null) return Number(item.price.amount);
  return Number(item.price);
}

function formatPrice(amount) {
  return `$${Number(amount).toFixed(2)}`;
}

export default function CartPage() {
  const { instance } = useMsal();
  const navigate = useNavigate();
  const cartId = localStorage.getItem(CART_ID_KEY);
  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState(null);

  const loadCart = () => {
    if (!cartId) { setLoading(false); return; }
    setLoading(true);
    cartApi.getCart(instance, cartId)
      .then(setCart)
      .catch(err => setMessage({ type: 'error', text: err.message }))
      .finally(() => setLoading(false));
  };

  useEffect(() => { loadCart(); }, [cartId]);

  const removeItem = async (itemId) => {
    try {
      await cartApi.removeItem(instance, cartId, itemId);
      loadCart();
    } catch (err) {
      setMessage({ type: 'error', text: err.message });
    }
  };

  const checkout = async () => {
    try {
      await ordersApi.placeOrder(instance, cartId);
      localStorage.removeItem(CART_ID_KEY);
      setMessage({ type: 'success', text: 'Order placed successfully!' });
      setTimeout(() => navigate('/orders'), 1500);
    } catch (err) {
      setMessage({ type: 'error', text: err.message });
    }
  };

  if (!cartId || (!loading && !cart)) {
    return (
      <div className="page">
        <h1 className="page__title">Cart</h1>
        <div className="empty-state">
          <div className="empty-state__icon">🛒</div>
          <div className="empty-state__title">Your cart is empty</div>
          <div className="empty-state__desc">Browse the catalog and add some items.</div>
          <Link to="/catalog" className="btn btn-primary">Go to Catalog</Link>
        </div>
      </div>
    );
  }

  const items = cart?.items || [];
  const total = items.reduce((sum, item) => sum + getItemPrice(item) * (item.quantity || 1), 0);

  return (
    <div className="page">
      <h1 className="page__title">Cart</h1>

      {message && (
        <div className={`alert alert-${message.type}`}>{message.text}</div>
      )}

      {loading ? (
        <div className="spinner" />
      ) : items.length === 0 ? (
        <div className="empty-state">
          <div className="empty-state__icon">🛒</div>
          <div className="empty-state__title">No items in cart</div>
          <div className="empty-state__desc">Add something from the catalog.</div>
          <Link to="/catalog" className="btn btn-primary">Go to Catalog</Link>
        </div>
      ) : (
        <>
          <div className="cart-container">
            <table className="cart-table">
              <thead>
                <tr>
                  <th>Item</th>
                  <th>Price</th>
                  <th>Qty</th>
                  <th>Subtotal</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {items.map(item => {
                  const price = getItemPrice(item);
                  const qty = item.quantity || 1;
                  return (
                    <tr key={item.id}>
                      <td className="cart-item__name">{item.name}</td>
                      <td className="cart-item__price">{formatPrice(price)}</td>
                      <td><span className="cart-item__qty">{qty}</span></td>
                      <td>{formatPrice(price * qty)}</td>
                      <td>
                        <button
                          className="btn btn-ghost btn-sm"
                          onClick={() => removeItem(item.id)}
                          title="Remove"
                        >
                          ✕
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          <div className="cart-summary">
            <div>
              <div className="cart-total-label">Total ({items.length} item{items.length !== 1 ? 's' : ''})</div>
              <div className="cart-total-amount">{formatPrice(total)}</div>
            </div>
            <button className="btn btn-success btn-lg" onClick={checkout}>
              Checkout
            </button>
          </div>
        </>
      )}
    </div>
  );
}
