import { useApp } from '../../context/AppContext'
import { LinkButton } from '../../utils/router'

export default function CartPage() {
  const { cart, cartTotal, updateQty, removeFromCart } = useApp()
  const fee = cart.length ? 4.99 : 0
  const promo = cartTotal > 30 ? -5 : 0
  return <section className="card"><h2>Cart</h2>{cart.map((x) => <div className="product" key={x.id}><span>{x.name} (${x.price})</span><div><input type="number" min="1" value={x.quantity} onChange={(e) => updateQty(x.id, Number(e.target.value))} /><button onClick={() => removeFromCart(x.id)}>Remove</button></div></div>)}<p>Items: ${cartTotal.toFixed(2)} | Promo: ${promo.toFixed(2)} | Delivery fee: ${fee.toFixed(2)}</p><strong>Total ${(cartTotal + fee + promo).toFixed(2)}</strong><p><LinkButton to="/checkout">Checkout</LinkButton></p></section>
}
