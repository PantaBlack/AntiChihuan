import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { Categoria } from '../../core/models/finance.models';

@Component({
  selector: 'app-categorias',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <h4 class="fw-bold text-white mb-4">Catálogo de Categorías Financieras</h4>

    <div class="card card-dark p-4 mb-4">
      <form (ngSubmit)="crear()" class="row g-3 align-items-end">
        <div class="col-md-5">
          <label class="form-label small">Nombre de Categoría Personalizada</label>
          <input type="text" class="form-control bg-dark text-white border-secondary" [(ngModel)]="nombre" name="nombre" required />
        </div>
        <div class="col-md-3">
          <label class="form-label small">Tipo</label>
          <select class="form-select bg-dark text-white border-secondary" [(ngModel)]="tipo" name="tipo">
            <option value="GASTO">Gasto</option>
            <option value="INGRESO">Ingreso</option>
          </select>
        </div>
        <div class="col-md-2">
          <label class="form-label small">Color</label>
          <input type="color" class="form-control form-control-color w-100" [(ngModel)]="colorHex" name="colorHex" />
        </div>
        <div class="col-md-2">
          <button type="submit" class="btn btn-warning fw-bold text-dark w-100">Crear</button>
        </div>
      </form>
    </div>

    <div class="row g-3">
      @for (c of categorias; track c.id) {
        <div class="col-md-4">
          <div class="card card-dark p-3 d-flex flex-row align-items-center justify-content-between">
            <div class="d-flex align-items-center gap-2">
              <span class="rounded-circle d-inline-block" style="width: 14px; height: 14px;" [style.background-color]="c.colorHex"></span>
              <span class="fw-semibold text-white">{{ c.nombre }}</span>
            </div>
            <span class="badge" [ngClass]="c.tipo === 'INGRESO' ? 'bg-success' : 'bg-danger'">{{ c.tipo }}</span>
          </div>
        </div>
      }
    </div>
  `
})
export class CategoriasComponent implements OnInit {
  categorias: Categoria[] = [];
  nombre = '';
  tipo: 'INGRESO' | 'GASTO' = 'GASTO';
  colorHex = '#f59e0b';

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.api.getCategorias().subscribe(res => (this.categorias = res));
  }

  crear(): void {
    this.api.crearCategoria({ nombre: this.nombre, tipo: this.tipo, colorHex: this.colorHex, icono: 'bi-tag' }).subscribe(() => {
      this.nombre = '';
      this.cargar();
    });
  }
}
