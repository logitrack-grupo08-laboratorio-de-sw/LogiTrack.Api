import { render, screen, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'

vi.mock('../../../pages/landing/LandingPage', () => ({ default: () => <div>LANDING_PAGE</div> }))
vi.mock('../../../pages/LoginPage', () => ({ default: () => <div>LOGIN_PAGE</div> }))
vi.mock('../../../pages/RegisterPage', () => ({ default: () => <div>REGISTER_PAGE</div> }))
vi.mock('../../../pages/Dashboard', () => ({ default: () => <div>DASHBOARD_PAGE</div> }))
vi.mock('../../../pages/ShipmentDetail', () => ({ default: () => <div>SHIPMENT_DETAIL</div> }))
vi.mock('../../../pages/VehicleDetail', () => ({ default: () => <div>VEHICLE_DETAIL</div> }))
vi.mock('../../../pages/transportista/RoutesDashboard', () => ({ default: () => <div>TRANSPORTISTA_HOME</div> }))
vi.mock('../../../pages/transportista/RouteDetail', () => ({ default: () => <div>TRANSPORTISTA_DETAIL</div> }))
vi.mock('../../../components/Layout', () => ({
  default: () => <div>LAYOUT_WRAPPER</div>,
}))

import App from '../../../App'

describe('App route guards', () => {
  beforeEach(() => {
    localStorage.clear()
    window.history.pushState({}, '', '/app')
  })

  it('CP-11 bloquea acceso a ruta privada sin sesion y redirige al flujo publico', async () => {
    render(<App />)

    expect(await screen.findByText('LANDING_PAGE')).toBeInTheDocument()
  })

  it('CP-12 expira sesion activa y redirige a login', async () => {
    localStorage.setItem(
      'user',
      JSON.stringify({
        id: 'u-1',
        name: 'Florencia',
        lastname: 'Paez',
        email: 'florencia@gmail.com',
        dni: '11111111',
        role: 'operador',
      }),
    )

    render(<App />)

    expect(await screen.findByText('LAYOUT_WRAPPER')).toBeInTheDocument()

    window.dispatchEvent(new CustomEvent('auth:session-expired'))

    await waitFor(async () => {
      expect(await screen.findByText('LOGIN_PAGE')).toBeInTheDocument()
    })
  })
})
