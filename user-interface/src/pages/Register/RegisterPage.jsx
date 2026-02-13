import { useState } from 'react'
import { useApp } from '../../context/AppContext'
import { LinkButton, useRouter } from '../../utils/router'

export default function RegisterPage() {
  const { register } = useApp()
  const { navigate } = useRouter()
  const [form, setForm] = useState({ fullName: '', email: '', phoneNumber: '', password: '' })
  const [error, setError] = useState('')
  const submit = async (e) => {
    e.preventDefault()
    try { await register(form); navigate('/') } catch (err) { setError(err.message) }
  }
  return <form className="card" onSubmit={submit}><h2>Register</h2>{error && <p className="error">{error}</p>}<input placeholder="Full Name" onChange={(e) => setForm({ ...form, fullName: e.target.value })} /><input placeholder="Email" onChange={(e) => setForm({ ...form, email: e.target.value })} /><input placeholder="Phone" onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })} /><input type="password" placeholder="Password" onChange={(e) => setForm({ ...form, password: e.target.value })} /><button>Create account</button><p><LinkButton to="/login">Back to login</LinkButton></p></form>
}
