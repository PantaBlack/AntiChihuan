# 🛡️ AntiChihuan: Sistema Web Inteligente para el Control de Gastos Personales con IA

Sistema web desarrollado con **ASP.NET Core MVC (.NET 10)**, **Entity Framework Core**, **SQL Server** y un **Agente de Inteligencia Artificial (Google Gemini)** para mitigar el descontrol financiero y prevenir el temido *"Efecto Chihuán"* (llegar a fin de mes sin dinero).

---

## 👥 Distribución de Roles y Módulos Desarrollados

| Integrante | Rol del Proyecto | Módulos y Componentes Implementados |
| :--- | :--- | :--- |
| **Alexis Mijael Huerta Usqueano** | **Inteligencia Artificial y Análisis Financiero** | • Servicio [`GeminiAIService`](file:///Services/GeminiAIService.cs) con integración REST a Google Gemini.<br>• Motor heurístico de diagnóstico financiero y detección de hábitos.<br>• Identificación de fugas por **Gastos Hormiga** y cálculo de ahorro sugerido.<br>• Controlador [`IAController`](file:///Controllers/IAController.cs) y vista interactiva con historial de alertas. |
| **Anderson Fabrizzio Larios Tito** | **Backend y Gestión de Usuarios** | • Integración de **ASP.NET Core Identity** con cookies seguras.<br>• Modelo [`ApplicationUser`](file:///Models/ApplicationUser.cs) y controlador [`AccountController`](file:///Controllers/AccountController.cs).<br>• Filtro de seguridad `[Authorize]` y aislamiento de datos por usuario.<br>• Páginas de inicio de sesión, registro y cierre de sesión. |
| **Gianino Hector Naranjo Servidio** | **Base de Datos y Gestión Financiera** | • Contexto [`ApplicationDbContext`](file:///Data/ApplicationDbContext.cs) con Fluent API y restricciones en SQL Server.<br>• Modelos de datos: [`Transaccion`](file:///Models/Transaccion.cs), [`Categoria`](file:///Models/Categoria.cs), [`Presupuesto`](file:///Models/Presupuesto.cs).<br>• Seeding automático de categorías predeterminadas (ingresos y gastos).<br>• CRUD completo en [`TransaccionesController`](file:///Controllers/TransaccionesController.cs) y [`PresupuestosController`](file:///Controllers/PresupuestosController.cs). |
| **Samir Rojas Diaz** | **Estadísticas y Despliegue** | • Integración de gráficos interactivos con **Chart.js**.<br>• Gráfico de dona (distribución de gastos por categoría) y barras (histórico 6 meses).<br>• Exportación de reportes a formato **CSV** descargable.<br>• Preparación para compilación y despliegue continuo en la nube. |
| **Yimmy Diaz Jange** | **Frontend y Diseño de Interfaz** | • Interfaz moderna y responsive con **Bootstrap 5**, **Bootstrap Icons** y **Plus Jakarta Sans**.<br>• Dashboard principal ([`Dashboard/Index.cshtml`](file:///Views/Dashboard/Index.cshtml)) con tarjetas KPI y diseño limpio.<br>• **Semáforo Financiero AntiChihuan** (Verde = Saludable, Amarillo = Alerta, Rojo = Modo Chihuán).<br>• Formularios con selector dinámico de tipo y validaciones client-side. |

---

## 🚀 Cómo Ejecutar el Proyecto

### 1. Requisitos Previos
* **.NET SDK** (.NET 8, 9 o 10).
* **SQL Server LocalDB** (instalado por defecto con Visual Studio) o SQL Server Express.

### 2. Iniciar la Aplicación
Abre una terminal en la raíz del proyecto y ejecuta:
```bash
dotnet run
```

La base de datos `AntiChihuanDb` se creará automáticamente en LocalDB en la primera ejecución, cargará las categorías base y generará un usuario demo con transacciones de ejemplo.

Accede desde tu navegador a:
👉 **`http://localhost:5171`** o **`https://localhost:7171`**

---

## 🔑 Cuentas de Prueba Preconfiguradas (10 Perfiles)

La contraseña común para todas las cuentas es: **`Password123!`**

### 👥 1. Integrantes del Equipo (5 Usuarios)
1. **Gianino Hector Naranjo Servidio** (`gianino@antichihuan.pe`)
   * **Perfil:** *Finanzas Saludables y Ahorro Sólido* (Ingresos: S/ 5,000 | Gastos: S/ 2,450 | Superávit: S/ +2,550).
2. **Alexis Mijael Huerta Usqueano** (`alexis@antichihuan.pe`)
   * **Perfil:** *Alto Patrimonio / Gran Capital* (Ingresos: S/ 16,500 | Gastos: S/ 5,800 | Superávit: S/ +10,700).
3. **Anderson Fabrizzio Larios Tito** (`anderson@antichihuan.pe`)
   * **Perfil:** *Deudas Críticas / Modo Chihuán* (Ingresos: S/ 2,400 | Gastos: S/ 3,920 en cuotas y préstamos | Déficit: S/ -1,520).
4. **Samir Rojas Diaz** (`samir@antichihuan.pe`)
   * **Perfil:** *Víctima de Gastos Hormiga* (Ingresos: S/ 3,300 | Gastos: S/ 3,180 | Fuga Hormiga: S/ 1,320 en Rappi, cafés y compras impulsivas).
5. **Yimmy Diaz Jange** (`yimmy@antichihuan.pe`)
   * **Perfil:** *Control Presupuestal Óptimo* (Ingresos: S/ 3,600 | Gastos: S/ 1,950 | Margen de ahorro de 45.8%).

### 🔥 2. Perfiles Financieros Extremos (5 Usuarios)
6. **Carlos Slim Rodríguez** (`carlos.slim@antichihuan.pe`): Empresario millonario con S/ 38,000 de ingresos mensuales y gran balance.
7. **Valeria Mendoza Paz** (`valeria.deudas@antichihuan.pe`): Quiebra inminente por tarjetas de crédito e intereses moratorios (Déficit S/ -2,750).
8. **Mateo Quispe Flores** (`mateo.delivery@antichihuan.pe`): 51.8% de su presupuesto gastado en apps de comida rápida y snacks.
9. **Lucía Fernández Castro** (`lucia.frugal@antichihuan.pe`): Ahorradora frugal que ahorra más del 64% de su salario.
10. **Renzo Balbuena Silva** (`renzo.chihuan@antichihuan.pe`): Descontrol financiero total con 202% de presupuesto reventado.

*(En la pantalla de Login tienes un selector rápido con 1-clic para autocompletar cualquiera de estas cuentas).*

---

## 🤖 Configuración de Google Gemini API (Opcional)

El sistema cuenta con un **motor heurístico inteligente** que funciona al 100% sin necesidad de API Key. 

Si deseas conectar el modelo de Google Gemini en vivo:
1. Obtén tu API Key gratuita en [Google AI Studio](https://aistudio.google.com/).
2. Ábre el archivo [`appsettings.json`](file:///appsettings.json) y coloca tu clave:
```json
"Gemini": {
  "ApiKey": "TU_API_KEY_AQUI",
  "Model": "gemini-2.5-flash"
}
```
3. Reinicia la aplicación (`dotnet run`). ¡El agente ahora responderá directamente con Gemini!

---

## 📁 Estructura del Proyecto

```
NoMasChihuan/
│
├── Controllers/
│   ├── AccountController.cs       # Autenticación y registro (Anderson)
│   ├── DashboardController.cs     # Panel principal y KPIs (Yimmy/Samir)
│   ├── TransaccionesController.cs # CRUD financiero y exportación CSV (Gianino/Samir)
│   ├── PresupuestosController.cs  # Gestión de presupuestos mensuales (Gianino)
│   ├── CategoriasController.cs    # Categorías fijas y personalizadas (Gianino)
│   ├── IAController.cs            # Módulo de Inteligencia Artificial (Alexis)
│   └── HomeController.cs          # Landing page y presentación pública
│
├── Models/
│   ├── ApplicationUser.cs         # Extensión de IdentityUser
│   ├── Categoria.cs               # Entidad de categorías
│   ├── Transaccion.cs             # Entidad de ingresos y gastos
│   ├── Presupuesto.cs             # Entidad de límites mensuales
│   ├── RecomendacionIA.cs         # Entidad de historial de IA
│   ├── TipoTransaccion.cs         # Enum Ingreso / Gasto
│   └── ViewModels/                # Modelos de vista para formularios y métricas
│
├── Data/
│   └── ApplicationDbContext.cs   # Contexto EF Core y Seed de datos iniciales
│
├── Services/
│   ├── IFinanceService.cs         # Interfaz de cálculos financieros
│   ├── FinanceService.cs          # Lógica de saldos, semáforo y estadísticas
│   ├── IAIService.cs              # Interfaz para diagnósticos inteligentes
│   └── GeminiAIService.cs         # Conexión Gemini API + fallback heurístico
│
├── Views/                         # Vistas Razor con Bootstrap 5 y Chart.js
└── wwwroot/                       # Archivos estáticos, CSS personalizado y JS
```
