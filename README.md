# 🛡️ AntiChihuan: Sistema Web Inteligente y Distribuido para el Control de Gastos Personales con IA (Angular 18 + Spring Boot 3)

Sistema web distribuido desarrollado con **Angular 18 (TypeScript + Bootstrap 5.3 + Chart.js)** en el Frontend, **Spring Boot 3.4 (Java 21 + Spring Data JPA + Spring Security JWT)** en el Backend, **SQL Server / H2 Database** y un **Agente de Inteligencia Artificial (Google Gemini `gemini-2.5-flash`)** para mitigar el descontrol financiero, detectar *gastos hormiga* y prevenir el temido *"Efecto Chihuán"* (llegar a fin de mes sin liquidez).

---

## 👥 Distribución de Roles y Módulos Desarrollados

| Integrante | Rol del Proyecto | Módulos y Componentes Implementados |
| :--- | :--- | :--- |
| **Alexis Mijael Huerta Usqueano** | **Inteligencia Artificial y Análisis Financiero** | • Servicio [`GeminiAIService.java`](backend/src/main/java/pe/antichihuan/service/GeminiAIService.java) con integración REST a Google Gemini (`gemini-2.5-flash`).<br>• Motor heurístico de diagnóstico financiero y detección de hábitos.<br>• Identificación de fugas por **Gastos Hormiga** y cálculo de ahorro sugerido.<br>• Controlador [`IAController.java`](backend/src/main/java/pe/antichihuan/controller/IAController.java) y componente Angular [`ia.component.ts`](frontend/src/app/features/ia/ia.component.ts). |
| **Anderson Fabrizzio Larios Tito** | **Backend Security y Gestión de Usuarios** | • Integración de **Spring Security** con autenticación **JWT Stateless** ([`SecurityConfig.java`](backend/src/main/java/pe/antichihuan/config/SecurityConfig.java), [`JwtFilter.java`](backend/src/main/java/pe/antichihuan/config/JwtFilter.java)).<br>• Entidad [`Usuario.java`](backend/src/main/java/pe/antichihuan/model/Usuario.java) y controlador [`AuthController.java`](backend/src/main/java/pe/antichihuan/controller/AuthController.java).<br>• Interceptor HTTP en Angular ([`auth.interceptor.ts`](frontend/src/app/core/interceptors/auth.interceptor.ts)) y Guard de rutas ([`auth.guard.ts`](frontend/src/app/core/guards/auth.guard.ts)). |
| **Gianino Hector Naranjo Servidio** | **Base de Datos y Gestión Financiera** | • Repositorios **Spring Data JPA** y modelado relacional con precisión `BigDecimal(18,2)`.<br>• Entidades JPA: [`Transaccion.java`](backend/src/main/java/pe/antichihuan/model/Transaccion.java), [`Categoria.java`](backend/src/main/java/pe/antichihuan/model/Categoria.java), [`Presupuesto.java`](backend/src/main/java/pe/antichihuan/model/Presupuesto.java).<br>• Seeding automático de 15 categorías y 10 perfiles en [`DataInitializer.java`](backend/src/main/java/pe/antichihuan/config/DataInitializer.java).<br>• Endpoints REST en [`TransaccionController.java`](backend/src/main/java/pe/antichihuan/controller/TransaccionController.java) y [`PresupuestoController.java`](backend/src/main/java/pe/antichihuan/controller/PresupuestoController.java). |
| **Samir Rojas Diaz** | **Estadísticas y Despliegue Cloud** | • Integración de gráficos interactivos con **Chart.js** en Angular ([`dashboard.component.ts`](frontend/src/app/features/dashboard/dashboard.component.ts)).<br>• Gráfico de dona (distribución de gastos por categoría) y barras (histórico 6 meses).<br>• Exportación de reportes a formato **CSV** descargable.<br>• Contenedorización con **Docker** (`Dockerfile` Multi-stage y `docker-compose.yml`). |
| **Yimmy Diaz Jange** | **Frontend Angular y Diseño UI/UX** | • SPA moderna y responsive en **Angular 18** con **Bootstrap 5.3 Dark Mode**, **Bootstrap Icons** y **Plus Jakarta Sans**.<br>• Dashboard principal con tarjetas KPI y **Semáforo Financiero AntiChihuan** (Verde = Excelente, Amarillo = Alerta, Rojo = Modo Chihuán).<br>• Formularios reactivos con selector dinámico de tipo y validaciones en cliente. |

---

## 🚀 Cómo Ejecutar el Proyecto

### 1. Ejecutar el Backend (Spring Boot 3)
```bash
cd backend
./mvnw spring-boot:run
```
El servidor API REST se iniciará en **`http://localhost:8080`** y sembrará automáticamente las 15 categorías predeterminadas y los 10 perfiles financieros de prueba.

### 2. Ejecutar el Frontend (Angular 18)
```bash
cd frontend
npm install
npm start
```
Abre tu navegador en **`http://localhost:4200`**.

---

## 🔑 Cuentas de Prueba Preconfiguradas (10 Perfiles)

Contraseña común para todas las cuentas: **`Password123!`**

1. **Gianino Hector Naranjo Servidio** (`gianino@antichihuan.pe`) — *Finanzas Saludables y Ahorro Sólido*
2. **Alexis Mijael Huerta Usqueano** (`alexis@antichihuan.pe`) — *Alto Patrimonio / Gran Capital*
3. **Anderson Fabrizzio Larios Tito** (`anderson@antichihuan.pe`) — *Deudas Críticas / Modo Chihuán*
4. **Samir Rojas Diaz** (`samir@antichihuan.pe`) — *Víctima de Gastos Hormiga*
5. **Yimmy Diaz Jange** (`yimmy@antichihuan.pe`) — *Control Presupuestal Óptimo*
6. **Carlos Slim Rodríguez** (`carlos.slim@antichihuan.pe`) — *Empresario Alto Capital*
7. **Valeria Mendoza Paz** (`valeria.deudas@antichihuan.pe`) — *Déficit por Tarjetas de Crédito*
8. **Mateo Quispe Flores** (`mateo.delivery@antichihuan.pe`) — *Exceso en Delivery y Antojos*
9. **Lucía Fernández Castro** (`lucia.frugal@antichihuan.pe`) — *Ahorradora Frugal (64% ahorro)*
10. **Renzo Balbuena Silva** (`renzo.chihuan@antichihuan.pe`) — *Presupuesto Excedido al 202%*
