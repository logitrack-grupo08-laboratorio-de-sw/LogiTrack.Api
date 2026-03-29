import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import LoginPage from '../../../pages/LoginPage'

vi.mock('../../../services/authService', () => ({
  authService: {
    login: vi.fn(),
    isValidEmail: vi.fn(),
    isLoginBlocked: vi.fn(),
    registerFailedLoginAttempt: vi.fn(),
    clearLoginAttempts: vi.fn(),
  },
}))

import { authService } from '../../../services/authService'

const mockedAuthService = authService as unknown as {
  login: ReturnType<typeof vi.fn>
  isValidEmail: ReturnType<typeof vi.fn>
  isLoginBlocked: ReturnType<typeof vi.fn>
  registerFailedLoginAttempt: ReturnType<typeof vi.fn>
  clearLoginAttempts: ReturnType<typeof vi.fn>
}

describe('LoginPage integration', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockedAuthService.isValidEmail.mockReturnValue(true)
    mockedAuthService.isLoginBlocked.mockReturnValue({ blocked: false, remainingMs: 0 })
    mockedAuthService.registerFailedLoginAttempt.mockReturnValue({
      blocked: false,
      attemptsRemaining: 2,
      remainingMs: 0,
    })
  })

  const renderLogin = (onLogin = vi.fn()) => {
    render(
      <MemoryRouter initialEntries={['/login']}>
        <Routes>
          <Route path="/login" element={<LoginPage onLogin={onLogin} />} />
          <Route path="/app" element={<div>APP_DASHBOARD</div>} />
          <Route path="/transportista" element={<div>TRANSPORTISTA_DASHBOARD</div>} />
          <Route path="/register" element={<div>REGISTER_PAGE</div>} />
        </Routes>
      </MemoryRouter>,
    )
  }

  it('CP-06 permite login exitoso y redirige al dashboard del rol', async () => {
    const user = userEvent.setup()
    const onLogin = vi.fn()

    mockedAuthService.login.mockResolvedValueOnce({
      id: 'u-1',
      name: 'Florencia',
      lastname: 'Paez',
      email: 'florencia@gmail.com',
      dni: '11111111',
      role: 'operador',
    })

    renderLogin(onLogin)

    await user.type(screen.getByLabelText('Email'), 'florencia@gmail.com')
    await user.type(screen.getByLabelText('Contraseña'), '12345678')
    await user.click(screen.getByRole('button', { name: 'Ingresar' }))

    expect(onLogin).toHaveBeenCalledTimes(1)
    expect(mockedAuthService.clearLoginAttempts).toHaveBeenCalledTimes(1)
    expect(await screen.findByText('APP_DASHBOARD')).toBeInTheDocument()
  })

  it('CP-07 y CP-08 muestra error para credenciales invalidas', async () => {
    const user = userEvent.setup()

    mockedAuthService.login.mockResolvedValueOnce(null)

    renderLogin()

    await user.type(screen.getByLabelText('Email'), 'example@gmail.com')
    await user.type(screen.getByLabelText('Contraseña'), '888888')
    await user.click(screen.getByRole('button', { name: 'Ingresar' }))

    expect(await screen.findByText(/Email o contraseña incorrectos/)).toBeInTheDocument()
  })

  it('CP-09 valida campos vacios antes de enviar', async () => {
    const user = userEvent.setup()

    renderLogin()

    await user.click(screen.getByRole('button', { name: 'Ingresar' }))

    expect(await screen.findByText('Por favor completá todos los campos')).toBeInTheDocument()
    expect(mockedAuthService.login).not.toHaveBeenCalled()
  })

  it('CP-10 bloquea el login luego de 3 intentos fallidos', async () => {
    const user = userEvent.setup()
    let failedAttempts = 0
    let blocked = false

    mockedAuthService.login.mockResolvedValue(null)
    mockedAuthService.isLoginBlocked.mockImplementation(() => ({
      blocked,
      remainingMs: blocked ? 60_000 : 0,
    }))
    mockedAuthService.registerFailedLoginAttempt.mockImplementation(() => {
      failedAttempts += 1
      if (failedAttempts >= 3) {
        blocked = true
        return { blocked: true, attemptsRemaining: 0, remainingMs: 60_000 }
      }

      return { blocked: false, attemptsRemaining: 3 - failedAttempts, remainingMs: 0 }
    })

    renderLogin()

    await user.type(screen.getByLabelText('Email'), 'example@gmail.com')
    await user.type(screen.getByLabelText('Contraseña'), '888888')

    await user.click(screen.getByRole('button', { name: 'Ingresar' }))
    expect(await screen.findByText('Email o contraseña incorrectos. Te quedan 2 intento(s).')).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: 'Ingresar' }))
    expect(await screen.findByText('Email o contraseña incorrectos. Te quedan 1 intento(s).')).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: 'Ingresar' }))
    expect(
      await screen.findByText('Usuario bloqueado temporalmente por intentos fallidos. Intentá nuevamente en unos minutos.'),
    ).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: 'Ingresar' }))
    expect(mockedAuthService.login).toHaveBeenCalledTimes(3)
  })
})
