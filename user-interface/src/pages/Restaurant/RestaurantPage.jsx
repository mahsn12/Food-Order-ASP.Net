import { useMemo, useState } from 'react'
import { useApp } from '../../context/AppContext'
import { useRouter } from '../../utils/router'

export default function RestaurantPage() {
  const { path } = useRouter()
  const id = path.split('/')[2]
  const { restaurants, addToCart } = useApp()
  const [qty, setQty] = useState({})
  const [notes, setNotes] = useState({})
  const restaurant = useMemo(() => restaurants.find((x) => String(x.id) === id), [restaurants, id])

  if (!restaurant) return <section className="card">Restaurant not found.</section>

  return <section className="card"><h2>{restaurant.name}</h2><p>{restaurant.description}</p><h3>Menu</h3>{restaurant.products.map((p) => <div className="product" key={p.id}><div><strong>{p.name}</strong><p>{p.description}</p><small>${p.price}</small><input placeholder="Special notes" value={notes[p.id] || ''} onChange={(e) => setNotes({ ...notes, [p.id]: e.target.value })} /></div><div><input type="number" min="1" value={qty[p.id] || 1} onChange={(e) => setQty({ ...qty, [p.id]: Number(e.target.value) })} /><button onClick={() => { for (let i = 0; i < (qty[p.id] || 1); i += 1) addToCart(restaurant.id, p, notes[p.id] || '') }}>Add to cart</button></div></div>)}</section>
}
