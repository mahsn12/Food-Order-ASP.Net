import { useState } from 'react'
import { apiRequest } from '../../api/client'

export default function ResetPasswordPage() {
  const [form, setForm] = useState({ email: '', token: '', newPassword: '' })
  const [message, setMessage] = useState('')
  const submit = async (e) => {
    e.preventDefault()
    const res = await apiRequest('/auth/reset-password', { method: 'POST', body: form })
    setMessage(res.message)
  }
  return <form className="card" onSubmit={submit}><h2>Email Verification & Reset Password</h2><input placeholder="Email" onChange={(e) => setForm({ ...form, email: e.target.value })} /><input placeholder="Reset token" onChange={(e) => setForm({ ...form, token: e.target.value })} /><input type="password" placeholder="New password" onChange={(e) => setForm({ ...form, newPassword: e.target.value })} /><button>Reset password</button>{message && <p>{message}</p>}</form>
}
