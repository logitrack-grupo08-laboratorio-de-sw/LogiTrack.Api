# Registro de Decisiones Arquitectónicas (ADRs) - LogiTrack

---

### ADR 001: Selección de React como Tecnología de Frontend
**Estado:** Aceptado
**Contexto:** Para el MVP del sistema LogiTrack, se requiere construir una interfaz de usuario dinámica y reactiva. El sistema maneja múltiples formularios interactivos (alta de envíos de paquetes, registro de transportistas, etc.) y requerirá actualizaciones de estado (seguimiento de Tracking IDs simulado). Además, es fundamental implementar un sistema de roles (Supervisor, Operador, Transportista) que restrinja el acceso a ciertas vistas.
**Decisión:** Se optó por utilizar React como la biblioteca principal para el desarrollo del frontend.
**Alternativas:**
* **Angular:** Aunque es un framework robusto y muy estructurado, su curva de aprendizaje es alta y resulta pesado para un MVP que requiere iteraciones rápidas.
**Consecuencias:**
* **Positivas:**
  * Reutilización de componentes: Permite crear una biblioteca de componentes base (botones, inputs, tablas de datos) que acelerará el desarrollo de las pantallas de gestión de envíos, vehículos y rutas.
  * Rendimiento: El uso del Virtual DOM asegura que los cambios de estado (como cuando un paquete pasa de "En Sucursal" a "En Tránsito") se reflejan en la interfaz de manera instantánea y fluida.
  * Ecosistema y Seguridad: Facilita la integración con librerías como para el manejo seguro de rutas privadas dependiendo del rol del usuario autenticado.
* **Negativas (Riesgos a mitigar):**
  * React no impone una arquitectura estricta (a diferencia de Angular), por lo que como equipo debemos definir la estructura de carpetas y el manejo del estado global.

---

### ADR 002: Selección de .NET como Framework de Backend
**Estado:** Aceptado
**Contexto:** El backend del sistema logístico debe ser capaz de procesar reglas de negocio complejas, como la "Máquina de Estados" para el ciclo de vida de los envíos de paquetes, la validación de unicidad de datos (DNI de transportistas, Patentes de vehículos) y la ejecución de transacciones en cascada (iniciar una ruta y actualizar todos sus envíos). Se necesita una plataforma robusta, escalable y con un fuerte tipado para minimizar errores en tiempo de ejecución.
**Decisión:** Se decidió utilizar .NET con el lenguaje C# para la construcción de la API RESTful del backend.
**Alternativas:**
* **Spring Boot (Java):** Descartado. Aunque ofrece una robustez transaccional similar a .NET, como equipo decidimos ir por una opción distinta a las que estamos acostumbrados y que nos permita experimentar y adquirir nuevos conocimientos.
* **Flask (Python):** Descartado. Aunque era una buena opción por su forma rápida y ligera para crear, al ser un "micro-framework" requiere ensamblar muchas librerías de terceros para lograr una arquitectura completa. Para manejar la complejidad de LogiTrack, .NET ofrece una estructura lista para usar y mucho más segura gracias al tipado fuerte de C# frente al tipado dinámico de Python.
**Consecuencias:**
* **Positivas:**
  * Tipado estricto y robustez: C# reduce significativamente los bugs en producción al detectar errores de tipo durante la compilación, algo crucial al manejar contratos de datos estructurados.
  * Manejo de Base de Datos: La integración nativa con Entity Framework Core facilita las transacciones ACID (asegurando que si falla la actualización de un paquete, no se inicie la ruta por error).
  * Seguridad Integrada: .NET proporciona herramientas nativas de primer nivel para la implementación de autenticación y autorización mediante JWT (JSON Web Tokens), facilitando el bloqueo de endpoints según el rol (ej: solo un Supervisor puede crear rutas).
* **Negativas (Riesgos a mitigar):**
  * La curva de aprendizaje inicial puede ser un poco más pronunciada y requiere escribir más código repetitivo en comparación con frameworks minimalistas, pero esto se compensa con la mantenibilidad a largo plazo.

---

### ADR 003: Selección de JWT (JSON Web Tokens) como capa de Autenticación
**Estado:** Aceptado
**Contexto:** El sistema expone una API RESTful que será consumida por la aplicación web en React. Es imperativo asegurar los endpoints y restringir el acceso basado en los roles de los usuarios (Control de Accesos), sin sobrecargar la memoria del servidor manteniendo sesiones activas.
**Decisión:** Se consideró implementar JWT (JSON Web Tokens) para la autenticación y autorización.
**Alternativas:**
* **Autenticación basada en Sesiones:** Descartada. Obliga al servidor a recordar el estado de cada usuario logueado, lo que dificulta la escalabilidad horizontal de la API y consume recursos innecesarios.
* **OAuth 2.0 / OpenID Connect:** Descartado para el MVP. Es un estándar ideal, pero añade una complejidad de infraestructura si no se requiere integración con inicios de sesión de terceros (ej. "Ingresar con Google").
**Consecuencias:**
* **Positivas:** Arquitectura sin estado, lo que permite escalar el backend fácilmente. El token viaja con la información del rol del usuario, agilizando las validaciones en el frontend y backend.
* **Negativas (Riesgos):** La invalidación de un token antes de su vencimiento es compleja. Se mitigará configurando tiempos de expiración cortos para los tokens de acceso.

---

### ADR 004: Selección de Netlify y Render como servicios de Despliegue (MVP)
**Estado:** Aceptado
**Contexto:** Para la etapa de Producto Mínimo Viable (MVP) de LogiTrack, se requiere desplegar el sistema en la nube de forma rápida, automatizada y con el menor costo inicial posible. Se necesita alojar la aplicación web estática y la API del backend.
**Decisión:** Se decidió utilizar una arquitectura de despliegue distribuido utilizando Netlify para el alojamiento del Frontend (React) y Render para el alojamiento del Backend (.NET).
**Alternativas:**
* **Microsoft Azure:** Descartado. Aunque tiene sinergia con .NET, el ecosistema de servicios integrados de AWS es de mayor familiarización del equipo por lo que se consideró una mejor opción.
* **AWS (Amazon Web Services):** Descartado para esta etapa inicial. Si bien ofrece la infraestructura más robusta y escalable del mercado, su configuración es compleja, requiere gestión manual de servicios (EC2, S3).
**Consecuencias:**
* **Positivas:** Despliegue instantáneo y automatizado en cada push a la rama principal (CI/CD integrado por defecto). Costo cero durante la fase de MVP gracias a las capas gratuitas (Free Tiers) de ambos servicios.
* **Negativas (Riesgos):** El plan gratuito de Render pone en estado de suspensión a las APIs que no reciben tráfico constante, lo que puede generar una demora de hasta 50 segundos en la primera petición del día.

---

### ADR 005: Selección de GitHub como control de versionado
**Estado:** Aceptado (con actualizaciones de CI)
**Contexto:** El equipo de desarrollo necesita un entorno centralizado para colaborar en el código fuente, realizar revisiones de código y preparar el terreno para integraciones automatizadas.
**Decisión:** Se optó por utilizar GitHub como plataforma de alojamiento y control de versiones.
**Alternativas:**
* **GitLab:** Considerado como fuerte alternativa por sus excelentes herramientas nativas de CI/CD, pero descartado ya que GitHub resulta más estándar y de adopción más rápida para el equipo.
**Consecuencias:**
* **Positivas:** Facilita la adopción de flujos de trabajo estándar (como GitFlow o GitHub Flow). Permite el uso futuro de GitHub Actions para automatizar los despliegues hacia cloud. 
  * *(Actualización: Se implementó exitosamente un pipeline de Integración Continua (CI) automatizado con GitHub Actions para validar compilaciones y pruebas unitarias).*
* **Negativas (Riesgos):** Dependencia de una plataforma externa en la nube para el resguardo del código fuente principal.

---

### ADR 006: Postergación de la Selección del Motor de Base de Datos y uso de Mocking (JSON)
**Estado:** Superado (Reemplazado por ADR 007)
**Contexto:** El desarrollo del MVP requiere validar rápidamente la lógica de negocio y la interfaz de usuario en React. Configurar y modelar una base de datos relacional completa, gestionar migraciones y configurar credenciales en la nube añade una capa de complejidad en etapas tempranas donde los modelos de dominio cambian constantemente.
**Decisión:** Se ha decidido postergar la selección e implementación de un motor de Base de Datos real para fases posteriores. Durante la fase actual, el sistema persistirá la información en un archivo estático local, actuando como un Mock de base de datos.
**Alternativas:**
* **SQL Server / PostgreSQL:** Descartados temporalmente. Si bien Entity Framework Core facilita la integración en .NET con estas bases de datos relacionales, el equipo prioriza la velocidad de entrega del prototipo visual y lógico.
**Consecuencias:**
* **Positivas:** Aceleración en los tiempos de desarrollo y pruebas. Permite al equipo de frontend consumir la API y validar flujos sin depender de la infraestructura de datos. Modificar el modelo de dominio no requiere ejecutar scripts de migración en esta etapa.
* **Negativas (Riesgos):** El sistema no es transaccionalmente seguro y el archivo mockeado podría corromperse. Los datos se reinician o pierden fácilmente entre despliegues. Esta deuda técnica tiene que ser saldada obligatoriamente implementando una base de datos relacional (ej. SQL Server).
  * *(Actualización: Esta deuda técnica fue saldada exitosamente en la iteración actual. Ver ADR 007).*

---

### ADR 007: Selección de Neon (Serverless Postgres) como Motor de Base de Datos (Actualización MVP)
**Estado:** Aceptado
**Contexto:** Tras validar la lógica de negocio del MVP utilizando un archivo JSON estático como Mock (según ADR 006), el sistema LogiTrack requiere transicionar hacia una persistencia de datos real y relacional. Dado que el sistema se encuentra desplegado en un ecosistema componible (Netlify + Render), se busca un motor de base de datos que ofrezca conexión en la nube sin costos fijos de infraestructura ni mantenimiento de servidores.
**Decisión:** Se ha decidido implementar **Neon**, una plataforma de base de datos PostgreSQL Serverless, como el motor de persistencia principal del sistema. El backend en .NET se conectará mediante su cadena de conexión estándar utilizando Entity Framework Core.
**Alternativas consideradas:**
* **AWS RDS:** Descartado. Requiere configuración de VPCs, gestión manual de instancias y puede incurrir en costos elevados y difíciles de predecir para un entorno universitario.
* **Supabase:** Descartado en esta iteración. Aunque ofrece PostgreSQL en la nube, es una plataforma *Backend-as-a-Service* (BaaS) completa que incluye módulos de autenticación y almacenamiento que el proyecto no utilizará, ya que esa responsabilidad recae enteramente en la API de .NET.
**Consecuencias:**
* **Positivas:** * Costo Cero y Serverless: Neon separa el almacenamiento del cómputo y permite el escalado a cero (Scale-to-Zero), ideal para el entorno de pruebas sin generar gastos inactivos.
  * Integración: Al ser PostgreSQL puro, la compatibilidad con .NET y Entity Framework (migraciones, consultas LINQ) es nativa.
* **Negativas (Riesgos):** Al igual que con el backend en Render, el escalado a cero implica que la primera consulta a la base de datos tras un periodo de inactividad experimentará una ligera latencia (*Cold Start*). Requiere gestionar cuidadosamente la cadena de conexión como secreto (Variable de Entorno) en Render.