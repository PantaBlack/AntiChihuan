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
          <div class="card card-dark p-4 shadow-lg">
            <h4 class="fw-bold text-white text-center mb-3">Crear Cuenta en Anti<span class="text-warning">Chihuan</span></h4>

            @if (errorMsg) {
              <div class="alert alert-danger py-2 small">{{ errorMsg }}</div>
            }

            <form (ngSubmit)="onRegister()">
              <div class="mb-3">
                <label class="form-label small">Nombre Completo</label>
                <input type="text" class="form-control bg-dark text-white border-secondary"
                       [(ngModel)]="nombreCompleto" name="nombreCompleto" required />
              </div>
              <div class="mb-3">
                <label class="form-label small">Correo Electrónico</label>
                <input type="email" class="form-control bg-dark text-white border-secondary"
                       [(ngModel)]="email" name="email" required />
              </div>
              <div class="mb-3">
                <label class="form-label small">Contraseña</label>
                <input type="password" class="form-control bg-dark text-white border-secondary"
                       [(ngModel)]="password" name="password" required />
              </div>
              <button type="submit" class="btn btn-warning w-100 fw-bold text-dark py-2">
                Registrarse
              </button>
            </form>

            <div class="text-center mt-3 small">
              <a routerLink="/login" class="text-warning text-decoration-none">Volver a Iniciar Sesión</a>
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

  constructor(private auth: AuthService, private router: Router) {}

  onRegister(): void {
    this.auth.register(this.nombreCompleto, this.email, this.password).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: err => (this.errorMsg = err.error?.message || 'No se pudo completar el registro.')
    });
  }
}
