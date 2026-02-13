import { useApp } from '../context/AppContext'
import { LinkButton } from '../utils/router'

export default function NavBar() {
  const { user, cart } = useApp()
  const isAdmin = user?.roles?.includes('Admin')

  return (
    <nav className="nav">
      <LinkButton to="/">Home</LinkButton>
      <LinkButton to="/orders/current">Orders</LinkButton>
      <LinkButton to="/orders/history">History</LinkButton>
      <LinkButton to="/cart">Cart ({cart.length})</LinkButton>
      <LinkButton to="/account">Account</LinkButton>
      {isAdmin && <LinkButton to="/admin-dashboard">Admin Dashboard</LinkButton>}
    </nav>
  )
}
