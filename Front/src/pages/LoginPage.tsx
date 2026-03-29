import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Box,
  TextField,
  Button,
  Link,
  Typography,
  Card,
  Alert,
  CircularProgress,
  Divider,
  Stack,
  Chip,
} from '@mui/material'
import LockOutlinedIcon from '@mui/icons-material/LockOutlined'
import LocalShippingIcon from '@mui/icons-material/LocalShipping'
import { authService } from '../services/authService'
import type { User, LoginCredentials } from '../types'

interface LoginPageProps {
  onLogin: (user: User) => void
}

function LoginPage({ onLogin }: LoginPageProps) {
  const showDemoUsers = import.meta.env.VITE_SHOW_DEMO_USERS === 'true'
  const navigate = useNavigate()
  const [credentials, setCredentials] = useState<LoginCredentials>({
    email: '',
    password: '',
  })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target
    setCredentials((prev) => ({ ...prev, [name]: value }))
    setError('')
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setError('')

    const blockStatus = authService.isLoginBlocked()
    if (blockStatus.blocked) {
      setError('Usuario bloqueado temporalmente por intentos fallidos. Intentá nuevamente en unos minutos.')
      setLoading(false)
      return
    }

    if (!credentials.email || !credentials.password) {
      setError('Por favor completá todos los campos')
      setLoading(false)
      return
    }

    if (!authService.isValidEmail(credentials.email)) {
      setError('El email no tiene un formato válido')
      setLoading(false)
      return
    }

    try {
      const user = await authService.login(credentials)
      if (user) {
        authService.clearLoginAttempts()
        onLogin(user)
        navigate(user.role === 'transportista' ? '/transportista' : '/app')
      } else {
        const attemptStatus = authService.registerFailedLoginAttempt()
        if (attemptStatus.blocked) {
          setError('Usuario bloqueado temporalmente por intentos fallidos. Intentá nuevamente en unos minutos.')
        } else {
          setError(`Email o contraseña incorrectos. Te quedan ${attemptStatus.attemptsRemaining} intento(s).`)
        }
      }
    } catch {
      setError('Error al iniciar sesión')
    } finally {
      setLoading(false)
    }
  }

  const fillDemo = (email: string) => {
    setCredentials({ email, password: 'password123' })
    setError('')
  }

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        background: 'linear-gradient(160deg, #0D47A1 0%, #1565C0 40%, #1976d2 70%, #0277BD 100%)',
        px: 2,
        py: 4,
      }}
    >
      <Box sx={{ width: '100%', maxWidth: 420 }}>
        {/* Brand header above card */}
        <Box sx={{ textAlign: 'center', mb: 3, color: 'white' }}>
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 1, mb: 1 }}>
            <LocalShippingIcon sx={{ fontSize: 36 }} />
            <Typography variant="h4" fontWeight={800} sx={{ letterSpacing: '-0.5px' }}>
              LogiTrack
            </Typography>
          </Box>
          <Typography variant="body2" sx={{ opacity: 0.8 }}>
            Sistema de Gestión de Envíos
          </Typography>
        </Box>

        <Card
          sx={{
            p: { xs: 3, sm: 4 },
            borderRadius: 3,
            boxShadow: '0 20px 60px rgba(0,0,0,0.3)',
          }}
        >
          {/* Card title */}
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 3 }}>
            <Box
              sx={{
                width: 36,
                height: 36,
                borderRadius: 2,
                bgcolor: 'primary.main',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
              }}
            >
              <LockOutlinedIcon sx={{ color: 'white', fontSize: 18 }} />
            </Box>
            <Typography variant="h6" fontWeight={700}>
              Iniciar sesión
            </Typography>
          </Box>

          {error && (
            <Alert severity="error" sx={{ mb: 2.5 }} onClose={() => setError('')}>
              {error}
            </Alert>
          )}

          <form onSubmit={handleSubmit} noValidate>
            <Stack spacing={2.5}>
              <TextField
                label="Email"
                name="email"
                type="email"
                value={credentials.email}
                onChange={handleChange}
                placeholder="usuario@ejemplo.com"
                disabled={loading}
                fullWidth
                autoFocus
              />
              <TextField
                label="Contraseña"
                name="password"
                type="password"
                value={credentials.password}
                onChange={handleChange}
                disabled={loading}
                fullWidth
              />
              <Button
                type="submit"
                variant="contained"
                size="large"
                disabled={loading}
                fullWidth
                sx={{ mt: 0.5, minHeight: 48 }}
              >
                {loading ? (
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <CircularProgress size={20} color="inherit" />
                    Ingresando...
                  </Box>
                ) : (
                  'Ingresar'
                )}
              </Button>
            </Stack>
          </form>

          <Box sx={{ mt: 2.5, textAlign: 'center' }}>
            <Typography variant="body2" color="text.secondary">
              ¿No tenés cuenta?{' '}
              <Link
                component="button"
                variant="body2"
                onClick={() => navigate('/register')}
                sx={{ fontWeight: 600 }}
              >
                Registrate aquí
              </Link>
            </Typography>
          </Box>

          {/* Demo credentials */}
          {showDemoUsers && (
            <Box>
              <Divider sx={{ my: 3 }}>
                <Typography variant="caption" color="text.disabled" fontWeight={600}>
                  DEMO
                </Typography>
              </Divider>

              <Typography variant="caption" color="text.secondary" display="block" sx={{ mb: 1.5, textAlign: 'center' }}>
                Clic en un rol para autocompletar · contraseña: <strong>password123</strong>
              </Typography>

              <Stack direction="row" spacing={1} justifyContent="center" flexWrap="wrap" useFlexGap>
                <Chip
                  label="Supervisor"
                  color="error"
                  variant="outlined"
                  size="small"
                  onClick={() => fillDemo('supervisor@logitrack.com')}
                  sx={{ cursor: 'pointer', fontWeight: 600 }}
                />
                <Chip
                  label="Operador"
                  color="primary"
                  variant="outlined"
                  size="small"
                  onClick={() => fillDemo('operador@logitrack.com')}
                  sx={{ cursor: 'pointer', fontWeight: 600 }}
                />
                <Chip
                  label="Transportista"
                  color="success"
                  variant="outlined"
                  size="small"
                  onClick={() => fillDemo('transportista@logitrack.com')}
                  sx={{ cursor: 'pointer', fontWeight: 600 }}
                />
              </Stack>
            </Box>
          )}
        </Card>
      </Box>
    </Box>
   ) 
  

}

export default LoginPage
