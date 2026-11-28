import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { Categoria, Transaccion } from '../../core/models/finance.models';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-transacciones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
      <h4 class="fw-bold text-white mb-0 fs-5 fs-md-4">Gestión de Transacciones y Gastos Hormiga</h4>
      <div class="d-flex align-items-center gap-2">
        <button (click)="exportarExcel()" class="btn btn-success btn-sm d-flex align-items-center gap-1 shadow-sm fw-semibold">
          <i class="bi bi-file-earmark-excel-fill"></i> Exportar a Excel (.xlsx)
        </button>
        <button (click)="descargarCsv()" class="btn btn-outline-warning btn-sm d-flex align-items-center gap-1 shadow-sm">
          <i class="bi bi-file-earmark-spreadsheet me-1"></i> CSV
        </button>
      </div>
    </div>

    <!-- Formulario Reactivo de Registro -->
    <div class="card card-dark p-3 p-md-4 mb-4">
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
    <div class="card card-dark p-3 p-md-4">
      <div class="table-responsive">
        <table class="table table-dark table-hover align-middle mb-0 text-nowrap">
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

  exportarExcel(): void {
    if (!this.transacciones || this.transacciones.length === 0) return;

    const dataFilas = this.transacciones.map(t => ({
      'ID': t.id,
      'Fecha': t.fecha,
      'Tipo de Movimiento': t.tipo,
      'Categoría': t.categoria?.nombre || 'General',
      'Descripción': t.descripcion,
      'Monto (S/)': Number(t.monto),
      'Es Gasto Hormiga': t.esGastoHormiga ? 'Sí (Gasto Hormiga)' : 'No',
      'Notas': t.notas || '-'
    }));

    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(dataFilas);

    // Calcular totales para el cuadro de resumen financiero
    const totalIngresos = this.transacciones
      .filter(t => t.tipo === 'INGRESO')
      .reduce((acc, t) => acc + Number(t.monto), 0);
    const totalGastos = this.transacciones
      .filter(t => t.tipo === 'GASTO')
      .reduce((acc, t) => acc + Number(t.monto), 0);
    const totalHormiga = this.transacciones
      .filter(t => t.tipo === 'GASTO' && t.esGastoHormiga)
      .reduce((acc, t) => acc + Number(t.monto), 0);
    const balance = totalIngresos - totalGastos;

    // Agregar cuadro de resumen alineado al final de la hoja
    XLSX.utils.sheet_add_aoa(ws, [
      [],
      ['', '', '', 'CUADRO DE RESUMEN GENERAL', 'VALOR TOTAL (S/)'],
      ['', '', '', 'TOTAL INGRESOS (+)', totalIngresos],
      ['', '', '', 'TOTAL GASTOS (-)', totalGastos],
      ['', '', '', 'TOTAL FUGAS HORMIGA', totalHormiga],
      ['', '', '', 'BALANCE DISPONIBLE NETO', balance]
    ], { origin: -1 });

    // Definir anchos ordenados para cada columna
    ws['!cols'] = [
      { wch: 8 },  // ID
      { wch: 14 }, // Fecha
      { wch: 20 }, // Tipo
      { wch: 30 }, // Categoría
      { wch: 35 }, // Descripción
      { wch: 15 }, // Monto
      { wch: 20 }, // Es Gasto Hormiga
      { wch: 20 }  // Notas
    ];

    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Transacciones');

    const fechaHoy = new Date().toISOString().slice(0, 10);
    XLSX.writeFile(wb, `Transacciones_AntiChihuan_${fechaHoy}.xlsx`);
  }

  descargarCsv(): void {
    this.api.exportarCsv().subscribe(blob => {
      const csvBlob = new Blob([blob], { type: 'text/csv;charset=utf-8;' });
      const url = window.URL.createObjectURL(csvBlob);
      const a = document.createElement('a');
      const fechaHoy = new Date().toISOString().slice(0, 10);
      a.href = url;
      a.download = `Transacciones_AntiChihuan_${fechaHoy}.csv`;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }
}
