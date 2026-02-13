import { useEffect, useMemo, useState } from 'react';

const DEFAULT_API_BASE_URL = 'http://localhost:5082';
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL?.trim() || DEFAULT_API_BASE_URL).replace(/\/+$/, '');

const resources = [
  { key: 'users', label: 'Users', endpoint: '/api/Admin/Users', createDefaults: { name: '', email: '', phone: '', password: '', role: 0 }, updateDefaults: { id: 0, name: '', phone: '', role: 0 } },
  { key: 'restaurants', label: 'Restaurants', endpoint: '/api/Admin/Restaurants', createDefaults: { name: '', description: '', phone: '' }, updateDefaults: { id: 0, name: '', description: '', phone: '', isOpen: true } },
  { key: 'products', label: 'Products', endpoint: '/api/Admin/Products', createDefaults: { restaurantId: 0, name: '', description: '', price: 0, imageUrl: '', isAvailable: true }, updateDefaults: { id: 0, restaurantId: 0, name: '', description: '', price: 0, imageUrl: '', isAvailable: true } },
  { key: 'orders', label: 'Orders', endpoint: '/api/Admin/Orders', createDefaults: { userId: 0, restaurantId: 0, driverId: null, totalPrice: 0, status: 1 }, updateDefaults: { id: 0, userId: 0, restaurantId: 0, driverId: null, totalPrice: 0, status: 1, deliveredAt: null } }
];

const ORDER_STATUS = { 1: 'Pending', 2: 'Preparing', 3: 'On the way', 4: 'Delivered', 5: 'Cancelled' };

function App() {
  const [mode, setMode] = useState('user');
  return (
    <div className="app">
      <header className="mode-switch panel">
        <h1>Food Delivery Portal</h1>
        <div className="actions">
          <button className={mode === 'user' ? '' : 'secondary'} onClick={() => setMode('user')}>User App</button>
          <button className={mode === 'admin' ? '' : 'secondary'} onClick={() => setMode('admin')}>Admin CRUD</button>
        </div>
      </header>
      {mode === 'user' ? <UserPortal /> : <AdminPortal />}
    </div>
  );
}

function UserPortal() {
  const [token, setToken] = useState(() => localStorage.getItem('userToken') || '');
  const [authForm, setAuthForm] = useState({ fullName: '', phoneNumber: '', email: '', password: '' });
  const [isRegister, setIsRegister] = useState(false);
  const [profile, setProfile] = useState(null);
  const [restaurants, setRestaurants] = useState([]);
  const [menu, setMenu] = useState([]);
  const [selectedRestaurant, setSelectedRestaurant] = useState(null);
  const [cart, setCart] = useState([]);
  const [orders, setOrders] = useState([]);
  const [activeOrder, setActiveOrder] = useState(null);
  const [message, setMessage] = useState('');

  const cartTotal = useMemo(() => cart.reduce((sum, item) => sum + item.price * item.quantity, 0), [cart]);

  const authHeaders = (withJson = false) => ({ ...(withJson ? { 'Content-Type': 'application/json' } : {}), ...(token ? { Authorization: `Bearer ${token}` } : {}) });

  useEffect(() => {
    loadRestaurants();
  }, []);

  useEffect(() => {
    if (!token) return;
    loadProfile();
    loadOrders();
  }, [token]);

  async function loadRestaurants() {
    const response = await fetch(`${API_BASE_URL}/api/UserPortal/restaurants`);
    setRestaurants(await response.json());
  }

  async function loadMenu(restaurant) {
    setSelectedRestaurant(restaurant);
    const response = await fetch(`${API_BASE_URL}/api/UserPortal/restaurants/${restaurant.id}/menu`);
    setMenu(await response.json());
  }

  async function loadProfile() {
    const response = await fetch(`${API_BASE_URL}/api/UserPortal/profile`, { headers: authHeaders() });
    if (response.ok) setProfile(await response.json());
  }

  async function loadOrders() {
    const response = await fetch(`${API_BASE_URL}/api/UserPortal/orders`, { headers: authHeaders() });
    if (response.ok) setOrders(await response.json());
  }

  async function getOrderDetails(id) {
    const response = await fetch(`${API_BASE_URL}/api/UserPortal/orders/${id}`, { headers: authHeaders() });
    if (response.ok) setActiveOrder(await response.json());
  }

  async function submitAuth(e) {
    e.preventDefault();
    const endpoint = isRegister ? '/api/Auth/register' : '/api/Auth/login';
    const payload = isRegister ? authForm : { email: authForm.email, password: authForm.password };
    const response = await fetch(`${API_BASE_URL}${endpoint}`, { method: 'POST', headers: authHeaders(true), body: JSON.stringify(payload) });
    if (!response.ok) return setMessage(await response.text());
    const data = await response.json();
    localStorage.setItem('userToken', data.token);
    setToken(data.token);
    setMessage('Authenticated successfully.');
  }

  function logout() {
    localStorage.removeItem('userToken');
    setToken('');
    setProfile(null);
    setOrders([]);
    setActiveOrder(null);
  }

  function addToCart(product) {
    setCart((prev) => {
      const exists = prev.find((x) => x.productId === product.id);
      if (exists) return prev.map((x) => (x.productId === product.id ? { ...x, quantity: x.quantity + 1 } : x));
      return [...prev, { productId: product.id, name: product.name, price: product.price, quantity: 1 }];
    });
  }

  async function placeOrder(paymentMethod) {
    if (!token) return setMessage('Login first to place orders.');
    if (!selectedRestaurant || cart.length === 0) return;

    const payload = { restaurantId: selectedRestaurant.id, paymentMethod, items: cart.map((x) => ({ productId: x.productId, quantity: x.quantity })) };
    const response = await fetch(`${API_BASE_URL}/api/UserPortal/orders`, { method: 'POST', headers: authHeaders(true), body: JSON.stringify(payload) });
    if (!response.ok) return setMessage(await response.text());

    const order = await response.json();
    setActiveOrder(order);
    setCart([]);
    setMessage('Order placed successfully.');
    loadOrders();
  }

  return (
    <>
      <section className="panel">
        {!token ? (
          <form className="auth-form" onSubmit={submitAuth}>
            <h2>{isRegister ? 'Register' : 'Login'} as Customer</h2>
            {isRegister && <input placeholder="Full name" value={authForm.fullName} onChange={(e) => setAuthForm({ ...authForm, fullName: e.target.value })} required />}
            {isRegister && <input placeholder="Phone number" value={authForm.phoneNumber} onChange={(e) => setAuthForm({ ...authForm, phoneNumber: e.target.value })} />}
            <input placeholder="Email" type="email" value={authForm.email} onChange={(e) => setAuthForm({ ...authForm, email: e.target.value })} required />
            <input placeholder="Password" type="password" value={authForm.password} onChange={(e) => setAuthForm({ ...authForm, password: e.target.value })} required />
            <div className="actions"><button type="submit">Continue</button><button type="button" className="secondary" onClick={() => setIsRegister((p) => !p)}>{isRegister ? 'Have account? Login' : 'Need account? Register'}</button></div>
          </form>
        ) : (
          <div className="toolbar"><strong>{profile?.name || 'Customer'}</strong><span>({profile?.email})</span><button className="secondary" onClick={logout}>Logout</button></div>
        )}
        {message && <p>{message}</p>}
      </section>

      <section className="panel two-col">
        <div>
          <h2>Restaurants</h2>
          {restaurants.map((r) => (
            <div key={r.id} className="list-item">
              <div><strong>{r.name}</strong> - {r.isOpen ? 'Open' : 'Closed'} </div>
              <small>{r.description}</small>
              <button onClick={() => loadMenu(r)}>View Menu</button>
            </div>
          ))}
        </div>

        <div>
          <h2>{selectedRestaurant ? `${selectedRestaurant.name} Menu` : 'Menu'}</h2>
          {menu.map((p) => (
            <div key={p.id} className="list-item">
              <div><strong>{p.name}</strong> (${p.price})</div>
              <small>{p.description}</small>
              <button disabled={!p.isAvailable} onClick={() => addToCart(p)}>{p.isAvailable ? 'Add to cart' : 'Unavailable'}</button>
            </div>
          ))}
        </div>
      </section>

      <section className="panel two-col">
        <div>
          <h2>Cart</h2>
          {cart.map((c) => <div key={c.productId}>{c.name} x {c.quantity} = ${(c.price * c.quantity).toFixed(2)}</div>)}
          <strong>Total: ${cartTotal.toFixed(2)}</strong>
          <div className="actions">
            <button onClick={() => placeOrder(0)}>Place (Cash)</button>
            <button onClick={() => placeOrder(1)}>Place (Card)</button>
            <button onClick={() => placeOrder(2)}>Place (Online)</button>
          </div>
        </div>
        <div>
          <h2>Order History</h2>
          {orders.map((o) => (
            <div key={o.id} className="list-item">
              <div>#{o.id} - {o.restaurantName} - {ORDER_STATUS[o.status] || o.status}</div>
              <small>${o.totalPrice} • {new Date(o.createdAt).toLocaleString()}</small>
              <button onClick={() => getOrderDetails(o.id)}>Details</button>
            </div>
          ))}
        </div>
      </section>

      {activeOrder && (
        <section className="panel">
          <h2>Order #{activeOrder.id} Details</h2>
          <p>Status: <strong>{ORDER_STATUS[activeOrder.status] || activeOrder.status}</strong></p>
          <p>Restaurant: {activeOrder.restaurantName}</p>
          <p>Total: ${activeOrder.totalPrice}</p>
          <h3>Items</h3>
          {activeOrder.items.map((item) => <div key={item.id}>{item.productName} x {item.quantity} = ${item.lineTotal}</div>)}
          <h3>Payment</h3>
          <p>{activeOrder.payment?.method} - {activeOrder.payment?.status} ({activeOrder.payment?.transactionRef})</p>
        </section>
      )}
    </>
  );
}

function AdminPortal() {
  const [resourceKey, setResourceKey] = useState(resources[0].key);
  const [rows, setRows] = useState([]);
  const [authToken, setAuthToken] = useState(() => localStorage.getItem('adminToken') ?? '');
  const [loginForm, setLoginForm] = useState({ email: '', password: '' });
  const resource = useMemo(() => resources.find((r) => r.key === resourceKey), [resourceKey]);
  const [createJson, setCreateJson] = useState('');

  useEffect(() => setCreateJson(JSON.stringify(resource.createDefaults, null, 2)), [resource]);
  useEffect(() => { if (authToken) loadData(resource); }, [authToken, resource]);

  async function adminLogin(event) {
    event.preventDefault();
    const response = await fetch(`${API_BASE_URL}/api/Auth/admin/login`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(loginForm) });
    if (!response.ok) return;
    const data = await response.json();
    localStorage.setItem('adminToken', data.token);
    setAuthToken(data.token);
  }

  async function loadData(activeResource) {
    const response = await fetch(`${API_BASE_URL}${activeResource.endpoint}`, { headers: { Authorization: `Bearer ${authToken}` } });
    if (response.ok) setRows(await response.json());
  }

  async function createItem() {
    await fetch(`${API_BASE_URL}${resource.endpoint}`, { method: 'POST', headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${authToken}` }, body: createJson });
    loadData(resource);
  }

  if (!authToken) return <section className="panel"><form className="auth-form" onSubmit={adminLogin}><h2>Admin Login</h2><input placeholder="email" onChange={(e) => setLoginForm({ ...loginForm, email: e.target.value })} /><input placeholder="password" type="password" onChange={(e) => setLoginForm({ ...loginForm, password: e.target.value })} /><button>Login</button></form></section>;

  return (
    <section className="panel">
      <div className="toolbar"><select value={resourceKey} onChange={(e) => setResourceKey(e.target.value)}>{resources.map((r) => <option value={r.key} key={r.key}>{r.label}</option>)}</select><button className="secondary" onClick={() => { localStorage.removeItem('adminToken'); setAuthToken(''); }}>Logout</button></div>
      <textarea rows={10} value={createJson} onChange={(e) => setCreateJson(e.target.value)} />
      <button onClick={createItem}>Create</button>
      <pre>{JSON.stringify(rows, null, 2)}</pre>
    </section>
  );
}

export default App;
