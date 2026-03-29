# Matriz de trazabilidad Front - Semana 3

## Estado actual

| Historia | Caso de Prueba | Automatizacion | Archivo de test | Estado |
|---|---|---|---|---|
| G08-7 | CP-06 Login exitoso | Si | Front/src/tests/integration/pages/LoginPage.test.tsx | Implementado |
| G08-7 | CP-07 Email no registrado | Si | Front/src/tests/integration/pages/LoginPage.test.tsx | Implementado |
| G08-7 | CP-08 Password incorrecta | Si | Front/src/tests/integration/pages/LoginPage.test.tsx | Implementado |
| G08-7 | CP-09 Campos vacios login | Si | Front/src/tests/integration/pages/LoginPage.test.tsx | Implementado |
| G08-7 | CP-10 Bloqueo por intentos fallidos | Si | Front/src/tests/integration/pages/LoginPage.test.tsx | Implementado |
| G08-8 | CP-11 Acceso directo sin sesion | Si | Front/src/tests/integration/routing/AppRoutes.test.tsx | Implementado |
| G08-8 | CP-12 Expiracion de sesion | Si | Front/src/tests/integration/routing/AppRoutes.test.tsx | Implementado |
| G08-10 | CP-13 Busqueda por tracking ID | Si | Front/src/tests/unit/components/SearchBar.test.tsx | Implementado |
| G08-10 | CP-14 Busqueda por destinatario | Si | Front/src/tests/unit/components/SearchBar.test.tsx | Implementado |
| G08-10 | CP-15 Busqueda sin resultados (limpieza) | Si | Front/src/tests/unit/components/SearchBar.test.tsx | Implementado |
| G08-10 | CP-16 Campo de busqueda vacio | Si | Front/src/tests/unit/components/SearchBar.test.tsx | Implementado |
| G08-12 | CP-17 Registro envio exitoso | Si | Front/src/tests/unit/services/shipmentService.test.ts | Implementado |
| G08-12 | CP-18 Campos obligatorios vacios en envio | Si | Front/src/tests/integration/components/ShipmentForm.test.tsx | Implementado |
| G08-12 | CP-19 Campos opcionales vacios en envio | Si | Front/src/tests/integration/components/ShipmentForm.test.tsx | Implementado |
| G08-13 | CP-24 Cambio estado envio exitoso | Si | Front/src/tests/unit/services/shipmentService.test.ts | Implementado |
| G08-13 | CP-25 Restricciones por rol en cambio de estado de envio | Si | Front/src/tests/integration/pages/ShipmentDetail.test.tsx | Implementado |
| G08-13 | CP-28 Transicion invalida | Si | Front/src/tests/unit/services/shipmentService.test.ts | Implementado |
| G08-20 | CP-31 Registro vehiculo exitoso | Si | Front/src/tests/unit/services/vehicleService.test.ts | Implementado |
| G08-20 | CP-32 Patente duplicada | Si | Front/src/tests/unit/services/vehicleService.test.ts | Implementado |
| G08-20 | CP-34 Validacion de capacidad de carga | Si (parcial UI) | Front/src/tests/integration/components/VehicleForm.test.tsx | Implementado |
| G08-27 | CP-39 Generar ruta con envios | Si | Front/src/tests/unit/services/routeService.test.ts | Implementado |
| G08-27 | CP-40 Error por ruta sin envios | Si | Front/src/tests/integration/components/RoutesList.test.tsx | Implementado |
| G08-27 | CP-42 Restriccion operador en asignar ruta | Si | Front/src/tests/integration/components/RoutesList.test.tsx | Implementado |
| G08-29 | CP-43 Comenzar ruta | Si | Front/src/tests/unit/services/routeService.test.ts | Implementado |
| G08-30 | CP-79 Cancelar ruta con motivo | Si | Front/src/tests/unit/services/routeService.test.ts | Implementado |
| G08-34 | CP-47 Registrar sucursal exitoso | Si | Front/src/tests/unit/services/branchService.test.ts | Implementado |
| G08-34 | CP-48 Campos incompletos sucursal | Si | Front/src/tests/integration/components/BranchForm.test.tsx | Implementado |
| G08-34 | CP-49 Nombre sucursal duplicado | Si | Front/src/tests/unit/services/branchService.test.ts | Implementado |
| G08-34 | CP-50 Formato numerico en sucursal | Si | Front/src/tests/integration/components/BranchForm.test.tsx | Implementado |
| G08-35 | CP-51 Listar sucursales con datos | Si | Front/src/tests/unit/services/branchService.test.ts | Implementado |
| G08-35 | CP-52 Listado sucursales vacio | Si | Front/src/tests/unit/services/branchService.test.ts | Implementado |
| G08-35 | CP-53 Permisos sucursales para supervisor | Si | Front/src/tests/integration/pages/Dashboard.branchPermissions.test.tsx | Implementado |
| G08-35 | CP-54 Permisos sucursales para operador | Si | Front/src/tests/integration/pages/Dashboard.branchPermissions.test.tsx | Implementado |
| G08-37 | CP-57 Filtro de rutas sin resultados | Si | Front/src/tests/integration/components/RoutesList.test.tsx | Implementado |
| G08-37 | CP-58 Nomenclatura de estados de ruta | Si | Front/src/tests/integration/components/RoutesList.test.tsx | Implementado |
| G08-20 | CP-59 Registro de vehiculo exitoso | Si | Front/src/tests/integration/components/VehicleForm.test.tsx | Implementado |
| G08-20 | CP-60 Error por patente duplicada | Si | Front/src/tests/integration/components/VehicleForm.test.tsx | Implementado |
| G08-20 | CP-61 Validaciones de obligatorios en alta de vehiculo UI | Si | Front/src/tests/integration/components/VehicleForm.test.tsx | Implementado |
| G08-20 | CP-62 Capacidad numerica valida | Si (parcial UI) | Front/src/tests/integration/components/VehicleForm.test.tsx | Implementado |
| G08-25 | CP-73 Botones de accion visibles | Si | Front/src/tests/integration/components/TransportistasList.test.tsx | Implementado |
| G08-25 | CP-74 Listado transportistas vacio | Si | Front/src/tests/integration/components/TransportistasList.test.tsx | Implementado |
| G08-28 | CP-78 Error por falta de paquetes | Si | Front/src/tests/integration/components/RoutesList.test.tsx | Implementado |

## Pendientes recomendados (integration/UI)

- Sin pendientes para esta tanda (CP-25, CP-53, CP-54 y CP-61 implementados).

## Convencion de commit

- `test(front): CP-06 CP-07 login service`
- `test(front): CP-17 CP-24 shipment flows`
- `test(front): CP-47 CP-51 branch service`

Completar luego con:
- Commit SHA
- PR
- Evidencia
