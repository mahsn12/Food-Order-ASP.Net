import { useEffect, useMemo, useState } from 'react'

const API = 'http://localhost:5095/api'

function App() {
  const [token, setToken] = useState(localStorage.getItem('user_token') || '')
  const [auth, setAuth] = useState({ fullName: '', email: '', password: '', phoneNumber: '' })
  const [account, setAccount] = useState({ fullName: '', email: '', phoneNumber: '' })
  const [restaurants, setRestaurants] = useState([])
  const [cart, setCart] = useState([])
  const [activeOrders, setActiveOrders] = useState([])
  const [history, setHistory] = useState([])
  const [tracking, setTracking] = useState(null)
  const [status, setStatus] = useState('')

  const cartTotal = useMemo(() => cart.reduce((sum, i) => sum + i.price * i.quantity, 0), [cart])

  const req = async (path, options = {}) => {
    const headers = { 'Content-Type': 'application/json', ...(options.headers || {}) }
    if (token) headers.Authorization = `Bearer ${token}`
    const res = await fetch(`${API}${path}`, { ...options, headers })
    if (!res.ok) throw new Error(await res.text())
    return res.status === 204 ? null : res.json()
  }

  const loadCatalog = async () => setRestaurants(await req('/user/restaurants'))
  const loadAccount = async () => token && setAccount(await req('/user/account'))
  const loadOrders = async () => {
    if (!token) return
    setActiveOrders(await req('/user/orders/active'))
    setHistory(await req('/user/orders/history'))
  }

  useEffect(() => { loadCatalog() }, [])
  useEffect(() => { loadAccount(); loadOrders() }, [token])

  const addToCart = (p, restaurantId) => {
    setCart((prev) => {
      if (prev.length && prev[0].restaurantId !== restaurantId) return [{ ...p, quantity: 1, restaurantId }]
      const found = prev.find((x) => x.id === p.id)
      if (found) return prev.map((x) => x.id === p.id ? { ...x, quantity: x.quantity + 1 } : x)
      return [...prev, { ...p, quantity: 1, restaurantId }]
    })
  }

  const register = async () => {
    const r = await req('/auth/register', { method: 'POST', body: JSON.stringify(auth) })
    localStorage.setItem('user_token', r.token); setToken(r.token); setStatus('Registered successfully.')
  }
  const login = async () => {
    const r = await req('/auth/login', { method: 'POST', body: JSON.stringify({ email: auth.email, password: auth.password }) })
    localStorage.setItem('user_token', r.token); setToken(r.token); setStatus('Logged in.')
  }
  const updateAccount = async () => {
    await req('/user/account', { method: 'PUT', body: JSON.stringify({ fullName: account.fullName, phoneNumber: account.phoneNumber }) })
    setStatus('Account updated.')
  }

  const placeOrder = async () => {
    if (!cart.length) return
    const order = await req('/user/orders', {
      method: 'POST',
      body: JSON.stringify({
        items: cart.map((i) => ({ productId: i.id, quantity: i.quantity })),
        deliveryLatitude: 40.73061,
        deliveryLongitude: -73.935242
      })
    })
    setStatus(`Order #${order.orderId} placed.`)
    setCart([])
    await loadOrders()
  }

  const trackOrder = async (id) => setTracking(await req(`/user/orders/${id}/tracking`))

  return (
    <div className="page">
      <header><h1>Foodly - User Interface</h1><p>Order food, track delivery, manage account and review history.</p></header>
      {status && <div className="status">{status}</div>}
      <section className="card">
        <h2>Authentication</h2>
        <div className="grid">
          <input placeholder="Full name" value={auth.fullName} onChange={(e) => setAuth({ ...auth, fullName: e.target.value })} />
          <input placeholder="Email" value={auth.email} onChange={(e) => setAuth({ ...auth, email: e.target.value })} />
          <input placeholder="Phone" value={auth.phoneNumber} onChange={(e) => setAuth({ ...auth, phoneNumber: e.target.value })} />
          <input placeholder="Password" type="password" value={auth.password} onChange={(e) => setAuth({ ...auth, password: e.target.value })} />
        </div>
        <button onClick={register}>Register</button><button onClick={login}>Login</button>
      </section>

      <section className="split">
        <div className="card">
          <h2>Menu</h2>
          {restaurants.map((r) => (
            <div key={r.id} className="restaurant">
              <h3>{r.name} {r.isOpen ? '🟢' : '🔴'}</h3><small>{r.description}</small>
              {r.products.map((p) => (
                <div key={p.id} className="product"><span>{p.name} - ${p.price}</span><button onClick={() => addToCart(p, r.id)}>Add</button></div>
              ))}
            </div>
          ))}
        </div>
        <div className="card">
          <h2>Cart</h2>
          {cart.map((i) => <div key={i.id}>{i.name} x {i.quantity}</div>)}
          <strong>Total: ${cartTotal.toFixed(2)}</strong>
          <button onClick={placeOrder} disabled={!token || !cart.length}>Place Order</button>
        </div>
      </section>

      <section className="split">
        <div className="card">
          <h2>Active Orders & Tracking</h2>
          {activeOrders.map((o) => <div key={o.orderId} className="product"><span>#{o.orderId} {o.restaurantName} - {o.status}</span><button onClick={() => trackOrder(o.orderId)}>Track</button></div>)}
          {tracking && <p>Order #{tracking.orderId}: {tracking.status}, location [{tracking.latitude}, {tracking.longitude}]</p>}
        </div>
        <div className="card">
          <h2>Order History</h2>
          {history.map((o) => <div key={o.orderId}>#{o.orderId} • {o.restaurantName} • ${o.totalPrice} • {o.status}</div>)}
        </div>
      </section>

      <section className="card">
        <h2>My Account</h2>
        <div className="grid">
          <input placeholder="Full name" value={account.fullName} onChange={(e) => setAccount({ ...account, fullName: e.target.value })} />
          <input placeholder="Email" value={account.email} readOnly />
          <input placeholder="Phone" value={account.phoneNumber || ''} onChange={(e) => setAccount({ ...account, phoneNumber: e.target.value })} />
        </div>
        <button onClick={updateAccount} disabled={!token}>Save</button>
      </section>
    </div>
  )
}

export default App
