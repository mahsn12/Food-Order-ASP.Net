import NavBar from './components/NavBar'
import { useApp } from './context/AppContext'
import LoginPage from './pages/Login/LoginPage'
import RegisterPage from './pages/Register/RegisterPage'
import ForgotPasswordPage from './pages/ForgotPassword/ForgotPasswordPage'
import ResetPasswordPage from './pages/ResetPassword/ResetPasswordPage'
import HomePage from './pages/Home/HomePage'
import RestaurantPage from './pages/Restaurant/RestaurantPage'
import CartPage from './pages/Cart/CartPage'
import CheckoutPage from './pages/Checkout/CheckoutPage'
import CurrentOrderPage from './pages/CurrentOrder/CurrentOrderPage'
import OrderHistoryPage from './pages/OrderHistory/OrderHistoryPage'
import MyAccountPage from './pages/MyAccount/MyAccountPage'
import AdminDashboardPage from './pages/AdminDashboard/AdminDashboardPage'
import { useRouter } from './utils/router'

export default function App() {
  const { token } = useApp()
  const { path } = useRouter()

  const privateView = (node) => token ? node : <LoginPage />
  const view = {
    '/': <HomePage />,
    '/login': <LoginPage />,
    '/register': <RegisterPage />,
    '/forgot-password': <ForgotPasswordPage />,
    '/reset-password': <ResetPasswordPage />,
    '/cart': <CartPage />,
    '/checkout': privateView(<CheckoutPage />),
    '/orders/current': privateView(<CurrentOrderPage />),
    '/orders/history': privateView(<OrderHistoryPage />,
    ),
    '/account': privateView(<MyAccountPage />),
    '/admin-dashboard': privateView(<AdminDashboardPage />)
  }

  const page = path.startsWith('/restaurant/') ? <RestaurantPage /> : (view[path] || <HomePage />)

  return <div className="page"><h1>Food Delivery App</h1><NavBar />{page}</div>
}
