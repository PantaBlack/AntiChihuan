import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { Presupuesto } from '../../core/models/finance.models';

@Component({
  selector: 'app-presupuestos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <h4 class="fw-bold text-white mb-4">Control de Presupuestos Mensuales</h4>
    <div class="card card-dark p-4 mb-4">
      <form (ngSubmit)="guardar()" class="row g-3 align-items-end">
        <div class="col-md-3">
          <label class="form-label small">Mes</label>
          <input type="number" min="1" max="12" class="form-control bg-dark text-white border-secondary" [(ngModel)]="mes" name="mes" />
        </div>
        <div class="col-md-3">
          <label class="form-label small">Año</label>
          <input type="number" class="form-control bg-dark text-white border-secondary" [(ngModel)]="anio" name="anio" />
        </div>
        <div class="col-md-4">
          <label class="form-label small">Monto Límite Mensual (S/)</label>
          <input type="number" step="0.01" class="form-control bg-dark text-white border-secondary" [(ngModel)]="montoLimite" name="montoLimite" />
        </div>
        <div class="col-md-2">
          <button type="submit" class="btn btn-warning fw-bold text-dark w-100">Asignar</button>
        </div>
      </form>
    </div>

    <div class="row g-3">
      @for (p of presupuestos; track p.id) {
        <div class="col-md-6">
          <div class="card card-dark p-3 d-flex flex-row justify-content-between align-items-center">
            <div>
              <h6 class="fw-bold text-warning mb-1">{{ p.categoria?.nombre || 'Presupuesto General del Mes' }}</h6>
              <div class="text-white fw-bold">Límite: S/ {{ p.montoLimite | number:'1.2-2' }} ({{ p.mes }}/{{ p.anio }})</div>
            </div>
            <button (click)="eliminar(p.id)" class="btn btn-outline-danger btn-sm"><i class="bi bi-trash"></i></button>
          </div>
        </div>
      }
    </div>
  `
})
export class PresupuestosComponent implements OnInit {
  presupuestos: Presupuesto[] = [];
  mes = new Date().getMonth() + 1;
  anio = new Date().getFullYear();
  montoLimite = 2500;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.api.getPresupuestos(this.mes, this.anio).subscribe(res => (this.presupuestos = res));
  }

  guardar(): void {
    this.api.crearPresupuesto({ mes: this.mes, anio: this.anio, montoLimite: this.montoLimite }).subscribe(() => this.cargar());
  }

  eliminar(id: number): void {
    this.api.eliminarPresupuesto(id).subscribe(() => this.cargar());
  }
}
