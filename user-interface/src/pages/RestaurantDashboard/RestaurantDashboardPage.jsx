import { useEffect, useState } from 'react'
import { apiRequest } from '../../api/client'
import { useApp } from '../../context/AppContext'

const orderStatuses = ['Pending', 'Preparing', 'OnTheWay', 'Delivered', 'Cancelled']

export default function RestaurantDashboardPage() {
  const { token } = useApp()
  const [overview, setOverview] = useState(null)
  const [products, setProducts] = useState([])
  const [orders, setOrders] = useState([])
  const [form, setForm] = useState({ name: '', description: '', price: '', imageUrl: '', isAvailable: true })
  const [message, setMessage] = useState('')

  const loadData = async () => {
    const [overviewData, productsData, ordersData] = await Promise.all([
      apiRequest('/restaurant-dashboard/overview', { token }),
      apiRequest('/restaurant-dashboard/products', { token }),
      apiRequest('/restaurant-dashboard/orders', { token })
    ])
    setOverview(overviewData)
    setProducts(productsData)
    setOrders(ordersData)
  }

  useEffect(() => { loadData() }, [])

  const createProduct = async (e) => {
    e.preventDefault()
    await apiRequest('/restaurant-dashboard/products', {
      token,
      method: 'POST',
      body: {
        name: form.name,
        description: form.description,
        price: Number(form.price),
        imageUrl: form.imageUrl,
        isAvailable: form.isAvailable
      }
    })
    setForm({ name: '', description: '', price: '', imageUrl: '', isAvailable: true })
    setMessage('Product added successfully.')
    await loadData()
  }

  const updateOrderStatus = async (orderId, status) => {
    await apiRequest(`/restaurant-dashboard/orders/${orderId}/status`, {
      token,
      method: 'PUT',
      body: { status }
    })
    await loadData()
  }

  return <section className="card"><h2>Restaurant Dashboard</h2>{overview && <p>{overview.name} • {overview.email} • Active orders: {overview.activeOrdersCount} • Products: {overview.productsCount}</p>}
    {message && <p>{message}</p>}
    <div className="split">
      <form onSubmit={createProduct}><h3>Add Product</h3><input required placeholder="Product name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} /><input placeholder="Description" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} /><input required type="number" min="0" step="0.01" placeholder="Price" value={form.price} onChange={(e) => setForm({ ...form, price: e.target.value })} /><input placeholder="Image URL" value={form.imageUrl} onChange={(e) => setForm({ ...form, imageUrl: e.target.value })} /><label><input type="checkbox" checked={form.isAvailable} onChange={(e) => setForm({ ...form, isAvailable: e.target.checked })} /> Available</label><button>Add Product</button></form>
      <div><h3>Your Products</h3>{products.map((p) => <div className="product" key={p.id}><div><strong>{p.name}</strong><p>{p.description}</p><small>${p.price}</small></div><small>{p.isAvailable ? 'Available' : 'Unavailable'}</small></div>)}</div>
    </div>
    <h3>Orders</h3>
    {orders.map((order) => <div className="product" key={order.orderId}><div><strong>Order #{order.orderId}</strong><p>Customer: {order.customerName}</p><p>Total: ${order.totalPrice}</p><p>{order.items.map((item) => `${item.productName} x${item.quantity}`).join(', ')}</p></div><div><select value={order.status} onChange={(e) => updateOrderStatus(order.orderId, e.target.value)}>{orderStatuses.map((status) => <option key={status} value={status}>{status}</option>)}</select></div></div>)}
  </section>
}
