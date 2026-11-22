import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <!-- Hero Section Principal -->
    <section class="py-5 text-white position-relative" style="background: radial-gradient(circle at 50% 20%, rgba(99, 102, 241, 0.18) 0%, transparent 65%), #0b0f19; border-bottom: 1px solid #1e293b;">
      <div class="container py-5 text-center">
        <!-- Badge Destacado -->
        <div class="d-inline-flex align-items-center gap-2 px-3 py-1 rounded-pill mb-4" 
             style="background-color: rgba(234, 179, 8, 0.15); border: 1px solid rgba(234, 179, 8, 0.4);">
          <i class="bi bi-stars text-warning fs-5"></i>
          <span class="text-warning fw-bold small">Potenciado con Google Gemini 3.8 Flash</span>
        </div>

        <!-- Título Principal -->
        <h1 class="display-3 fw-bold mb-4 tracking-tight" style="color: #ffffff; line-height: 1.15;">
          Toma el control de tu dinero y <br>
          <span style="background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 50%, #f97316 100%); -webkit-background-clip: text; -webkit-text-fill-color: transparent;">
            ¡no te quedes chihuán a fin de mes!
          </span>
        </h1>

        <!-- Párrafo Descriptivo -->
        <p class="lead mx-auto mb-5" style="max-width: 780px; color: #cbd5e1; font-size: 1.2rem; line-height: 1.6;">
          Sistema web inteligente para registrar tus ingresos, erradicar <strong class="text-white">gastos hormiga</strong>, establecer presupuestos mensuales y recibir recomendaciones automatizadas con IA antes de que tu dinero se agote.
        </p>

        <!-- Botones de Acción -->
        <div class="d-flex flex-wrap justify-content-center gap-3">
          <a routerLink="/register" class="btn btn-warning btn-lg px-5 py-3 fw-bold shadow-lg" style="color: #0b0f19; font-size: 1.05rem;">
            <i class="bi bi-rocket-takeoff-fill me-2"></i> Comenzar Gratis Ahora
          </a>
          <a routerLink="/login" class="btn btn-outline-light btn-lg px-5 py-3 fw-semibold" style="font-size: 1.05rem;">
            <i class="bi bi-box-arrow-in-right me-2 text-warning"></i> Iniciar Sesión
          </a>
        </div>
      </div>
    </section>

    <!-- Problemática que Resuelve -->
    <section class="py-5" style="background-color: #101626; border-bottom: 1px solid #1e293b;">
      <div class="container py-4">
        <div class="row align-items-center g-5">
          <div class="col-lg-6">
            <span class="badge px-3 py-2 rounded-pill fw-bold mb-3" style="background-color: rgba(99, 102, 241, 0.2); color: #a5b4fc; border: 1px solid rgba(99, 102, 241, 0.4);">
              <i class="bi bi-shield-exclamation me-1"></i> Problemática Real
            </span>
            <h2 class="display-6 fw-bold text-white mb-3">
              ¿Por qué el 80% de las personas no sabe en qué se fue su sueldo?
            </h2>
            <p style="color: #94a3b8; font-size: 1.05rem; line-height: 1.6;">
              La falta de registro diario ocasiona un desbalance silencioso. Las pequeñas compras diarias (snacks, taxis innecesarios, cafés de paso y compras impulsivas) se convierten en un drenaje masivo que te deja en números rojos.
            </p>

            <!-- Tarjeta Efecto Chihuán -->
            <div class="p-3 rounded-3 mt-4" style="background-color: rgba(244, 63, 94, 0.1); border: 1px solid rgba(244, 63, 94, 0.3);">
              <div class="d-flex align-items-center gap-3">
                <div class="stat-icon" style="background-color: #e11d48; color: white;">
                  <i class="bi bi-lightning-charge-fill fs-4"></i>
                </div>
                <div>
                  <h6 class="mb-1 fw-bold" style="color: #fda4af;">El Diagnóstico AntiChihuan</h6>
                  <p class="mb-0 small" style="color: #e2e8f0;">Nuestra IA evalúa tus fugas de capital y te avisa a tiempo mediante un semáforo de alerta temprana.</p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-6">
            <div class="row g-3">
              <div class="col-sm-6">
                <div class="card p-4 h-100">
                  <div class="stat-icon mb-3" style="background-color: rgba(99, 102, 241, 0.2); color: #818cf8;">
                    <i class="bi bi-arrow-left-right fs-4"></i>
                  </div>
                  <h5 class="fw-bold text-white mb-2">Control Total</h5>
                  <p class="small mb-0" style="color: #94a3b8;">Registro instantáneo y categorización automática de ingresos y gastos.</p>
                </div>
              </div>

              <div class="col-sm-6">
                <div class="card p-4 h-100">
                  <div class="stat-icon mb-3" style="background-color: rgba(16, 185, 129, 0.2); color: #34d399;">
                    <i class="bi bi-pie-chart-fill fs-4"></i>
                  </div>
                  <h5 class="fw-bold text-white mb-2">Presupuestos</h5>
                  <p class="small mb-0" style="color: #94a3b8;">Fija límites máximos por rubro y monitorea tu consumo en tiempo real.</p>
                </div>
              </div>

              <div class="col-sm-6">
                <div class="card p-4 h-100">
                  <div class="stat-icon mb-3" style="background-color: rgba(245, 158, 11, 0.2); color: #fbbf24;">
                    <i class="bi bi-robot fs-4"></i>
                  </div>
                  <h5 class="fw-bold text-white mb-2">IA Predictiva</h5>
                  <p class="small mb-0" style="color: #94a3b8;">Diagnósticos inteligentes y recomendaciones accionables para ahorrar más.</p>
                </div>
              </div>

              <div class="col-sm-6">
                <div class="card p-4 h-100">
                  <div class="stat-icon mb-3" style="background-color: rgba(244, 63, 94, 0.2); color: #fb7185;">
                    <i class="bi bi-cup-hot fs-4"></i>
                  </div>
                  <h5 class="fw-bold text-white mb-2">Gastos Hormiga</h5>
                  <p class="small mb-0" style="color: #94a3b8;">Filtro especializado para aislar esas compras micro que arruinan tu mes.</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Equipo y Roles del Proyecto -->
    <section class="py-5" style="background-color: #0b0f19; border-bottom: 1px solid #1e293b;">
      <div class="container py-4">
        <div class="text-center mb-5">
          <span class="badge px-3 py-2 rounded-pill fw-bold mb-2" style="background-color: rgba(6, 182, 212, 0.15); color: #22d3ee; border: 1px solid rgba(6, 182, 212, 0.3);">
            Arquitectura de Software y Roles
          </span>
          <h2 class="display-6 fw-bold text-white mt-1">Equipo de Desarrollo AntiChihuan</h2>
          <p class="mx-auto" style="max-width: 680px; color: #94a3b8;">
            Implementación modular en Angular 18 y Spring Boot 3 con responsabilidades técnicas especializadas:
          </p>
        </div>

        <div class="row g-4">
          <!-- Alexis Huerta -->
          <div class="col-lg-4 col-md-6">
            <div class="card p-4 h-100">
              <div class="d-flex align-items-center gap-3 mb-3">
                <div class="stat-icon" style="background-color: rgba(245, 158, 11, 0.2); color: #f59e0b;">
                  <i class="bi bi-cpu-fill fs-4"></i>
                </div>
                <div>
                  <h6 class="fw-bold text-white mb-0">Alexis Huerta</h6>
                  <span class="badge bg-warning text-dark fw-bold" style="font-size: 0.7rem;">Inteligencia Artificial</span>
                </div>
              </div>
              <p class="small mb-0" style="color: #cbd5e1; line-height: 1.5;">
                Integración del agente de IA con Google Gemini, detección de patrones de consumo y generación de alertas preventivas.
              </p>
            </div>
          </div>

          <!-- Anderson Larios -->
          <div class="col-lg-4 col-md-6">
            <div class="card p-4 h-100">
              <div class="d-flex align-items-center gap-3 mb-3">
                <div class="stat-icon" style="background-color: rgba(99, 102, 241, 0.2); color: #818cf8;">
                  <i class="bi bi-shield-lock-fill fs-4"></i>
                </div>
                <div>
                  <h6 class="fw-bold text-white mb-0">Anderson Larios</h6>
                  <span class="badge bg-primary text-white fw-bold" style="font-size: 0.7rem;">Backend Security y Usuarios</span>
                </div>
              </div>
              <p class="small mb-0" style="color: #cbd5e1; line-height: 1.5;">
                Control de autenticación con Spring Security y JWT, protección de endpoints REST y gestión de perfiles de usuario.
              </p>
            </div>
          </div>

          <!-- Gianino Naranjo -->
          <div class="col-lg-4 col-md-6">
            <div class="card p-4 h-100">
              <div class="d-flex align-items-center gap-3 mb-3">
                <div class="stat-icon" style="background-color: rgba(16, 185, 129, 0.2); color: #34d399;">
                  <i class="bi bi-database-fill-gear fs-4"></i>
                </div>
                <div>
                  <h6 class="fw-bold text-white mb-0">Gianino Naranjo</h6>
                  <span class="badge bg-success text-white fw-bold" style="font-size: 0.7rem;">BD y Gestión Financiera</span>
                </div>
              </div>
              <p class="small mb-0" style="color: #cbd5e1; line-height: 1.5;">
                Modelado de base de datos relacional con JPA/Hibernate, CRUD de ingresos/gastos, presupuestos y cálculo de saldos.
              </p>
            </div>
          </div>

          <!-- Samir Rojas -->
          <div class="col-lg-6 col-md-6">
            <div class="card p-4 h-100">
              <div class="d-flex align-items-center gap-3 mb-3">
                <div class="stat-icon" style="background-color: rgba(6, 182, 212, 0.2); color: #22d3ee;">
                  <i class="bi bi-bar-chart-line-fill fs-4"></i>
                </div>
                <div>
                  <h6 class="fw-bold text-white mb-0">Samir Rojas</h6>
                  <span class="badge bg-info text-dark fw-bold" style="font-size: 0.7rem;">Estadísticas y Despliegue</span>
                </div>
              </div>
              <p class="small mb-0" style="color: #cbd5e1; line-height: 1.5;">
                Gráficos dinámicos interactivos, reportes de consumo, contenedorización con Docker y despliegue cloud en Render.
              </p>
            </div>
          </div>

          <!-- Yimmy Diaz -->
          <div class="col-lg-6 col-md-12">
            <div class="card p-4 h-100">
              <div class="d-flex align-items-center gap-3 mb-3">
                <div class="stat-icon" style="background-color: rgba(244, 63, 94, 0.2); color: #fb7185;">
                  <i class="bi bi-palette-fill fs-4"></i>
                </div>
                <div>
                  <h6 class="fw-bold text-white mb-0">Yimmy Diaz</h6>
                  <span class="badge bg-danger text-white fw-bold" style="font-size: 0.7rem;">Frontend Angular y UI/UX</span>
                </div>
              </div>
              <p class="small mb-0" style="color: #cbd5e1; line-height: 1.5;">
                Diseño dark fintech premium en Angular 18, semáforo visual de alerta financiera, componentes standalone y experiencia responsiva.
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Banner Final CTA -->
    <section class="py-5" style="background: linear-gradient(180deg, #0b0f19 0%, #101626 100%);">
      <div class="container py-4 text-center">
        <h3 class="fw-bold text-white mb-3">¿Listo para transformar tus finanzas personales?</h3>
        <p class="text-muted mb-4 mx-auto" style="max-width: 600px;">
          Empieza hoy mismo a registrar tus movimientos y aprovecha los diagnósticos de inteligencia artificial.
        </p>
        <div class="d-flex justify-content-center gap-3">
          <a routerLink="/register" class="btn btn-warning px-4 py-2 fw-bold text-dark rounded-pill">
            <i class="bi bi-person-plus-fill me-1"></i> Registrarse Gratis
          </a>
          <a routerLink="/login" class="btn btn-outline-light px-4 py-2 rounded-pill">
            <i class="bi bi-box-arrow-in-right me-1 text-warning"></i> Iniciar Sesión
          </a>
        </div>
      </div>
    </section>
  `
})
export class HomeComponent {}
