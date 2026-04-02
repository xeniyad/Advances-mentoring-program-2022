import React, { useEffect, useState } from 'react';
import { useMsal } from '@azure/msal-react';
import { adminApi } from '../services/api';

// ---- Category form ----
function CategoryForm({ initial, categories, onSave, onCancel }) {
  const [name, setName] = useState(initial?.name || '');
  const [parentId, setParentId] = useState(initial?.parentId ?? '');
  const [imageUrl, setImageUrl] = useState(initial?.image?.url || initial?.image || '');

  const handleSubmit = (e) => {
    e.preventDefault();
    onSave({
      name,
      parentId: parentId !== '' ? Number(parentId) : null,
      image: imageUrl || null,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="admin-form">
      <div className="form-row">
        <label className="form-label">Name <span className="form-required">*</span></label>
        <input className="form-input" value={name} onChange={e => setName(e.target.value)} required placeholder="Category name" />
      </div>
      <div className="form-row">
        <label className="form-label">Parent Category</label>
        <select className="form-input" value={parentId} onChange={e => setParentId(e.target.value)}>
          <option value="">None (top-level)</option>
          {categories.filter(c => c.id !== initial?.id).map(c => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
      </div>
      <div className="form-row">
        <label className="form-label">Image URL</label>
        <input className="form-input" value={imageUrl} onChange={e => setImageUrl(e.target.value)} type="url" placeholder="https://example.com/image.jpg" />
      </div>
      <div className="form-actions">
        <button type="submit" className="btn btn-primary">Save</button>
        <button type="button" className="btn btn-ghost" onClick={onCancel}>Cancel</button>
      </div>
    </form>
  );
}

// ---- Item form ----
function ItemForm({ initial, onSave, onCancel }) {
  const [name, setName] = useState(initial?.name || '');
  const [description, setDescription] = useState(initial?.description || '');
  const [price, setPrice] = useState(initial?.price ?? '');
  const [amount, setAmount] = useState(initial?.amount ?? '');
  const [imageUrl, setImageUrl] = useState(initial?.image?.url || initial?.image || '');

  const handleSubmit = (e) => {
    e.preventDefault();
    const data = {
      name,
      description: description || null,
      price: Number(price),
      amount: Number(amount),
      image: imageUrl ? { url: imageUrl } : null,
    };
    if (initial?.id) data.id = initial.id;
    onSave(data);
  };

  return (
    <form onSubmit={handleSubmit} className="admin-form">
      <div className="form-grid">
        <div className="form-row">
          <label className="form-label">Name <span className="form-required">*</span></label>
          <input className="form-input" value={name} onChange={e => setName(e.target.value)} required placeholder="Item name" />
        </div>
        <div className="form-row">
          <label className="form-label">Price <span className="form-required">*</span></label>
          <input className="form-input" value={price} onChange={e => setPrice(e.target.value)} type="number" step="0.01" min="0" required placeholder="0.00" />
        </div>
        <div className="form-row">
          <label className="form-label">Amount in stock</label>
          <input className="form-input" value={amount} onChange={e => setAmount(e.target.value)} type="number" min="0" placeholder="0" />
        </div>
        <div className="form-row">
          <label className="form-label">Image URL</label>
          <input className="form-input" value={imageUrl} onChange={e => setImageUrl(e.target.value)} type="url" placeholder="https://example.com/image.jpg" />
        </div>
        <div className="form-row form-row--full">
          <label className="form-label">Description</label>
          <textarea className="form-input" value={description} onChange={e => setDescription(e.target.value)} rows={2} placeholder="Optional description" />
        </div>
      </div>
      <div className="form-actions">
        <button type="submit" className="btn btn-primary">Save</button>
        <button type="button" className="btn btn-ghost" onClick={onCancel}>Cancel</button>
      </div>
    </form>
  );
}

// ---- Main admin page ----
export default function AdminPage() {
  const { instance, accounts } = useMsal();
  const isAuthenticated = accounts.length > 0;
  const [tab, setTab] = useState('categories');

  // Categories
  const [categories, setCategories] = useState([]);
  const [catForm, setCatForm] = useState(null);
  const [catMsg, setCatMsg] = useState(null);

  // Items
  const [selectedCatId, setSelectedCatId] = useState('');
  const [items, setItems] = useState([]);
  const [itemForm, setItemForm] = useState(null);
  const [itemMsg, setItemMsg] = useState(null);

  const loadCategories = () => {
    adminApi.getCategories(instance)
      .then(data => setCategories(data?.categories || []))
      .catch(err => setCatMsg({ type: 'error', text: err.message }));
  };

  const loadItems = (catId) => {
    if (!catId) return;
    adminApi.getItems(instance, catId)
      .then(data => setItems(data?.items || []))
      .catch(err => setItemMsg({ type: 'error', text: err.message }));
  };

  useEffect(() => { loadCategories(); }, [instance]);
  useEffect(() => { loadItems(selectedCatId); }, [selectedCatId, instance]);

  // --- Category CRUD ---
  const saveCategory = async (data) => {
    try {
      if (catForm?.id) {
        await adminApi.updateCategory(instance, catForm.id, data);
      } else {
        await adminApi.createCategory(instance, data);
      }
      setCatForm(null);
      setCatMsg({ type: 'success', text: catForm?.id ? 'Category updated.' : 'Category created.' });
      loadCategories();
    } catch (err) {
      setCatMsg({ type: 'error', text: err.message });
    }
  };

  const deleteCategory = async (id) => {
    if (!window.confirm('Delete this category and all its items?')) return;
    try {
      await adminApi.deleteCategory(instance, id);
      setCatMsg({ type: 'success', text: 'Category deleted.' });
      loadCategories();
    } catch (err) {
      setCatMsg({ type: 'error', text: err.message });
    }
  };

  // --- Item CRUD ---
  const saveItem = async (data) => {
    try {
      if (data.id) {
        await adminApi.updateItem(instance, selectedCatId, data);
      } else {
        await adminApi.createItem(instance, selectedCatId, data);
      }
      setItemForm(null);
      setItemMsg({ type: 'success', text: data.id ? 'Item updated.' : 'Item created.' });
      loadItems(selectedCatId);
    } catch (err) {
      setItemMsg({ type: 'error', text: err.message });
    }
  };

  const deleteItem = async (itemId) => {
    if (!window.confirm('Delete this item?')) return;
    try {
      await adminApi.deleteItem(instance, selectedCatId, itemId);
      setItemMsg({ type: 'success', text: 'Item deleted.' });
      loadItems(selectedCatId);
    } catch (err) {
      setItemMsg({ type: 'error', text: err.message });
    }
  };

  const getCategoryName = (id) => categories.find(c => c.id === id)?.name || '—';

  if (!isAuthenticated) {
    return (
      <div className="page">
        <h1 className="page__title">Admin — Catalog</h1>
        <div className="empty-state">
          <div className="empty-state__icon">🔒</div>
          <div className="empty-state__title">Authentication required</div>
          <div className="empty-state__desc">Please log in to manage the catalog.</div>
          <button
            className="btn btn-primary"
            style={{ marginTop: '1rem' }}
            onClick={() => instance.loginPopup().catch(console.error)}
          >
            Login
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="page">
      <h1 className="page__title">Admin — Catalog</h1>

      <div className="admin-tabs">
        <button
          className={`admin-tab${tab === 'categories' ? ' active' : ''}`}
          onClick={() => setTab('categories')}
        >
          Categories
        </button>
        <button
          className={`admin-tab${tab === 'items' ? ' active' : ''}`}
          onClick={() => setTab('items')}
        >
          Items
        </button>
      </div>

      {/* ---- Categories tab ---- */}
      {tab === 'categories' && (
        <div className="admin-section">
          {catMsg && <div className={`alert alert-${catMsg.type}`}>{catMsg.text}</div>}

          {catForm !== null ? (
            <div className="admin-form-panel">
              <h3 className="admin-form-title">{catForm?.id ? 'Edit Category' : 'New Category'}</h3>
              <CategoryForm
                initial={catForm?.id ? catForm : null}
                categories={categories}
                onSave={saveCategory}
                onCancel={() => setCatForm(null)}
              />
            </div>
          ) : (
            <div className="admin-toolbar">
              <button className="btn btn-primary" onClick={() => { setCatForm({}); setCatMsg(null); }}>
                + Add Category
              </button>
            </div>
          )}

          <div className="cart-container" style={{ marginTop: '1rem' }}>
            <table className="cart-table">
              <thead>
                <tr>
                  <th style={{ width: 60 }}>ID</th>
                  <th>Name</th>
                  <th>Parent</th>
                  <th>Image</th>
                  <th style={{ width: 130 }}></th>
                </tr>
              </thead>
              <tbody>
                {categories.length === 0 ? (
                  <tr>
                    <td colSpan="5" className="admin-empty-row">No categories yet. Create one above.</td>
                  </tr>
                ) : categories.map(cat => (
                  <tr key={cat.id}>
                    <td className="admin-id">{cat.id}</td>
                    <td className="admin-name">{cat.name}</td>
                    <td>{cat.parentId ? getCategoryName(cat.parentId) : <span className="admin-none">—</span>}</td>
                    <td>
                      {cat.image?.url
                        ? <img src={cat.image.url} alt="" className="admin-thumb" />
                        : <span className="admin-none">—</span>}
                    </td>
                    <td>
                      <div className="admin-actions">
                        <button className="btn btn-outline btn-sm" onClick={() => { setCatForm(cat); setCatMsg(null); }}>Edit</button>
                        <button className="btn btn-danger btn-sm" onClick={() => deleteCategory(cat.id)}>Delete</button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ---- Items tab ---- */}
      {tab === 'items' && (
        <div className="admin-section">
          {itemMsg && <div className={`alert alert-${itemMsg.type}`}>{itemMsg.text}</div>}

          <div className="admin-toolbar">
            <select
              className="admin-select"
              value={selectedCatId}
              onChange={e => { setSelectedCatId(e.target.value); setItemForm(null); setItems([]); setItemMsg(null); }}
            >
              <option value="">— Select a category —</option>
              {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
            {selectedCatId && (
              <button className="btn btn-primary" onClick={() => { setItemForm({}); setItemMsg(null); }}>
                + Add Item
              </button>
            )}
          </div>

          {itemForm !== null && selectedCatId && (
            <div className="admin-form-panel">
              <h3 className="admin-form-title">{itemForm?.id ? 'Edit Item' : 'New Item'}</h3>
              <ItemForm
                initial={itemForm?.id ? itemForm : null}
                onSave={saveItem}
                onCancel={() => setItemForm(null)}
              />
            </div>
          )}

          {selectedCatId && (
            <div className="cart-container" style={{ marginTop: '1rem' }}>
              <table className="cart-table">
                <thead>
                  <tr>
                    <th style={{ width: 60 }}>ID</th>
                    <th>Name</th>
                    <th style={{ width: 90 }}>Price</th>
                    <th style={{ width: 90 }}>Stock</th>
                    <th>Description</th>
                    <th style={{ width: 130 }}></th>
                  </tr>
                </thead>
                <tbody>
                  {items.length === 0 ? (
                    <tr>
                      <td colSpan="6" className="admin-empty-row">No items in this category yet.</td>
                    </tr>
                  ) : items.map(item => (
                    <tr key={item.id}>
                      <td className="admin-id">{item.id}</td>
                      <td className="admin-name">{item.name}</td>
                      <td className="cart-item__price">${Number(item.price ?? 0).toFixed(2)}</td>
                      <td>{item.amount ?? '—'}</td>
                      <td className="admin-desc">{item.description || <span className="admin-none">—</span>}</td>
                      <td>
                        <div className="admin-actions">
                          <button className="btn btn-outline btn-sm" onClick={() => { setItemForm(item); setItemMsg(null); }}>Edit</button>
                          <button className="btn btn-danger btn-sm" onClick={() => deleteItem(item.id)}>Delete</button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {!selectedCatId && (
            <div className="empty-state">
              <div className="empty-state__icon">📂</div>
              <div className="empty-state__title">Select a category</div>
              <div className="empty-state__desc">Choose a category above to manage its items.</div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
