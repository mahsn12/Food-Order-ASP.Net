import { useEffect, useState } from 'react'
import { apiRequest } from '../../api/client'
import { useApp } from '../../context/AppContext'

export default function CurrentOrderPage() {
  const { token, orders, loadOrders } = useApp()
  const [tracking, setTracking] = useState({})

  useEffect(() => { loadOrders() }, [])

  const track = async (id) => setTracking({ ...tracking, [id]: await apiRequest(`/user/orders/${id}/tracking`, { token }) })
  const cancel = async (id) => { await apiRequest(`/user/orders/${id}/cancel`, { token, method: 'POST' }); await loadOrders() }

  return <section className="card"><h2>Current Orders</h2>{orders.active.map((o) => <div className="product" key={o.orderId}><span>#{o.orderId} {o.restaurantName} - {o.status}</span><div><button onClick={() => track(o.orderId)}>Track</button><button onClick={() => cancel(o.orderId)}>Cancel</button></div>{tracking[o.orderId] && <small>Driver location (optional map): [{tracking[o.orderId].latitude}, {tracking[o.orderId].longitude}] ETA {tracking[o.orderId].updatedAt}</small>}</div>)}</section>
}
