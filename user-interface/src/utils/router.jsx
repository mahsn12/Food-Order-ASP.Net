import { createContext, useContext, useEffect, useMemo, useState } from 'react'

const RouterContext = createContext(null)

export function RouterProvider({ children }) {
  const [path, setPath] = useState(window.location.pathname || '/')

  useEffect(() => {
    const onPop = () => setPath(window.location.pathname || '/')
    window.addEventListener('popstate', onPop)
    return () => window.removeEventListener('popstate', onPop)
  }, [])

  const navigate = (to) => {
    if (to === path) return
    window.history.pushState({}, '', to)
    setPath(to)
  }

  const value = useMemo(() => ({ path, navigate }), [path])
  return <RouterContext.Provider value={value}>{children}</RouterContext.Provider>
}

export const useRouter = () => useContext(RouterContext)

export function LinkButton({ to, children }) {
  const { navigate } = useRouter()
  return <button type="button" className="linkLike" onClick={() => navigate(to)}>{children}</button>
}
