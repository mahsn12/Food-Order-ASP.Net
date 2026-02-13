import { useApp } from '../context/AppContext'
import { LinkButton } from '../utils/router'

export default function NavBar() {
  const { user, cart, isAdmin, isRestaurant, isUser } = useApp()

  return (
    <nav className="nav">
      <LinkButton to="/">Home</LinkButton>
      {isUser && <>
        <LinkButton to="/orders/current">Orders</LinkButton>
        <LinkButton to="/orders/history">History</LinkButton>
        <LinkButton to="/cart">Cart ({cart.length})</LinkButton>
      </>}
      <LinkButton to="/account">Account</LinkButton>
      {isAdmin && <LinkButton to="/admin-dashboard">Admin Dashboard</LinkButton>}
      {isRestaurant && <LinkButton to="/restaurant-dashboard">Restaurant Dashboard</LinkButton>}
      {user?.email && <small>Logged in as {user.email}</small>}
    </nav>
  )
}
