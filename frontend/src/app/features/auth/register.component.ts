import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="container py-5">
      <div class="row justify-content-center">
        <div class="col-lg-5">
          <div class="card card-dark p-4 shadow-lg border-secondary">
            <div class="text-center mb-3">
              <span class="badge p-3 rounded-4 mb-2" style="background: linear-gradient(135deg, #f59e0b, #d97706);">
                <i class="bi bi-person-plus-fill fs-3 text-dark"></i>
              </span>
              <h4 class="fw-bold text-white mb-1">Crear Cuenta en Anti<span class="text-warning">Chihuan</span></h4>
              <p class="text-muted small mb-0">Completa tus datos con validación segura</p>
            </div>

            @if (errorMsg) {
              <div class="alert alert-danger py-2 px-3 small rounded-3 mb-3 d-flex align-items-center gap-2">
                <i class="bi bi-exclamation-triangle-fill flex-shrink-0"></i>
                <div>{{ errorMsg }}</div>
              </div>
            }

            <form (ngSubmit)="onRegister()">
              <div class="mb-3">
                <label class="form-label small text-light fw-semibold">Nombre Completo</label>
                <input type="text" class="form-control bg-dark text-white border-secondary"
                       placeholder="Ejemplo: Juan Carlos Pérez"
                       [(ngModel)]="nombreCompleto" name="nombreCompleto" required />
                <div class="form-text small" style="color: #94a3b8; font-size: 0.75rem;">
                  Mínimo 3 caracteres, solo letras y espacios.
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label small text-light fw-semibold">Correo Electrónico</label>
                <input type="email" class="form-control bg-dark text-white border-secondary"
                       placeholder="ejemplo@correo.com"
                       [(ngModel)]="email" name="email" required />
                <div class="form-text small" style="color: #94a3b8; font-size: 0.75rem;">
                  Debe ser un correo válido con dominio (ejemplo&#64;dominio.com).
                </div>
              </div>

              <div class="mb-4">
                <label class="form-label small text-light fw-semibold">Contraseña</label>
                <input type="password" class="form-control bg-dark text-white border-secondary"
                       placeholder="Mínimo 6 caracteres"
                       [(ngModel)]="password" name="password" required />
                <div class="form-text small" style="color: #94a3b8; font-size: 0.75rem;">
                  Mínimo 6 caracteres, incluyendo al menos una letra y un número.
                </div>
              </div>

              <button type="submit" [disabled]="cargando" class="btn btn-warning w-100 fw-bold text-dark py-2">
                <i class="bi bi-check2-circle me-1"></i>
                {{ cargando ? 'Validando y Registrando...' : 'Registrarse en AntiChihuan' }}
              </button>
            </form>

            <div class="text-center mt-3 small">
              <a routerLink="/login" class="text-warning text-decoration-none">
                <i class="bi bi-arrow-left me-1"></i>¿Ya tienes cuenta? Inicia sesión aquí
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RegisterComponent {
  nombreCompleto = '';
  email = '';
  password = '';
  errorMsg = '';
  cargando = false;

  constructor(private auth: AuthService, private router: Router) {}

  onRegister(): void {
    this.errorMsg = '';

    const nombre = this.nombreCompleto.trim();
    const correo = this.email.trim();
    const pass = this.password;

    // 1. Validación de Nombre
    if (!nombre || nombre.length < 3) {
      this.errorMsg = 'El nombre completo debe tener al menos 3 caracteres.';
      return;
    }
    const nombreRegex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$/;
    if (!nombreRegex.test(nombre)) {
      this.errorMsg = 'El nombre completo solo puede contener letras y espacios.';
      return;
    }

    // 2. Validación de Correo Electrónico
    if (!correo) {
      this.errorMsg = 'El correo electrónico es obligatorio.';
      return;
    }
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(correo)) {
      this.errorMsg = 'Ingresa un correo electrónico válido (ejemplo: usuario@correo.com).';
      return;
    }

    // 3. Validación de Contraseña
    if (!pass || pass.length < 6) {
      this.errorMsg = 'La contraseña debe tener un mínimo de 6 caracteres.';
      return;
    }
    const passRegex = /^(?=.*[A-Za-z])(?=.*\d).+$/;
    if (!passRegex.test(pass)) {
      this.errorMsg = 'La contraseña debe contener al menos una letra y un número (ejemplo: Clave123).';
      return;
    }

    // Envío seguro al backend
    this.cargando = true;
    this.auth.register(nombre, correo, pass).subscribe({
      next: () => {
        this.cargando = false;
        this.router.navigate(['/dashboard']);
      },
      error: err => {
        this.cargando = false;
        this.errorMsg = err.error?.message || 'No se pudo completar el registro.';
      }
    });
  }
}
