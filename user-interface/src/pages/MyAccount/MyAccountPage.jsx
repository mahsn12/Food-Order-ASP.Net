import { useEffect, useState } from 'react'
import { apiRequest } from '../../api/client'
import { useApp } from '../../context/AppContext'

export default function MyAccountPage() {
  const { token, logout } = useApp()
  const [account, setAccount] = useState({ fullName: '', email: '', phoneNumber: '', addresses: [''] })
  const [password, setPassword] = useState({ currentPassword: '', newPassword: '' })

  useEffect(() => {
    apiRequest('/user/account', { token }).then((a) => setAccount((prev) => ({ ...prev, ...a })))
  }, [token])

  const save = async () => { await apiRequest('/user/account', { token, method: 'PUT', body: { fullName: account.fullName, phoneNumber: account.phoneNumber } }); alert('Profile updated') }
  const changePassword = async () => { await apiRequest('/user/change-password', { token, method: 'POST', body: password }); alert('Password changed') }

  return <section className="card"><h2>My Account</h2><input value={account.fullName} onChange={(e) => setAccount({ ...account, fullName: e.target.value })} /><input value={account.email} readOnly /><input value={account.phoneNumber || ''} onChange={(e) => setAccount({ ...account, phoneNumber: e.target.value })} /><input placeholder="Address" value={account.addresses[0]} onChange={(e) => setAccount({ ...account, addresses: [e.target.value] })} /><button onClick={save}>Save profile</button><h3>Change password</h3><input type="password" placeholder="Current password" onChange={(e) => setPassword({ ...password, currentPassword: e.target.value })} /><input type="password" placeholder="New password" onChange={(e) => setPassword({ ...password, newPassword: e.target.value })} /><button onClick={changePassword}>Change password</button><button onClick={logout}>Logout</button></section>
}
