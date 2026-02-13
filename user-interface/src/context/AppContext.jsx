import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import { apiRequest } from '../api/client'

const AppContext = createContext(null)

export function AppProvider({ children }) {
  const [token, setToken] = useState(localStorage.getItem('user_token') || sessionStorage.getItem('user_token') || '')
  const [rememberMe, setRememberMe] = useState(Boolean(localStorage.getItem('user_token')))
  const [user, setUser] = useState(null)
  const [restaurants, setRestaurants] = useState([])
  const [cart, setCart] = useState([])
  const [orders, setOrders] = useState({ active: [], history: [] })

  const cartTotal = useMemo(() => cart.reduce((sum, x) => sum + x.price * x.quantity, 0), [cart])
  const isAdmin = user?.roles?.includes('Admin')
  const isRestaurant = user?.roles?.includes('Restaurant')
  const isUser = user?.roles?.includes('User')

  const setAuthToken = (nextToken, remember = rememberMe) => {
    setToken(nextToken)
    setRememberMe(remember)
    localStorage.removeItem('user_token')
    sessionStorage.removeItem('user_token')
    if (nextToken) {
      ;(remember ? localStorage : sessionStorage).setItem('user_token', nextToken)
    }
  }

  const loadRestaurants = async () => setRestaurants(await apiRequest('/user/restaurants'))
  const loadOrders = async () => {
    if (!token || !isUser) return
    const [active, history] = await Promise.all([
      apiRequest('/user/orders/active', { token }),
      apiRequest('/user/orders/history', { token })
    ])
    setOrders({ active, history })
  }

  const loadProfile = async () => {
    if (!token) return setUser(null)
    const profile = await apiRequest('/auth/profile', { token })
    setUser((current) => ({ ...(current || {}), ...profile, roles: current?.roles || [] }))
  }

  useEffect(() => { loadRestaurants() }, [])
  useEffect(() => { loadProfile() }, [token])
  useEffect(() => { loadOrders() }, [token, isUser])

  const login = async (email, password, remember = false) => {
    const auth = await apiRequest('/auth/login', { method: 'POST', body: { email, password } })
    setAuthToken(auth.token, remember)
    setUser({ id: auth.userId, fullName: auth.fullName, email: auth.email, roles: auth.roles })
    return auth
  }

  const register = async (payload) => {
    const auth = await apiRequest('/auth/register', { method: 'POST', body: payload })
    setAuthToken(auth.token, true)
    setUser({ id: auth.userId, fullName: auth.fullName, email: auth.email, roles: auth.roles })
    return auth
  }

  const logout = () => {
    setAuthToken('', false)
    setUser(null)
    setCart([])
  }

  const addToCart = (restaurantId, product, notes = '', quantity = 1) => {
    const safeQuantity = Number.isFinite(quantity) ? Math.max(1, Math.floor(quantity)) : 1

    setCart((prev) => {
      if (prev.length && prev[0].restaurantId !== restaurantId) {
        return [{ ...product, restaurantId, quantity: safeQuantity, notes }]
      }

      const found = prev.find((x) => x.id === product.id)

      if (found) {
        return prev.map((x) => x.id === product.id
          ? { ...x, quantity: x.quantity + safeQuantity, notes: notes || x.notes }
          : x)
      }

      return [...prev, { ...product, restaurantId, quantity: safeQuantity, notes }]
    })
  }

  const updateQty = (id, quantity) => setCart((prev) => prev.map((x) => x.id === id ? { ...x, quantity: Math.max(1, quantity) } : x))
  const removeFromCart = (id) => setCart((prev) => prev.filter((x) => x.id !== id))

  const placeOrder = async ({ lat = 40.73061, lng = -73.935242 } = {}) => {
    const order = await apiRequest('/user/orders', {
      token,
      method: 'POST',
      body: { items: cart.map((x) => ({ productId: x.id, quantity: x.quantity })), deliveryLatitude: lat, deliveryLongitude: lng }
    })
    setCart([])
    await loadOrders()
    return order
  }

  return <AppContext.Provider value={{ token, user, rememberMe, restaurants, cart, cartTotal, orders, isAdmin, isRestaurant, isUser, loadOrders, setUser, setRememberMe, login, register, logout, addToCart, updateQty, removeFromCart, placeOrder, setAuthToken }}>{children}</AppContext.Provider>
}

export const useApp = () => useContext(AppContext)
