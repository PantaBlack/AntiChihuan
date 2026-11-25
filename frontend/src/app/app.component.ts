import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    @if (auth.currentUser(); as user) {
      <div class="d-flex min-vh-100 position-relative" style="overflow-x: hidden;">
        <!-- Mobile Sidebar Backdrop -->
        @if (sidebarOpen) {
          <div class="d-lg-none position-fixed top-0 start-0 w-100 h-100 bg-black bg-opacity-75"
               style="z-index: 1040;"
               (click)="closeSidebar()"></div>
        }

        <!-- Sidebar Dark Fintech (Desktop: fija a la izquierda; Móvil: deslizable overlay) -->
        <aside class="border-end d-flex flex-column"
               [ngClass]="{
                 'd-none d-lg-flex': !sidebarOpen,
                 'position-fixed top-0 start-0 h-100 shadow-lg': sidebarOpen,
                 'position-relative': !sidebarOpen
               }"
               style="width: 270px; background-color: #090d16; border-color: #1e293b !important; z-index: 1050; flex-shrink: 0;">
          
          <!-- Header del Sidebar con botón de cerrar en móvil -->
          <div class="px-4 py-4 border-bottom d-flex align-items-center justify-content-between gap-2" style="border-color: #1e293b !important;">
            <div class="d-flex align-items-center gap-2">
              <span class="badge p-2 rounded-3" style="background: linear-gradient(135deg, #f59e0b, #d97706);">
                <i class="bi bi-shield-check fs-5 text-dark"></i>
              </span>
              <div>
                <span class="fs-5 fw-bold text-white">Anti<span class="text-warning">Chihuan</span></span>
                <div class="text-muted small" style="font-size: 0.72rem;">Angular 18 + Spring Boot 3</div>
              </div>
            </div>
            <!-- Botón cerrar (solo móvil) -->
            <button class="btn btn-sm btn-outline-secondary border-0 text-light d-lg-none" (click)="closeSidebar()">
              <i class="bi bi-x-lg fs-5"></i>
            </button>
          </div>

          <!-- Links de navegación con auto-cierre al hacer clic -->
          <div class="list-group list-group-flush my-3 px-3 gap-1">
            <a routerLink="/dashboard" routerLinkActive="bg-warning text-dark fw-bold" (click)="closeSidebar()"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-grid-1x2-fill"></i> Dashboard
            </a>
            <a routerLink="/transacciones" routerLinkActive="bg-warning text-dark fw-bold" (click)="closeSidebar()"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-arrow-left-right"></i> Transacciones
            </a>
            <a routerLink="/presupuestos" routerLinkActive="bg-warning text-dark fw-bold" (click)="closeSidebar()"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-pie-chart-fill"></i> Presupuestos
            </a>
            <a routerLink="/categorias" routerLinkActive="bg-warning text-dark fw-bold" (click)="closeSidebar()"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-tags-fill"></i> Categorías
            </a>
            <a routerLink="/ia" routerLinkActive="bg-warning text-dark fw-bold" (click)="closeSidebar()"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-robot"></i> Asistente IA
              <span class="badge bg-warning text-dark ms-auto" style="font-size: 0.65rem;">Gemini 3.8</span>
            </a>
          </div>

          <div class="px-4 mt-auto pt-3 pb-3 border-top small" style="border-color: #1e293b !important; color: #94a3b8;">
            <p class="mb-1 text-light fw-semibold"><i class="bi bi-people-fill text-warning me-1"></i> Equipo AntiChihuan</p>
            <div style="font-size: 0.72rem; line-height: 1.5;">
              • Alexis H. (IA Gemini)<br>
              • Anderson L. (Spring Security)<br>
              • Gianino N. (JPA & Finanzas)<br>
              • Samir R. (Chart.js & Cloud)<br>
              • Yimmy D. (Angular UI)
            </div>
          </div>
        </aside>

        <!-- Main Content Area -->
        <div class="flex-grow-1 d-flex flex-column min-w-0" style="width: 100%; max-width: 100%; overflow-x: hidden;">
          <!-- Topbar -->
          <nav class="navbar border-bottom px-3 px-md-4 py-2 py-md-3" style="background-color: #131b2e; border-color: #1e293b !important;">
            <div class="d-flex align-items-center gap-2">
              <!-- Botón Hamburguesa en Móvil -->
              <button class="btn btn-outline-secondary btn-sm d-lg-none text-light border-0 px-2"
                      (click)="toggleSidebar()"
                      aria-label="Abrir menú">
                <i class="bi bi-list fs-3"></i>
              </button>
              <div>
                <span class="text-muted small d-none d-md-block" style="font-size: 0.75rem;">Panel de Control Financiero</span>
                <h6 class="mb-0 fw-bold text-white text-truncate" style="max-width: 180px;">{{ user.nombreCompleto }}</h6>
              </div>
            </div>
            <div class="ms-auto d-flex align-items-center gap-2">
              <a routerLink="/transacciones" class="btn btn-warning btn-sm rounded-pill px-2 px-sm-3 fw-bold text-dark d-flex align-items-center gap-1">
                <i class="bi bi-plus-circle"></i>
                <span class="d-none d-sm-inline">Nuevo Movimiento</span>
                <span class="d-sm-none">Nuevo</span>
              </a>
              <button (click)="auth.logout()" class="btn btn-outline-danger btn-sm rounded-pill px-2 px-sm-3 d-flex align-items-center gap-1">
                <i class="bi bi-box-arrow-right"></i>
                <span class="d-none d-sm-inline">Salir</span>
              </button>
            </div>
          </nav>

          <main class="container-fluid px-3 px-md-4 py-3 py-md-4 flex-grow-1" style="max-width: 100%; overflow-x: hidden;">
            <router-outlet></router-outlet>
          </main>
        </div>
      </div>
    } @else {
      <!-- Public Layout (Landing, Login, Register) -->
      <div class="d-flex flex-column min-vh-100" style="overflow-x: hidden;">
        <!-- Public Navigation Bar -->
        <header>
          <nav class="navbar py-2 py-md-3 sticky-top" style="background-color: #090d16; border-bottom: 1px solid #1e293b;">
            <div class="container d-flex align-items-center justify-content-between">
              <a class="navbar-brand fw-bold d-flex align-items-center gap-2 m-0" routerLink="/">
                <span class="badge p-2 rounded-3" style="background: linear-gradient(135deg, #4f46e5, #06b6d4);">
                  <i class="bi bi-shield-check fs-5 text-white"></i>
                </span>
                <span class="fs-4 text-white">Anti<span class="text-warning">Chihuan</span></span>
              </a>
              <div class="d-flex align-items-center gap-2 gap-sm-3">
                <a routerLink="/" class="nav-link text-light px-2 fw-medium d-none d-md-inline">Inicio</a>
                <a routerLink="/login" class="btn btn-outline-light btn-sm px-2 px-sm-3 rounded-pill">Ingresar</a>
                <a routerLink="/register" class="btn btn-warning btn-sm px-2 px-sm-3 rounded-pill fw-bold text-dark shadow-sm">Registrarse</a>
              </div>
            </div>
          </nav>
        </header>

        <!-- Public Content Area -->
        <main class="flex-grow-1">
          <router-outlet></router-outlet>
        </main>

        <!-- Public Footer -->
        <footer class="footer py-4 border-top text-center small" style="background-color: #090d16; border-color: #1e293b !important; color: #64748b;">
          <div class="container">
            <p class="mb-1 text-light fw-bold">AntiChihuan &copy; 2026 - Universidad / Proyecto Integrador</p>
            <p class="text-muted mb-0">Desarrollado con Angular 18, Spring Boot 3 y Google Gemini AI.</p>
          </div>
        </footer>
      </div>
    }
  `
})
export class AppComponent {
  sidebarOpen = false;

  constructor(public auth: AuthService) {}

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebar(): void {
    this.sidebarOpen = false;
  }
}
