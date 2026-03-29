import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Outlet, Route, Routes } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import Dashboard from '../../../pages/Dashboard'

vi.mock('../../../services/shipmentService', () => ({
  shipmentService: {
    getAllShipments: vi.fn(),
    searchByTrackingId: vi.fn(),
    registerShipment: vi.fn(),
  },
}))

vi.mock('../../../services/vehicleService', () => ({
  vehicleService: {
    getAllVehicles: vi.fn(),
    createVehicle: vi.fn(),
  },
}))

vi.mock('../../../services/branchService', () => ({
  branchService: {
    getAllBranches: vi.fn(),
  },
}))

import { shipmentService } from '../../../services/shipmentService'
import { vehicleService } from '../../../services/vehicleService'
import { branchService } from '../../../services/branchService'

const mockedShipmentService = shipmentService as unknown as {
  getAllShipments: ReturnType<typeof vi.fn>
}
const mockedVehicleService = vehicleService as unknown as {
  getAllVehicles: ReturnType<typeof vi.fn>
}
const mockedBranchService = branchService as unknown as {
  getAllBranches: ReturnType<typeof vi.fn>
}

function renderDashboardWithRole(role: 'supervisor' | 'operador') {
  const user = {
    id: 'u-1',
    name: 'Flor',
    lastname: 'Paez',
    email: 'flor@test.com',
    dni: '11111111',
    role,
  }

  render(
    <MemoryRouter initialEntries={['/app']}>
      <Routes>
        <Route element={<Outlet context={user} />}>
          <Route path="/app" element={<Dashboard />} />
        </Route>
      </Routes>
    </MemoryRouter>,
  )
}

describe('Dashboard branch permissions by role', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockedShipmentService.getAllShipments.mockResolvedValue([])
    mockedVehicleService.getAllVehicles.mockResolvedValue([])
    mockedBranchService.getAllBranches.mockResolvedValue([])
  })

  it('CP-53 muestra accion de registrar sucursal para supervisor', async () => {
    const user = userEvent.setup()

    renderDashboardWithRole('supervisor')

    await user.click(await screen.findByRole('tab', { name: /Sucursales/i }))

    expect(await screen.findByRole('button', { name: 'Registrar sucursal' })).toBeInTheDocument()
  })

  it('CP-54 oculta accion de registrar sucursal para operador', async () => {
    const user = userEvent.setup()

    renderDashboardWithRole('operador')

    const branchTab = (await screen.findAllByRole('tab', { name: /Sucursales/i }))[0]
    await user.click(branchTab)

    expect(screen.queryByRole('button', { name: 'Registrar sucursal' })).not.toBeInTheDocument()
  })
})
