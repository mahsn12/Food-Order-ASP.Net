import { useEffect, useState } from 'react'
import { apiRequest } from '../../api/client'
import { useApp } from '../../context/AppContext'

export default function OrderHistoryPage() {
  const { token, orders, loadOrders } = useApp()
  const [ratings, setRatings] = useState({})

  useEffect(() => { loadOrders() }, [])

  const reorder = async (id) => { await apiRequest(`/user/orders/${id}/reorder`, { token, method: 'POST' }); await loadOrders() }
  const rate = async (id) => {
    const value = ratings[id]
    if (!value?.ratingValue) return
    await apiRequest(`/user/orders/${id}/rating`, { token, method: 'POST', body: value })
    alert('Thanks for your feedback')
  }

  return <section className="card"><h2>Order History</h2>{orders.history.map((o) => <div key={o.orderId} className="restaurant"><p>#{o.orderId} • {o.restaurantName} • ${o.totalPrice} • {o.status}</p><button onClick={() => reorder(o.orderId)}>Reorder</button><input type="number" min="1" max="5" placeholder="Rate 1-5" onChange={(e) => setRatings({ ...ratings, [o.orderId]: { ...ratings[o.orderId], ratingValue: Number(e.target.value) } })} /><input placeholder="Leave comment" onChange={(e) => setRatings({ ...ratings, [o.orderId]: { ...ratings[o.orderId], comment: e.target.value } })} /><button onClick={() => rate(o.orderId)}>Submit review</button></div>)}</section>
}
