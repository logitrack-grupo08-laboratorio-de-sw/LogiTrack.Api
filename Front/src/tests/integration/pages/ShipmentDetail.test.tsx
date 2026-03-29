import { render, screen } from '@testing-library/react'
import { MemoryRouter, Outlet, Route, Routes } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import ShipmentDetail from '../../../pages/ShipmentDetail'

vi.mock('../../../services/shipmentService', () => ({
  shipmentService: {
    getShipmentTracking: vi.fn(),
    changeShipmentStatus: vi.fn(),
    cancelShipment: vi.fn(),
    resendCancelledShipment: vi.fn(),
  },
}))

import { shipmentService } from '../../../services/shipmentService'

const mockedShipmentService = shipmentService as unknown as {
  getShipmentTracking: ReturnType<typeof vi.fn>
}

const baseShipment = {
  id: 's-1',
  trackingId: 'LOG-123',
  status: 'En tránsito',
  origin: 'Buenos Aires',
  destination: 'Cordoba',
  createdDate: '2026-03-28',
  lastUpdate: '2026-03-28',
  estimatedDelivery: '2026-03-30',
  weight: 12,
  description: 'Paquete de prueba',
  sender: {
    name: 'Remitente Uno',
    address: 'Calle 1',
    city: 'Buenos Aires',
    postalCode: '1000',
  },
  receiver: {
    name: 'Destinatario Uno',
    address: 'Calle 2',
    city: 'Cordoba',
    postalCode: '5000',
  },
} as const

function renderWithRole(role: 'supervisor' | 'operador' | 'transportista') {
  const user = {
    id: 'u-1',
    name: 'Flor',
    lastname: 'Paez',
    email: 'flor@test.com',
    dni: '11111111',
    role,
  }

  render(
    <MemoryRouter initialEntries={['/shipment/s-1']}>
      <Routes>
        <Route element={<Outlet context={user} />}>
          <Route path="/shipment/:id" element={<ShipmentDetail />} />
        </Route>
      </Routes>
    </MemoryRouter>,
  )
}

describe('ShipmentDetail role restrictions', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockedShipmentService.getShipmentTracking.mockResolvedValue(baseShipment)
  })

  it('CP-25 bloquea cambio de estado para rol operador', async () => {
    renderWithRole('operador')

    const button = await screen.findByRole('button', { name: '🔒 Sin permiso para cambiar estado' })
    expect(button).toBeDisabled()
  })
})
