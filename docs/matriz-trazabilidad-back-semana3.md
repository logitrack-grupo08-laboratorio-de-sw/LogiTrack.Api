# Matriz de trazabilidad Back - Semana 3

## Estado actual

| Historia | Caso de Prueba | Automatizacion | Archivo de test | Estado |
|---|---|---|---|---|
| Global | CP-01..CP-87 Catalogo completo Excel | Si (especificacion/trazabilidad) | Back/tests/Back.UnitTests/Specification/CasosPruebaExcelBackTests.cs | Implementado |
| G08-12 | CP-17 Registro envio exitoso | Si | Back/tests/Back.UnitTests/Application/EnviosServiceTests.cs | Implementado |
| G08-7 | CP-06 Login exitoso | Si | Back/tests/Back.UnitTests/Application/AuthServiceTests.cs | Implementado |
| G08-7 | CP-07 Email no registrado | Si | Back/tests/Back.UnitTests/Application/AuthServiceTests.cs | Implementado |
| G08-7 | CP-08 Password incorrecta | Si | Back/tests/Back.UnitTests/Application/AuthServiceTests.cs | Implementado |
| G08-13 | CP-24 Cambio de estado (en transito) | Si | Back/tests/Back.UnitTests/Domain/PaqueteTests.cs | Implementado |
| G08-13 | CP-27 Transicion valida EnSucursal -> EnTransito | Si | Back/tests/Back.UnitTests/Domain/PaqueteTests.cs | Implementado |
| G08-13 | CP-28 Transicion invalida (saltear pasos) | Si | Back/tests/Back.UnitTests/Domain/PaqueteTests.cs | Implementado |
| G08-13 | CP-26 Visualizacion de estado actualizado (nivel dominio) | Si | Back/tests/Back.UnitTests/Domain/PaqueteTests.cs | Implementado |
| G08-27 | CP-39 Generacion de ruta con envios | Si | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-27 | CP-40 Error por ruta sin envios (regla de finalizacion) | Si (parcial) | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-27 | CP-40 Error por ruta sin envios (validacion al crear) | Si | Back/tests/Back.UnitTests/Application/RutasServiceTests.cs | Implementado |
| G08-29 | CP-43 Inicio de ruta y cascada de estados | Si | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-31 | CP-83 Cierre de ruta con estados finales | Si | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-30 | CP-79 Cancelacion de ruta con motivo | Si | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-28 | CP-75 Reasignacion de transportista | Si | Back/tests/Back.UnitTests/Application/EnviosServiceTests.cs | Implementado |
| G08-28 | CP-76 Restriccion por transportista no disponible | Si | Back/tests/Back.UnitTests/Application/EnviosServiceTests.cs | Implementado |
| G08-27 | CP-42 Restriccion operador en asignar ruta (transportista no disponible) | Si | Back/tests/Back.UnitTests/Application/RutasServiceTests.cs | Implementado |
| G08-27 | CP-39 Generacion de ruta con envios (nivel servicio) | Si | Back/tests/Back.UnitTests/Application/RutasServiceTests.cs | Implementado |
| G08-20 | CP-31 Registro de usuario operador/supervisor (nivel servicio auth) | Si | Back/tests/Back.UnitTests/Application/AuthServiceTests.cs | Implementado |

## Suite actual

- Proyecto: `Back/tests/Back.UnitTests/Back.UnitTests.csproj`
- Total de tests: 112
- Estado: en verde

## Pendientes recomendados

- Profundizar cobertura funcional end-to-end por CP con tests de integracion (`WebApplicationFactory`) para validar contratos HTTP, roles y persistencia real.
