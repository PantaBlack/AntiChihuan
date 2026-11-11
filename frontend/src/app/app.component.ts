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
      <div class="d-flex" style="min-height: 100vh;">
        <!-- Sidebar Dark Fintech -->
        <aside class="border-end d-flex flex-column" style="width: 260px; background-color: #090d16; border-color: #1e293b !important;">
          <div class="px-4 py-4 border-bottom d-flex align-items-center gap-2" style="border-color: #1e293b !important;">
            <span class="badge p-2 rounded-3" style="background: linear-gradient(135deg, #f59e0b, #d97706);">
              <i class="bi bi-shield-check fs-5 text-dark"></i>
            </span>
            <div>
              <span class="fs-5 fw-bold text-white">Anti<span class="text-warning">Chihuan</span></span>
              <div class="text-muted small" style="font-size: 0.72rem;">Angular 18 + Spring Boot 3</div>
            </div>
          </div>

          <div class="list-group list-group-flush my-3 px-3 gap-1">
            <a routerLink="/dashboard" routerLinkActive="bg-warning text-dark fw-bold"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-grid-1x2-fill"></i> Dashboard
            </a>
            <a routerLink="/transacciones" routerLinkActive="bg-warning text-dark fw-bold"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-arrow-left-right"></i> Transacciones
            </a>
            <a routerLink="/presupuestos" routerLinkActive="bg-warning text-dark fw-bold"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-pie-chart-fill"></i> Presupuestos
            </a>
            <a routerLink="/categorias" routerLinkActive="bg-warning text-dark fw-bold"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-tags-fill"></i> Categorías
            </a>
            <a routerLink="/ia" routerLinkActive="bg-warning text-dark fw-bold"
               class="list-group-item list-group-item-action rounded-3 d-flex align-items-center gap-3 py-2 text-light border-0 bg-transparent">
              <i class="bi bi-robot"></i> Asistente IA
              <span class="badge bg-warning text-dark ms-auto" style="font-size: 0.65rem;">Gemini</span>
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

        <!-- Main Content -->
        <div class="flex-grow-1 d-flex flex-column">
          <nav class="navbar border-bottom px-4 py-3" style="background-color: #131b2e; border-color: #1e293b !important;">
            <div>
              <span class="text-muted small">Panel de Control Financiero Distribuido</span>
              <h6 class="mb-0 fw-bold text-white">{{ user.nombreCompleto }} ({{ user.email }})</h6>
            </div>
            <div class="ms-auto d-flex align-items-center gap-2">
              <a routerLink="/transacciones" class="btn btn-warning btn-sm rounded-pill px-3 fw-bold text-dark">
                <i class="bi bi-plus-circle me-1"></i> Nuevo Movimiento
              </a>
              <button (click)="auth.logout()" class="btn btn-outline-danger btn-sm rounded-pill px-3">
                <i class="bi bi-box-arrow-right me-1"></i> Salir
              </button>
            </div>
          </nav>

          <main class="container-fluid p-4 flex-grow-1">
            <router-outlet></router-outlet>
          </main>
        </div>
      </div>
    } @else {
      <router-outlet></router-outlet>
    }
  `
})
export class AppComponent {
  constructor(public auth: AuthService) {}
}
