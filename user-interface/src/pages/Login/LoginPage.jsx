import { useState } from 'react'
import { useApp } from '../../context/AppContext'
import { LinkButton, useRouter } from '../../utils/router'

export default function LoginPage() {
  const { login } = useApp()
  const { navigate } = useRouter()
  const [form, setForm] = useState({ email: '', password: '', remember: true, admin: false })
  const [error, setError] = useState('')

  const submit = async (e) => {
    e.preventDefault()
    try { await login(form.email, form.password, form.admin, form.remember); navigate('/') } catch (err) { setError(err.message) }
  }

  return <form className="card" onSubmit={submit}><h2>Login</h2>{error && <p className="error">{error}</p>}<input placeholder="Email" onChange={(e) => setForm({ ...form, email: e.target.value })} /><input type="password" placeholder="Password" onChange={(e) => setForm({ ...form, password: e.target.value })} /><label><input type="checkbox" checked={form.remember} onChange={(e) => setForm({ ...form, remember: e.target.checked })} /> Remember me</label><label><input type="checkbox" checked={form.admin} onChange={(e) => setForm({ ...form, admin: e.target.checked })} /> Login as Admin</label><button>Login</button><p><LinkButton to="/register">Register</LinkButton> <LinkButton to="/forgot-password">Forgot Password</LinkButton></p></form>
}
