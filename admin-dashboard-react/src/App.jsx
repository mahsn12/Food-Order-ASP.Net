import { useEffect, useMemo, useState } from 'react';

const resources = [
  {
    key: 'users',
    label: 'Users',
    endpoint: '/api/Admin/Users',
    fields: ['id', 'name', 'email', 'phone', 'password', 'role', 'createdAt'],
    createDefaults: { name: '', email: '', phone: '', password: '', role: 0 },
    updateDefaults: { id: 0, name: '', phone: '', role: 0 }
  },
  {
    key: 'restaurants',
    label: 'Restaurants',
    endpoint: '/api/Admin/Restaurants',
    fields: ['id', 'name', 'description', 'phone', 'ratingAvg', 'isOpen', 'createdAt'],
    createDefaults: { name: '', description: '', phone: '' },
    updateDefaults: { id: 0, name: '', description: '', phone: '', isOpen: true }
  },
  {
    key: 'products',
    label: 'Products',
    endpoint: '/api/Admin/Products',
    fields: ['id', 'restaurantId', 'name', 'description', 'price', 'imageUrl', 'isAvailable'],
    createDefaults: { restaurantId: 0, name: '', description: '', price: 0, imageUrl: '', isAvailable: true },
    updateDefaults: { id: 0, restaurantId: 0, name: '', description: '', price: 0, imageUrl: '', isAvailable: true }
  },
  {
    key: 'orders',
    label: 'Orders',
    endpoint: '/api/Admin/Orders',
    fields: ['id', 'userId', 'restaurantId', 'driverId', 'totalPrice', 'status', 'createdAt', 'deliveredAt'],
    createDefaults: { userId: 0, restaurantId: 0, driverId: null, totalPrice: 0, status: 0 },
    updateDefaults: { id: 0, userId: 0, restaurantId: 0, driverId: null, totalPrice: 0, status: 0, deliveredAt: null }
  },
  {
    key: 'orderItems',
    label: 'Order Items',
    endpoint: '/api/Admin/OrderItems',
    fields: ['id', 'orderId', 'productId', 'quantity', 'price'],
    createDefaults: { orderId: 0, productId: 0, quantity: 1, price: 0 },
    updateDefaults: { id: 0, orderId: 0, productId: 0, quantity: 1, price: 0 }
  },
  {
    key: 'payments',
    label: 'Payments',
    endpoint: '/api/Admin/Payments',
    fields: ['id', 'orderId', 'method', 'status', 'transactionRef', 'paidAt'],
    createDefaults: { orderId: 0, method: 0, status: 0, transactionRef: '', paidAt: null },
    updateDefaults: { id: 0, orderId: 0, method: 0, status: 0, transactionRef: '', paidAt: null }
  },
  {
    key: 'ratings',
    label: 'Ratings',
    endpoint: '/api/Admin/Ratings',
    fields: ['id', 'userId', 'orderId', 'restaurantId', 'driverId', 'ratingValue', 'comment', 'createdAt'],
    createDefaults: { userId: 0, orderId: 0, restaurantId: 0, driverId: null, ratingValue: 5, comment: '' },
    updateDefaults: { id: 0, userId: 0, orderId: 0, restaurantId: 0, driverId: null, ratingValue: 5, comment: '' }
  },
  {
    key: 'deliveryTrackings',
    label: 'Delivery Trackings',
    endpoint: '/api/Admin/DeliveryTrackings',
    fields: ['id', 'orderId', 'latitude', 'longitude', 'updatedAt'],
    createDefaults: { orderId: 0, latitude: 0, longitude: 0, updatedAt: null },
    updateDefaults: { id: 0, orderId: 0, latitude: 0, longitude: 0, updatedAt: null }
  }
];

const DEFAULT_API_BASE_URL = 'http://localhost:5082';

function resolveApiBaseUrl() {
  const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim();
  const baseUrl = configuredBaseUrl || DEFAULT_API_BASE_URL;

  return baseUrl.replace(/\/+$/, '');
}

const API_BASE_URL = resolveApiBaseUrl();

function App() {
  const [resourceKey, setResourceKey] = useState(resources[0].key);
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [createJson, setCreateJson] = useState('');
  const [editId, setEditId] = useState(null);
  const [editJson, setEditJson] = useState('');
  const [adminUser, setAdminUser] = useState(() => localStorage.getItem('adminUser') ?? '');
  const [authToken, setAuthToken] = useState(() => localStorage.getItem('adminToken') ?? '');
  const [loginForm, setLoginForm] = useState({ email: '', password: '' });
  const [loginLoading, setLoginLoading] = useState(false);

  const resource = useMemo(() => resources.find((r) => r.key === resourceKey), [resourceKey]);

  useEffect(() => {
    setCreateJson(JSON.stringify(resource.createDefaults, null, 2));
    setEditId(null);
    setEditJson('');

    if (authToken) {
      loadData(resource);
    }
  }, [resource, authToken]);

  function createHeaders(includeContentType = true) {
    const headers = {};

    if (includeContentType) {
      headers['Content-Type'] = 'application/json';
    }

    if (authToken) {
      headers.Authorization = `Bearer ${authToken}`;
    }

    return headers;
  }

  async function loadData(activeResource = resource) {
    if (!authToken) return;

    setLoading(true);
    setError('');

    try {
      const response = await fetch(`${API_BASE_URL}${activeResource.endpoint}`, {
        headers: createHeaders(false)
      });
      if (!response.ok) throw new Error(`Failed to fetch ${activeResource.label}.`);
      const data = await response.json();
      setRows(Array.isArray(data) ? data : []);
    } catch (e) {
      setError(e.message);
      setRows([]);
    } finally {
      setLoading(false);
    }
  }

  async function adminLogin(event) {
    event.preventDefault();
    setError('');
    setLoginLoading(true);

    try {
      const response = await fetch(`${API_BASE_URL}/api/Auth/admin/login`, {
        method: 'POST',
        headers: createHeaders(),
        body: JSON.stringify(loginForm)
      });

      if (!response.ok) {
        if (response.status === 403) {
          throw new Error('This user does not have the Admin role.');
        }

        const body = await response.text();
        throw new Error(body || 'Admin login failed.');
      }

      const data = await response.json();
      const roles = Array.isArray(data.roles) ? data.roles : [];
      if (!roles.includes('Admin')) {
        throw new Error('This user does not have the Admin role.');
      }

      localStorage.setItem('adminToken', data.token);
      localStorage.setItem('adminUser', data.fullName || data.email);
      setAuthToken(data.token);
      setAdminUser(data.fullName || data.email);
      setLoginForm({ email: '', password: '' });
    } catch (e) {
      setError(`Login error: ${e.message}`);
    } finally {
      setLoginLoading(false);
    }
  }

  function logout() {
    localStorage.removeItem('adminToken');
    localStorage.removeItem('adminUser');
    setAuthToken('');
    setAdminUser('');
    setRows([]);
    setError('');
  }

  async function createItem() {
    try {
      const payload = JSON.parse(createJson);
      const response = await fetch(`${API_BASE_URL}${resource.endpoint}`, {
        method: 'POST',
        headers: createHeaders(),
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const body = await response.text();
        throw new Error(body || 'Create failed.');
      }

      await loadData();
      setCreateJson(JSON.stringify(resource.createDefaults, null, 2));
    } catch (e) {
      setError(`Create error: ${e.message}`);
    }
  }

  async function updateItem() {
    if (editId == null) return;

    try {
      const payload = JSON.parse(editJson);
      const response = await fetch(`${API_BASE_URL}${resource.endpoint}/${editId}`, {
        method: 'PUT',
        headers: createHeaders(),
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const body = await response.text();
        throw new Error(body || 'Update failed.');
      }

      await loadData();
      setEditId(null);
      setEditJson('');
    } catch (e) {
      setError(`Update error: ${e.message}`);
    }
  }

  async function deleteItem(id) {
    const confirmed = window.confirm(`Delete ${resource.label} record #${id}?`);
    if (!confirmed) return;

    try {
      const response = await fetch(`${API_BASE_URL}${resource.endpoint}/${id}`, {
        method: 'DELETE',
        headers: createHeaders(false)
      });
      if (!response.ok) throw new Error('Delete failed.');
      await loadData();
    } catch (e) {
      setError(`Delete error: ${e.message}`);
    }
  }

  function startEdit(row) {
    setEditId(row.id);
    setEditJson(JSON.stringify({ ...resource.updateDefaults, ...row, id: row.id }, null, 2));
  }

  if (!authToken) {
    return (
      <div className="auth-page">
        <section className="auth-card panel">
          <h1>Admin Login</h1>
          <p>Sign in with an account that has the Admin role.</p>

          {error && <p className="error">{error}</p>}

          <form className="auth-form" onSubmit={adminLogin}>
            <label htmlFor="admin-email">Email</label>
            <input
              id="admin-email"
              type="email"
              autoComplete="email"
              value={loginForm.email}
              onChange={(e) => setLoginForm((prev) => ({ ...prev, email: e.target.value }))}
              required
            />

            <label htmlFor="admin-password">Password</label>
            <input
              id="admin-password"
              type="password"
              autoComplete="current-password"
              value={loginForm.password}
              onChange={(e) => setLoginForm((prev) => ({ ...prev, password: e.target.value }))}
              required
            />

            <button type="submit" disabled={loginLoading}>
              {loginLoading ? 'Signing in...' : 'Sign in as Admin'}
            </button>
          </form>
        </section>
      </div>
    );
  }

  return (
    <div className="app">
      <header>
        <h1>Food Delivery Admin Dashboard</h1>
        <p>Manage all API resources from one place.</p>
      </header>

      <div className="toolbar">
        <label htmlFor="resource">Resource</label>
        <select id="resource" value={resourceKey} onChange={(e) => setResourceKey(e.target.value)}>
          {resources.map((r) => (
            <option key={r.key} value={r.key}>{r.label}</option>
          ))}
        </select>

        <button onClick={() => loadData()} disabled={loading}>Refresh</button>
        <span className="auth-user">Signed in as {adminUser || 'Admin'}</span>
        <button className="secondary" onClick={logout}>Logout</button>
      </div>

      {error && <p className="error">{error}</p>}

      <section className="panel">
        <h2>{resource.label}</h2>
        {loading ? <p>Loading...</p> : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  {resource.fields.map((field) => <th key={field}>{field}</th>)}
                  <th>actions</th>
                </tr>
              </thead>
              <tbody>
                {rows.map((row) => (
                  <tr key={row.id}>
                    {resource.fields.map((field) => <td key={`${row.id}-${field}`}>{String(row[field] ?? '')}</td>)}
                    <td className="actions">
                      <button onClick={() => startEdit(row)}>Edit</button>
                      <button className="danger" onClick={() => deleteItem(row.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>

      <section className="forms">
        <div className="panel">
          <h3>Create {resource.label}</h3>
          <textarea value={createJson} onChange={(e) => setCreateJson(e.target.value)} rows={12} />
          <button onClick={createItem}>Create</button>
        </div>

        <div className="panel">
          <h3>Edit {resource.label}</h3>
          {editId == null ? <p>Select a row to edit.</p> : (
            <>
              <textarea value={editJson} onChange={(e) => setEditJson(e.target.value)} rows={12} />
              <button onClick={updateItem}>Update #{editId}</button>
            </>
          )}
        </div>
      </section>
    </div>
  );
}

export default App;
