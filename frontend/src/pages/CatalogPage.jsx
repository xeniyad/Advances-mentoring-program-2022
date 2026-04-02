import React, { useEffect, useState } from 'react';
import { useMsal } from '@azure/msal-react';
import { catalogApi, cartApi } from '../services/api';

const CART_ID_KEY = 'cartId';

function getOrCreateCartId() {
  let id = localStorage.getItem(CART_ID_KEY);
  if (!id) {
    id = crypto.randomUUID();
    localStorage.setItem(CART_ID_KEY, id);
  }
  return id;
}

function formatPrice(price) {
  if (price == null) return '';
  if (typeof price === 'object' && price.amount != null) return `$${Number(price.amount).toFixed(2)}`;
  return `$${Number(price).toFixed(2)}`;
}

export default function CatalogPage() {
  const { instance, accounts } = useMsal();
  const isAuthenticated = accounts.length > 0;
  const [categories, setCategories] = useState([]);
  const [items, setItems] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState(null);

  // Load categories once
  useEffect(() => {
    catalogApi.getCategories(instance)
      .then(data => setCategories(data?.categories || []))
      .catch(err => setMessage({ type: 'error', text: err.message }));
  }, [instance]);

  // Load items whenever selectedCategory or categories change
  useEffect(() => {
    setLoading(true);
    setItems([]);

    if (selectedCategory) {
      catalogApi.getItems(instance, selectedCategory)
        .then(data => setItems(data?.items || []))
        .catch(err => setMessage({ type: 'error', text: err.message }))
        .finally(() => setLoading(false));
    } else {
      // All categories — fetch items in parallel
      const cats = Array.isArray(categories) ? categories : [];
      if (cats.length === 0) { setLoading(false); return; }
      Promise.all(
        cats.map(cat =>
          catalogApi.getItems(instance, cat.id)
            .then(data => data?.items || [])
            .catch(() => [])
        )
      )
        .then(results => setItems(results.flat()))
        .finally(() => setLoading(false));
    }
  }, [instance, selectedCategory, categories]);

  const addToCart = async (item) => {
    if (!isAuthenticated) {
      setMessage({ type: 'info', text: 'Please log in to add items to cart.' });
      return;
    }
    const cartId = getOrCreateCartId();
    try {
      await cartApi.addItem(instance, cartId, {
        id: item.id,
        name: item.name,
        price: { amount: item.price, currency: 'USD' },
        quantity: 1,
      });
      setMessage({ type: 'success', text: `"${item.name}" added to cart!` });
      setTimeout(() => setMessage(null), 3000);
    } catch (err) {
      setMessage({ type: 'error', text: err.message });
    }
  };

  return (
    <div className="page">
      <h1 className="page__title">Catalog</h1>

      {message && (
        <div className={`alert alert-${message.type}`}>{message.text}</div>
      )}

      <div className="category-filter">
        <button
          className={`category-pill${!selectedCategory ? ' active' : ''}`}
          onClick={() => setSelectedCategory(null)}
        >
          All
        </button>
        {categories.map(cat => (
          <button
            key={cat.id}
            className={`category-pill${selectedCategory === cat.id ? ' active' : ''}`}
            onClick={() => setSelectedCategory(cat.id)}
          >
            {cat.name}
          </button>
        ))}
      </div>

      {loading ? (
        <div className="spinner" />
      ) : items.length === 0 ? (
        <div className="empty-state">
          <div className="empty-state__icon">📦</div>
          <div className="empty-state__title">No items found</div>
          <div className="empty-state__desc">Try selecting a different category.</div>
        </div>
      ) : (
        <div className="product-grid">
          {items.map(item => (
            <div key={item.id} className="product-card">
              {item.image?.url
                ? <img className="product-card__img" src={item.image.url} alt={item.image.altText || item.name} />
                : <div className="product-card__img-placeholder">🏷️</div>
              }
              <div className="product-card__body">
                <div className="product-card__name">{item.name}</div>
                <div className="product-card__price">{formatPrice(item.price)}</div>
              </div>
              <div className="product-card__footer">
                <button className="btn btn-primary" onClick={() => addToCart(item)}>
                  Add to Cart
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
