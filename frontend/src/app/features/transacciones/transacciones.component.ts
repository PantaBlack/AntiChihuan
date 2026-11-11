import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { Categoria, Transaccion } from '../../core/models/finance.models';

@Component({
  selector: 'app-transacciones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="fw-bold text-white mb-0">Gestión de Transacciones y Gastos Hormiga</h4>
      <button (click)="descargarCsv()" class="btn btn-outline-warning btn-sm">
        <i class="bi bi-file-earmark-spreadsheet me-1"></i> Exportar CSV
      </button>
    </div>

    <!-- Formulario Reactivo de Registro -->
    <div class="card card-dark p-4 mb-4">
      <h6 class="fw-bold text-warning mb-3">Registrar Nuevo Movimiento</h6>
      <form (ngSubmit)="guardar()" class="row g-3">
        <div class="col-md-3">
          <label class="form-label small">Descripción</label>
          <input type="text" class="form-control bg-dark text-white border-secondary" [(ngModel)]="nuevo.descripcion" name="descripcion" required />
        </div>
        <div class="col-md-2">
          <label class="form-label small">Monto (S/)</label>
          <input type="number" step="0.01" class="form-control bg-dark text-white border-secondary" [(ngModel)]="nuevo.monto" name="monto" required />
        </div>
        <div class="col-md-2">
          <label class="form-label small">Tipo</label>
          <select class="form-select bg-dark text-white border-secondary" [(ngModel)]="nuevo.tipo" name="tipo" (change)="cargarCategorias()">
            <option value="GASTO">Gasto</option>
            <option value="INGRESO">Ingreso</option>
          </select>
        </div>
        <div class="col-md-3">
          <label class="form-label small">Categoría</label>
          <select class="form-select bg-dark text-white border-secondary" [(ngModel)]="nuevo.categoriaId" name="categoriaId" required>
            @for (c of categorias; track c.id) {
              <option [value]="c.id">{{ c.nombre }}</option>
            }
          </select>
        </div>
        <div class="col-md-2 d-flex align-items-end">
          <button type="submit" class="btn btn-warning fw-bold text-dark w-100">Guardar</button>
        </div>
        <div class="col-12">
          <div class="form-check form-switch">
            <input class="form-check-input" type="checkbox" [(ngModel)]="nuevo.esGastoHormiga" name="esGastoHormiga" id="chkHormiga">
            <label class="form-check-label small text-warning" for="chkHormiga">
              ¿Marcar como Gasto Hormiga 🐜? (Snacks, cafés, antojos, delivery)
            </label>
          </div>
        </div>
      </form>
    </div>

    <!-- Tabla de Movimientos -->
    <div class="card card-dark p-4">
      <table class="table table-dark table-hover align-middle mb-0">
        <thead>
          <tr>
            <th>Fecha</th>
            <th>Descripción</th>
            <th>Categoría</th>
            <th class="text-end">Monto</th>
            <th class="text-end">Acciones</th>
          </tr>
        </thead>
        <tbody>
          @for (t of transacciones; track t.id) {
            <tr>
              <td class="small text-muted">{{ t.fecha }}</td>
              <td>
                <span class="fw-semibold">{{ t.descripcion }}</span>
                @if (t.esGastoHormiga) {
                  <span class="badge bg-warning text-dark ms-2">Hormiga 🐜</span>
                }
              </td>
              <td>{{ t.categoria?.nombre }}</td>
              <td class="text-end fw-bold" [ngClass]="t.tipo === 'INGRESO' ? 'text-success' : 'text-danger'">
                {{ t.tipo === 'INGRESO' ? '+' : '-' }} S/ {{ t.monto | number:'1.2-2' }}
              </td>
              <td class="text-end">
                <button (click)="eliminar(t.id)" class="btn btn-outline-danger btn-sm">
                  <i class="bi bi-trash"></i>
                </button>
              </td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  `
})
export class TransaccionesComponent implements OnInit {
  transacciones: Transaccion[] = [];
  categorias: Categoria[] = [];
  nuevo = {
    descripcion: '',
    monto: 0,
    fecha: new Date().toISOString().substring(0, 10),
    tipo: 'GASTO',
    esGastoHormiga: false,
    categoriaId: 0
  };

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.cargarTodo();
    this.cargarCategorias();
  }

  cargarTodo(): void {
    this.api.getTransacciones().subscribe(res => (this.transacciones = res));
  }

  cargarCategorias(): void {
    this.api.getCategorias(this.nuevo.tipo).subscribe(cats => {
      this.categorias = cats;
      if (cats.length > 0) this.nuevo.categoriaId = cats[0].id;
    });
  }

  guardar(): void {
    this.api.crearTransaccion(this.nuevo).subscribe(() => {
      this.nuevo.descripcion = '';
      this.nuevo.monto = 0;
      this.cargarTodo();
    });
  }

  eliminar(id: number): void {
    this.api.eliminarTransaccion(id).subscribe(() => this.cargarTodo());
  }

  descargarCsv(): void {
    this.api.exportarCsv().subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'Transacciones_AntiChihuan.csv';
      a.click();
    });
  }
}
