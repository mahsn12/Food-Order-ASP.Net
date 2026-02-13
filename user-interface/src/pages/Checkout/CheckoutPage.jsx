import { useState } from 'react'
import { useApp } from '../../context/AppContext'
import { useRouter } from '../../utils/router'

export default function CheckoutPage() {
  const { placeOrder, cart } = useApp()
  const { navigate } = useRouter()
  const [form, setForm] = useState({ address: '', paymentMethod: 'Cash' })
  const [status, setStatus] = useState('')

  const submit = async () => {
    const order = await placeOrder({})
    setStatus(`Order #${order.orderId} confirmed using ${form.paymentMethod}.`)
    navigate('/orders/current')
  }

  return <section className="card"><h2>Checkout</h2><input placeholder="Delivery address" onChange={(e) => setForm({ ...form, address: e.target.value })} /><select value={form.paymentMethod} onChange={(e) => setForm({ ...form, paymentMethod: e.target.value })}><option>Cash</option><option>Card</option></select><button disabled={!cart.length} onClick={submit}>Confirm order</button>{status && <p>{status}</p>}</section>
}
