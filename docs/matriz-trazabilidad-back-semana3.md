# Matriz de trazabilidad Back - Semana 3

## Estado actual

| Historia | Caso de Prueba | Automatizacion | Archivo de test | Estado |
|---|---|---|---|---|
| G08-12 | CP-17 Registro envio exitoso | Si | Back/tests/Back.UnitTests/Application/EnviosServiceTests.cs | Implementado |
| G08-13 | CP-24 Cambio de estado (en transito) | Si | Back/tests/Back.UnitTests/Domain/PaqueteTests.cs | Implementado |
| G08-13 | CP-27 Transicion valida EnSucursal -> EnTransito | Si | Back/tests/Back.UnitTests/Domain/PaqueteTests.cs | Implementado |
| G08-13 | CP-28 Transicion invalida (saltear pasos) | Si | Back/tests/Back.UnitTests/Domain/PaqueteTests.cs | Implementado |
| G08-13 | CP-26 Visualizacion de estado actualizado (nivel dominio) | Si | Back/tests/Back.UnitTests/Domain/PaqueteTests.cs | Implementado |
| G08-27 | CP-39 Generacion de ruta con envios | Si | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-27 | CP-40 Error por ruta sin envios (regla de finalizacion) | Si (parcial) | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-29 | CP-43 Inicio de ruta y cascada de estados | Si | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-31 | CP-83 Cierre de ruta con estados finales | Si | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-30 | CP-79 Cancelacion de ruta con motivo | Si | Back/tests/Back.UnitTests/Domain/RutaTests.cs | Implementado |
| G08-28 | CP-75 Reasignacion de transportista | Si | Back/tests/Back.UnitTests/Application/EnviosServiceTests.cs | Implementado |
| G08-28 | CP-76 Restriccion por transportista no disponible | Si | Back/tests/Back.UnitTests/Application/EnviosServiceTests.cs | Implementado |

## Suite actual

- Proyecto: `Back/tests/Back.UnitTests/Back.UnitTests.csproj`
- Total de tests: 13
- Estado: en verde

## Pendientes recomendados

- Integracion de controladores con `WebApplicationFactory` para validar status code y contratos HTTP.
- Casos de permisos por rol en endpoints.
- Casos de sucursales y vehiculos a nivel API.
- Pruebas de repositorios con base de datos de test.
