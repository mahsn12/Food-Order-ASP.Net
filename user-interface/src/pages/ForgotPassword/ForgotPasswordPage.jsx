import { useState } from 'react'
import { apiRequest } from '../../api/client'
import { LinkButton } from '../../utils/router'

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState('')
  const [result, setResult] = useState(null)
  const submit = async (e) => { e.preventDefault(); setResult(await apiRequest('/auth/forget-password', { method: 'POST', body: { email } })) }
  return <form className="card" onSubmit={submit}><h2>Forgot Password</h2><input placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} /><button>Send verification/reset link</button>{result && <p>Email verification token: <code>{result.token || 'Generated'}</code></p>}<p><LinkButton to="/reset-password">Go to reset page</LinkButton></p></form>
}
