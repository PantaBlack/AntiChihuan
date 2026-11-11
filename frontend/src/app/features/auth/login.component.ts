import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="container py-5">
      <div class="row justify-content-center">
        <div class="col-lg-6">
          <div class="card card-dark p-4 shadow-lg">
            <div class="text-center mb-4">
              <span class="badge p-3 rounded-4 mb-2" style="background: linear-gradient(135deg, #f59e0b, #d97706);">
                <i class="bi bi-shield-check fs-3 text-dark"></i>
              </span>
              <h3 class="fw-bold text-white mb-1">Anti<span class="text-warning">Chihuan</span></h3>
              <p class="text-muted small mb-0">Inicia sesión en tu asesor financiero inteligente (Angular 18 + Spring Boot 3)</p>
            </div>

            @if (errorMsg) {
              <div class="alert alert-danger py-2 small">{{ errorMsg }}</div>
            }

            <form (ngSubmit)="onLogin()">
              <div class="mb-3">
                <label class="form-label small text-light">Correo electrónico</label>
                <input type="email" class="form-control bg-dark text-white border-secondary"
                       [(ngModel)]="email" name="email" required />
              </div>
              <div class="mb-3">
                <label class="form-label small text-light">Contraseña</label>
                <input type="password" class="form-control bg-dark text-white border-secondary"
                       [(ngModel)]="password" name="password" required />
              </div>
              <button type="submit" class="btn btn-warning w-100 fw-bold text-dark py-2">
                <i class="bi bi-box-arrow-in-right me-1"></i> Ingresar al Sistema
              </button>
            </form>

            <hr class="border-secondary my-4" />
            <p class="small text-warning fw-bold mb-2"><i class="bi bi-lightning-charge-fill"></i> Cuentas de Prueba Preconfiguradas (10 Perfiles - 1 Clic):</p>
            <div class="d-flex flex-wrap gap-1">
              @for (demo of demos; track demo.email) {
                <button type="button" (click)="usarDemo(demo.email)"
                        class="btn btn-outline-light btn-sm" style="font-size: 0.72rem;">
                  {{ demo.label }}
                </button>
              }
            </div>

            <div class="text-center mt-4 small">
              <a routerLink="/register" class="text-warning text-decoration-none">¿No tienes cuenta? Regístrate aquí</a>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  email = 'gianino@antichihuan.pe';
  password = 'Password123!';
  errorMsg = '';

  demos = [
    { label: '1. Gianino (Saludable)', email: 'gianino@antichihuan.pe' },
    { label: '2. Alexis (Gran Capital)', email: 'alexis@antichihuan.pe' },
    { label: '3. Anderson (Modo Chihuán)', email: 'anderson@antichihuan.pe' },
    { label: '4. Samir (Gastos Hormiga)', email: 'samir@antichihuan.pe' },
    { label: '5. Yimmy (Óptimo)', email: 'yimmy@antichihuan.pe' },
    { label: '6. Carlos Slim (Millonario)', email: 'carlos.slim@antichihuan.pe' },
    { label: '7. Valeria (Deudas Tarjetas)', email: 'valeria.deudas@antichihuan.pe' },
    { label: '8. Mateo (Delivery 51%)', email: 'mateo.delivery@antichihuan.pe' },
    { label: '9. Lucía (Ahorro Frugal)', email: 'lucia.frugal@antichihuan.pe' },
    { label: '10. Renzo (Crítico 202%)', email: 'renzo.chihuan@antichihuan.pe' }
  ];

  constructor(private auth: AuthService, private router: Router) {}

  usarDemo(email: string): void {
    this.email = email;
    this.password = 'Password123!';
    this.onLogin();
  }

  onLogin(): void {
    this.errorMsg = '';
    this.auth.login(this.email, this.password).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: err => (this.errorMsg = err.error?.message || 'Error al iniciar sesión.')
    });
  }
}
