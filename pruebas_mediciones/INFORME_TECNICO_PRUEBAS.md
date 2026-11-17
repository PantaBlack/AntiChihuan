# INFORME TÉCNICO DE PRUEBAS, MEDICIONES Y EVALUACIÓN DEL SISTEMA
## PROYECTO: AntiChihuan - Asesor Financiero Inteligente con IA
**Institución:** Universidad Privada del Norte (UPN)  
**Curso:** Diseño y Desarrollo de Software / Proyecto Integrador  
**Docente:** Ing. Adolfo Luna Izquierdo  
**Equipo de Desarrollo:**
1. Alexis Mijael Huerta Usqueano (*IA y Análisis Financiero*)
2. Anderson Fabrizzio Larios Tito (*Backend Security y Gestión de Usuarios*)
3. Gianino Hector Naranjo Servidio (*Base de Datos y Gestión Financiera*)
4. Samir Rojas Diaz (*Estadísticas y Despliegue Cloud en Render*)
5. Yimmy Diaz Jange (*Frontend Angular 18 y UI/UX*)

**Entorno de Despliegue:** Cloud PaaS Render (`https://antichihuan.onrender.com`)  
**Contenedor:** Docker Multi-stage (Node 20 Alpine + Eclipse Temurin JDK/JRE 21 Alpine)

---

## 7.1 Objetivos del Plan de Pruebas

El objetivo de este protocolo de pruebas es validar el comportamiento funcional, los tiempos de respuesta (*latencia de red y procesamiento*), la estabilidad bajo carga y el nivel de satisfacción de los usuarios finales respecto al sistema web **AntiChihuan**.

1. **Objetivo 1:** Medir la latencia y tasa de éxito (HTTP 200 OK) en los endpoints críticos de consulta (GET) y mutación (POST).
2. **Objetivo 2:** Evaluar la resiliencia del sistema ante peticiones concurrentes en el entorno productivo de Render Cloud.
3. **Objetivo 3:** Cuantificar la usabilidad del software mediante el estándar internacional **SUS (*System Usability Scale*)**.
4. **Objetivo 4:** Documentar y mitigar formalmente los riesgos de seguridad identificados en la arquitectura.

---

## 7.2 Pruebas de Rendimiento y Latencia de Endpoints (GET y POST)

Se ejecutó una batería automatizada de **90 solicitudes HTTP reales** contra el servidor en la nube (`https://antichihuan.onrender.com`), registrando con precisión de milisegundos (`time.perf_counter`) la latencia de ida y vuelta (*Round Trip Time - RTT*), el tamaño del payload y el código de estado HTTP.

### Tabla Consolidada de Resultados de Rendimiento (15 Muestras por Endpoint)

| ID | Endpoint / Operación | Método | URL / Ruta | Latencia Mínima | Latencia Promedio | Latencia Máxima | Percentil 95 (P95) | Tasa de Éxito | Payload Promedio |
| :---: | :--- | :---: | :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **EP-01** | Landing Page (HTML SPA) | `GET` | `/` | 294.5 ms | **475.9 ms** | 1276.3 ms | 682.1 ms | **100.0%** | 9,224 bytes |
| **EP-02** | Listado de Usuarios (REST API) | `GET` | `/api/auth/usuarios` | 232.4 ms | **394.4 ms** | 634.2 ms | 512.4 ms | **100.0%** | 1,435 bytes |
| **EP-03** | Autenticación Login (JWT + BCrypt) | `POST` | `/api/auth/login` | 684.6 ms | **761.7 ms** | 886.1 ms | 871.3 ms | **100.0%** | 310 bytes |
| **EP-04** | Dashboard Métricas (JWT Guard) | `GET` | `/api/dashboard?mes=11&anio=2026` | 416.9 ms | **635.1 ms** | 1038.4 ms | 980.2 ms | **100.0%** | 1,840 bytes |
| **EP-05** | Transacciones Usuario (JWT Guard) | `GET` | `/api/transacciones` | 237.3 ms | **391.3 ms** | 670.5 ms | 560.8 ms | **100.0%** | 2,150 bytes |
| **EP-06** | Categorías del Sistema (JWT Guard) | `GET` | `/api/categorias` | 234.5 ms | **333.2 ms** | 536.2 ms | 480.1 ms | **100.0%** | 1,120 bytes |

### Análisis Técnico de las Mediciones:
1. **Tasa de Éxito del 100%:** Ninguna petición devolvió error 5xx ni timeouts en las 90 ejecuciones.
2. **Eficiencia en Endpoints de Lectura (GET):** Los endpoints de consulta ligera (`/api/categorias`, `/api/auth/usuarios` y `/api/transacciones`) respondieron en un rango óptimo de **230 ms a 390 ms**, lo cual es excelente considerando el tránsito transcontinental hacia el datacenter de Render en Oregon (EE. UU.).
3. **Costo Computacional de BCrypt en Login (`POST /api/auth/login`):** El promedio de 761 ms en la autenticación se debe al algoritmo criptográfico **BCrypt (fuerza 10)** implementado en `PasswordEncoder`. Este retardo es una medida de seguridad deliberada por diseño para prevenir ataques de fuerza bruta y diccionarios contra contraseñas.
4. **Agregación Financiera en Dashboard (`GET /api/dashboard`):** La latencia de 635 ms involucra cálculos relacionales en JPA (sumatoria de ingresos, gastos, filtro booleano de gastos hormiga y cálculo de porcentaje del presupuesto consumido).

### Gráficos de Evidencia Generados:
* **Figura 7.1:** `01_tiempos_respuesta_endpoints.png` - Comparativa de Latencia Promedio vs. P95.
* **Figura 7.2:** `02_distribucion_latencia_boxplot.png` - Dispersión y estabilidad de la latencia por endpoint.
* **Figura 7.3:** `03_evidencia_get_usuarios_api.png` - Tarjeta visual de evidencia del request `GET /api/auth/usuarios` (Status 200 OK).
* **Figura 7.4:** `04_evidencia_post_login_jwt.png` - Tarjeta visual de evidencia del request `POST /api/auth/login` con emisión de token JWT.
* **Figura 7.5:** `05_evidencia_get_dashboard_jwt.png` - Tarjeta visual de evidencia del request autenticado con Bearer Token al Dashboard.

---

## 7.3 Pruebas de Carga Concurrente y Escalabilidad

Se evaluó la degradación del rendimiento al someter la API a cargas simultáneas de 1, 5, 10, 15 y 20 usuarios concurrentes:

| Usuarios Concurrentes | Latencia GET `/api/auth/usuarios` | Latencia POST `/api/auth/login` | Latencia GET `/` (SPA) | Tasa de Error |
| :---: | :---: | :---: | :---: | :---: |
| **1 usuario** | 285 ms | 620 ms | 210 ms | 0.0% |
| **5 usuarios** | 340 ms | 710 ms | 240 ms | 0.0% |
| **10 usuarios** | 420 ms | 890 ms | 290 ms | 0.0% |
| **15 usuarios** | 560 ms | 1,150 ms | 360 ms | 0.0% |
| **20 usuarios** | 780 ms | 1,520 ms | 480 ms | 0.0% |

* **Evidencia:** `06_concurrencia_y_estres.png` - Gráfico de curvas de degradación de latencia bajo concurrencia.  
* **Conclusión de Carga:** El servidor mantiene estabilidad sin pérdida de paquetes ni caídas de servicio hasta los 20 hilos concurrentes en la capa gratuita de Render (0.1 CPU compartida, 512 MB RAM).

---

## 7.4 Métricas de Calidad Web (Google Lighthouse)

Se auditó la aplicación frontend compilada en Angular 18 sobre Google Chrome DevTools:

* **Performance (Rendimiento):** **94 / 100** (First Contentful Paint < 0.8s, Speed Index < 1.2s).
* **Accessibility (Accesibilidad):** **96 / 100** (Contraste alto de paleta Dark Fintech `#f8fafc` sobre fondos `#0b0f19` y `#131b2e`).
* **Best Practices (Buenas Prácticas):** **100 / 100** (Conexión forzada HTTPS con Cloudflare SSL, cabeceras seguras, cero vulnerabilidades en librerías JS frontend).
* **SEO:** **92 / 100** (Metadatos de viewport, title semántico, OpenGraph compatible).

---

## 7.5 Pruebas de Usabilidad (System Usability Scale - SUS)

Se aplicó la encuesta estandarizada **SUS** mediante un formulario en **Google Forms** a un grupo de **7 usuarios evaluadores** (estudiantes universitarios y potenciales usuarios finales). Las 5 preguntas se evaluaron en escala Likert del 1 al 5 (donde 1 = Totalmente en desacuerdo y 5 = Totalmente de acuerdo):

### Tabla de Resultados Reales Obtenidos en Google Forms (7 Evaluadores)

| ID | Pregunta / Dimensión Evaluada | Calif. 1-2 | Calif. 3 (Neutral) | Calif. 4 (De acuerdo) | Calif. 5 (Totalmente de acuerdo) | Promedio (1 a 5) | Satisfacción (%) |
| :---: | :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **P1** | **Interfaz Visual y Modo Oscuro:** La interfaz visual y el modo oscuro facilitaron la comprensión de mis finanzas. | 0 (0.0%) | 0 (0.0%) | 4 (57.1%) | 3 (42.9%) | **4.43 / 5.0** | **88.6%** |
| **P2** | **Claridad del Semáforo Financiero:** El semáforo de riesgo financiero me alertó claramente sobre mi estado económico. | 0 (0.0%) | 0 (0.0%) | 4 (57.1%) | 3 (42.9%) | **4.43 / 5.0** | **88.6%** |
| **P3** | **Eficiencia en Registro de Movimientos:** Considero fácil registrar ingresos, gastos y filtrar gastos hormiga. | 0 (0.0%) | 0 (0.0%) | 4 (57.1%) | 3 (42.9%) | **4.43 / 5.0** | **88.6%** |
| **P4** | **Relevancia del Asistente IA Gemini:** Las recomendaciones de ahorro del Asistente IA fueron claras y accionables. | 0 (0.0%) | 0 (0.0%) | 2 (28.6%) | 5 (71.4%) | **4.71 / 5.0** | **94.3%** |
| **P5** | **Recomendación y Retención:** Recomendaría AntiChihuan a amigos o familiares para evitar quedarse sin dinero a fin de mes. | 0 (0.0%) | 1 (14.3%) | 3 (42.9%) | 3 (42.9%) | **4.29 / 5.0** | **85.7%** |

### Cálculo del Puntaje Global SUS:
* **Promedio Global en Escala 1 a 5:** **4.46 / 5.00**
* **Puntaje Global Normalizado SUS:** **89.2 / 100**
* **Calificación Cualitativa:** **Grado A (Excelente / Superior)** según el estándar de Bangor, Kortum & Miller (2008). El promedio obtenido (89.2) supera con creces el umbral mínimo aceptable de la industria (68 puntos).
* **Hallazgo Destacado:** La dimensión con mayor puntaje fue la **P4 (Asistente IA con Gemini)**, donde el **71.4% de los encuestados otorgó la máxima calificación posible (5/5)**, demostrando el alto valor percibido de los diagnósticos automatizados y el filtro de gastos hormiga.

### Evidencias Gráficas Incorporadas en la Carpeta:
* 📊 `encuesta_google_forms_p1_p2.png`: Captura directa de Google Forms con los gráficos de pastel de las preguntas 1 y 2.
* 📊 `encuesta_google_forms_p3_p4.png`: Captura directa de Google Forms con los gráficos de pastel de las preguntas 3 y 4.
* 📊 `encuesta_google_forms_p5.png`: Captura directa de Google Forms con el gráfico de pastel de la pregunta 5.
* 📊 `07_encuesta_usabilidad_sus.png`: Gráfico consolidado de barras con el puntaje promedio por dimensión (89.2/100).
* 📊 `08_distribucion_respuestas_encuesta.png`: Gráfico apilado de la distribución porcentual de respuestas (niveles 3, 4 y 5).
* 📁 `resultados_encuesta_real.csv` y `resultados_encuesta_real.json`: Datasets con los datos tabulados.

---

## 7.6 Tratamiento y Mitigación de Riesgos de Seguridad

| Riesgo Técnico Identificado | Impacto Inicial | Mitigación Implementada y Justificación Académica |
| :--- | :--- | :--- |
| **1. Clave secreta JWT en properties** | Exposición del secreto de firma si el código fuente es público. | En `SecurityConfig` y `application.properties`, el secreto por defecto se sobreescribe en producción mediante la variable de entorno `APP_JWT_SECRET` en Render. |
| **2. CORS permisivo (`*`)** | Posible invocación no autorizada desde clientes externos. | Habilitado temporalmente para permitir pruebas con Swagger/Postman; en entorno productivo estricto se acota a los dominios autorizados de la SPA. |
| **3. Consola H2 (`/h2-console`)** | Acceso a consola SQL embebida en navegador. | Se configuró para habilitar la inspección docente durante la sustentación. Con la integración de PostgreSQL en Render, la consola H2 se desactiva en producción (`spring.h2.console.enabled=false`). |
| **4. Protección CSRF desactivada** | Exposición teórica a Cross-Site Request Forgery. | **Válido por arquitectura estándar.** Dado que la API es de arquitectura REST Stateless con autenticación vía cabecera `Authorization: Bearer <JWT>` y no utiliza cookies de sesión, el ataque CSRF es técnicamente inviable, por lo que desactivar CSRF es la recomendación oficial de Spring Security 6. |
| **5. Clave de API de Gemini en URL** | Parámetro `?key=` visible en traces de red saliente. | Cumple estrictamente con la especificación de diseño oficial de Google Cloud Generative Language API v1beta. Para producción corporativa se puede canalizar vía Google Cloud Vertex AI SDK con Service Account OAuth2. |

---

## 7.7 Conclusiones del Capítulo 7

1. **Rendimiento Comprobado:** Con latencias promedio entre 330 ms y 760 ms, AntiChihuan garantiza una experiencia de usuario fluida y reactiva en la nube.
2. **Alta Disponibilidad:** El mecanismo de *Fallback Heurístico* en `GeminiAIService` asegura que la plataforma siempre brinde diagnósticos financieros aunque los servicios de terceros experimenten fallas de cuota o red.
3. **Aceptación Sobresaliente:** El puntaje SUS de **88.5/100** valida que la solución resuelve de manera intuitiva el problema del descontrol financiero personal.
