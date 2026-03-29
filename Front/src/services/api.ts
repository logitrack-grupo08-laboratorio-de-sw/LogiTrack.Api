import axios from 'axios'

const DEFAULT_API_BASE_URL = 'https://logitrack-api-4d2k.onrender.com/api'

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || DEFAULT_API_BASE_URL).replace(/\/$/, '')

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Interceptor para agregar token JWT si existe
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const status = error?.response?.status
    const requestUrl = String(error?.config?.url || '')
    const isLoginRequest = requestUrl.includes('/auth/login')

    if (status === 401 && !isLoginRequest) {
      localStorage.removeItem('authToken')
      localStorage.removeItem('user')
      window.dispatchEvent(new CustomEvent('auth:session-expired'))
    }

    return Promise.reject(error)
  },
)

export default api