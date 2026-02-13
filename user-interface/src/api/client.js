const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5095/api'

export async function apiRequest(path, { token, method = 'GET', body, headers = {} } = {}) {
  const requestHeaders = { ...headers }
  if (body !== undefined) requestHeaders['Content-Type'] = 'application/json'
  if (token) requestHeaders.Authorization = `Bearer ${token}`

  const response = await fetch(`${API_BASE}${path}`, {
    method,
    headers: requestHeaders,
    body: body !== undefined ? JSON.stringify(body) : undefined
  })

  if (!response.ok) {
    throw new Error(await response.text())
  }

  if (response.status === 204) return null
  return response.json()
}
