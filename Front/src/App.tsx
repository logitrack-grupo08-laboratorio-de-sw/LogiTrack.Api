import { useState, useEffect } from 'react'
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { Box, CircularProgress } from '@mui/material'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import Dashboard from './pages/Dashboard'
import ShipmentDetail from './pages/ShipmentDetail'
import VehicleDetail from './pages/VehicleDetail'
import Layout from './components/Layout'
import RoutesDashboard from './pages/transportista/RoutesDashboard'
import RouteDetail from './pages/transportista/RouteDetail'
import LandingPage from './pages/landing/LandingPage'
import { authService } from './services/authService'
import type { User } from './types'

function App() {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)
  const [sessionExpired, setSessionExpired] = useState(false)

  useEffect(() => {
    const storedUser = localStorage.getItem('user')
    if (storedUser) {
      setUser(JSON.parse(storedUser))
    }
    setLoading(false)
  }, [])

  useEffect(() => {
    const handleSessionExpired = () => {
      setUser(null)
      setSessionExpired(true)
      authService.clearSession()
    }

    window.addEventListener('auth:session-expired', handleSessionExpired)

    return () => {
      window.removeEventListener('auth:session-expired', handleSessionExpired)
    }
  }, [])

  const handleLogin = (userData: User) => {
    setUser(userData)
    setSessionExpired(false)
    localStorage.setItem('user', JSON.stringify(userData))
  }

  const handleLogout = () => {
    setUser(null)
    setSessionExpired(false)
    authService.logout()
  }

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    )
  }

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LandingPage />} />

        <Route
          path="/login"
          element={
            user ? <Navigate to={user.role === 'transportista' ? '/transportista' : '/app'} /> : <LoginPage onLogin={handleLogin} />
          }
        />
        <Route
          path="/register"
          element={
            user ? <Navigate to={user.role === 'transportista' ? '/transportista' : '/app'} /> : <RegisterPage onLogin={handleLogin} />
          }
        />

        <Route
          path="/transportista"
          element={
            user?.role === 'transportista' ? (
              <Layout user={user} onLogout={handleLogout} />
            ) : user ? (
              <Navigate to="/app" />
            ) : (
              <Navigate to={sessionExpired ? '/login' : '/'} />
            )
          }
        >
          <Route index element={<RoutesDashboard user={user as User} />} />
          <Route path="ruta/:id" element={<RouteDetail />} />
        </Route>

        <Route
          element={
            user ? (
              user.role === 'transportista' ? (
                <Navigate to="/transportista" />
              ) : (
                <Layout user={user} onLogout={handleLogout} />
              )
            ) : (
              <Navigate to={sessionExpired ? '/login' : '/'} />
            )
          }
        >
          <Route path="/app" element={<Dashboard />} />
          <Route path="/shipment/:id" element={<ShipmentDetail />} />
          <Route path="/vehiculo/:id" element={<VehicleDetail />} />
        </Route>

        <Route path="*" element={<Navigate to={user ? (user.role === 'transportista' ? '/transportista' : '/app') : '/'} />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
