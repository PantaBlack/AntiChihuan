import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { DashboardData } from '../../core/models/finance.models';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    @if (data) {
      <div class="d-flex flex-wrap align-items-center justify-content-between gap-3 mb-4">
        <div>
          <h2 class="fw-bold mb-1 text-white fs-3 fs-md-2">Hola, {{ data.usuarioNombre }} 👋</h2>
          <p class="text-muted small mb-0">Resumen financiero para <strong class="text-warning">{{ data.nombreMesActual }} {{ data.anioActual }}</strong></p>
        </div>
        <a routerLink="/ia" class="btn btn-warning btn-sm fw-bold text-dark px-3 shadow-sm">
          <i class="bi bi-stars me-1"></i> Diagnóstico IA Gemini
        </a>
      </div>

      <!-- Semáforo Financiero AntiChihuan -->
      <div class="p-3 rounded-3 mb-4 d-flex align-items-center gap-3"
           [ngStyle]="{
             'background-color': data.estadoSemaforo === 'CHIHUAN' ? 'rgba(239,68,68,0.15)' : data.estadoSemaforo === 'ALERTA' ? 'rgba(245,158,11,0.15)' : 'rgba(16,185,129,0.15)',
             'border': data.estadoSemaforo === 'CHIHUAN' ? '1px solid #ef4444' : data.estadoSemaforo === 'ALERTA' ? '1px solid #f59e0b' : '1px solid #10b981'
           }">
        <i class="bi fs-2"
           [ngClass]="data.estadoSemaforo === 'CHIHUAN' ? 'bi-exclamation-diamond-fill text-danger' : data.estadoSemaforo === 'ALERTA' ? 'bi-exclamation-triangle-fill text-warning' : 'bi-check-circle-fill text-success'"></i>
        <div class="flex-grow-1">
          <div class="fw-bold small text-white">SEMÁFORO FINANCIERO: {{ data.estadoSemaforo }}</div>
          <div class="small text-light">{{ data.mensajeSemaforo }}</div>
        </div>
      </div>

      <!-- 4 Tarjetas KPI -->
      <div class="row g-2 g-md-3 mb-4">
        <div class="col-6 col-lg-3">
          <div class="card card-dark p-3 h-100">
            <span class="small text-muted" style="font-size: 0.78rem;">Ingresos del Mes</span>
            <h3 class="fw-bold text-success mt-1 fs-5 fs-md-4">S/ {{ data.totalIngresosMes | number:'1.2-2' }}</h3>
          </div>
        </div>
        <div class="col-6 col-lg-3">
          <div class="card card-dark p-3 h-100">
            <span class="small text-muted" style="font-size: 0.78rem;">Gastos del Mes</span>
            <h3 class="fw-bold text-danger mt-1 fs-5 fs-md-4">S/ {{ data.totalGastosMes | number:'1.2-2' }}</h3>
          </div>
        </div>
        <div class="col-6 col-lg-3">
          <div class="card card-dark p-3 h-100">
            <span class="small text-muted" style="font-size: 0.78rem;">Balance Disponible</span>
            <h3 class="fw-bold mt-1 fs-5 fs-md-4" [ngClass]="data.balanceMes >= 0 ? 'text-white' : 'text-danger'">
              S/ {{ data.balanceMes | number:'1.2-2' }}
            </h3>
          </div>
        </div>
        <div class="col-6 col-lg-3">
          <div class="card card-dark p-3 h-100">
            <span class="small text-muted" style="font-size: 0.78rem;">Fuga Gastos Hormiga 🐜</span>
            <h3 class="fw-bold text-warning mt-1 fs-5 fs-md-4">S/ {{ data.totalGastosHormigaMes | number:'1.2-2' }}</h3>
            <span class="text-muted" style="font-size: 0.72rem;">{{ data.porcentajeGastosHormiga }}% del gasto total</span>
          </div>
        </div>
      </div>

      <!-- Gráficos Chart.js -->
      <div class="row g-4 mb-4">
        <div class="col-lg-5">
          <div class="card card-dark p-3 p-md-4 h-100">
            <div class="d-flex align-items-center justify-content-between mb-3 flex-wrap gap-2">
              <h6 class="fw-bold text-white mb-0">
                <i class="bi bi-pie-chart-fill text-warning me-1"></i>
                Distribución por Categoría
              </h6>
              <!-- Switcher Gastos / Ingresos -->
              <div class="btn-group btn-group-sm" role="group">
                <button type="button" class="btn fw-semibold"
                        [ngClass]="tipoVistaGrafico === 'GASTOS' ? 'btn-danger' : 'btn-outline-secondary text-light'"
                        (click)="cambiarVistaGrafico('GASTOS')">
                  Gastos ({{ data.gastosPorCategoria.length }})
                </button>
                <button type="button" class="btn fw-semibold"
                        [ngClass]="tipoVistaGrafico === 'INGRESOS' ? 'btn-success' : 'btn-outline-secondary text-light'"
                        (click)="cambiarVistaGrafico('INGRESOS')">
                  Ingresos ({{ data.ingresosPorCategoria?.length || 0 }})
                </button>
              </div>
            </div>

            @if (itemsGraficoActual.length > 0) {
              <div style="height: 220px; position: relative; width: 100%;">
                <canvas #donaCanvas></canvas>
              </div>

              <!-- Desglose de categorías con montos y porcentajes legibles -->
              <div class="mt-3 pt-3 border-top border-secondary border-opacity-25" style="max-height: 200px; overflow-y: auto;">
                <div class="d-flex flex-column gap-2">
                  @for (cat of itemsGraficoActual; track cat.categoria) {
                    <div class="d-flex align-items-center justify-content-between p-2 rounded-2" style="background: rgba(255,255,255,0.03);">
                      <div class="d-flex align-items-center gap-2 text-truncate me-2">
                        <span class="rounded-circle d-inline-block flex-shrink-0" [style.background-color]="cat.colorHex" style="width: 10px; height: 10px;"></span>
                        <i [class]="'bi ' + cat.icono" class="text-secondary small"></i>
                        <span class="small text-white text-truncate">{{ cat.categoria }}</span>
                      </div>
                      <div class="text-end flex-shrink-0">
                        <span class="small fw-bold text-light">S/ {{ cat.monto | number:'1.2-2' }}</span>
                        <span class="badge bg-dark text-muted ms-2 border border-secondary border-opacity-25" style="font-size: 0.7rem;">{{ cat.porcentaje }}%</span>
                      </div>
                    </div>
                  }
                </div>
              </div>
            } @else {
              <div class="text-center py-5 text-muted">
                <i class="bi bi-pie-chart fs-1 d-block mb-2 text-secondary opacity-50"></i>
                <p class="small mb-0">No hay {{ tipoVistaGrafico === 'GASTOS' ? 'gastos' : 'ingresos' }} registrados en este mes.</p>
              </div>
            }
          </div>
        </div>
        <div class="col-lg-7">
          <div class="card card-dark p-3 p-md-4 h-100">
            <h6 class="fw-bold text-white mb-3">
              <i class="bi bi-bar-chart-fill text-info me-1"></i>
              Ingresos vs Gastos (Últimos 6 Meses)
            </h6>
            <div style="height: 320px; min-height: 260px; position: relative; width: 100%;">
              <canvas #barrasCanvas></canvas>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class DashboardComponent implements OnInit {
  data?: DashboardData;
  tipoVistaGrafico: 'GASTOS' | 'INGRESOS' = 'GASTOS';
  private donaChartInstance?: Chart;
  private barrasChartInstance?: Chart;

  @ViewChild('donaCanvas') donaCanvas?: ElementRef<HTMLCanvasElement>;
  @ViewChild('barrasCanvas') barrasCanvas?: ElementRef<HTMLCanvasElement>;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getDashboard().subscribe(res => {
      this.data = res;
      setTimeout(() => this.renderCharts(), 50);
    });
  }

  get itemsGraficoActual() {
    if (!this.data) return [];
    return this.tipoVistaGrafico === 'GASTOS'
      ? (this.data.gastosPorCategoria || [])
      : (this.data.ingresosPorCategoria || []);
  }

  cambiarVistaGrafico(tipo: 'GASTOS' | 'INGRESOS'): void {
    this.tipoVistaGrafico = tipo;
    setTimeout(() => this.renderDonaChart(), 50);
  }

  private renderCharts(): void {
    this.renderDonaChart();
    this.renderBarrasChart();
  }

  private renderDonaChart(): void {
    if (!this.data || !this.donaCanvas) return;
    if (this.donaChartInstance) {
      this.donaChartInstance.destroy();
      this.donaChartInstance = undefined;
    }

    const items = this.itemsGraficoActual;
    if (items.length === 0) return;

    this.donaChartInstance = new Chart(this.donaCanvas.nativeElement, {
      type: 'doughnut',
      data: {
        labels: items.map(c => c.categoria),
        datasets: [{
          data: items.map(c => Number(c.monto)),
          backgroundColor: items.map(c => c.colorHex),
          borderWidth: 2,
          borderColor: '#131b2e'
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'top',
            labels: {
              color: '#cbd5e1',
              boxWidth: 12,
              font: { size: 11 }
            }
          },
          tooltip: {
            callbacks: {
              label: (ctx) => ` ${ctx.label}: S/ ${Number(ctx.raw).toFixed(2)}`
            }
          }
        }
      }
    });
  }

  private renderBarrasChart(): void {
    if (!this.data || !this.barrasCanvas) return;
    if (this.barrasChartInstance) {
      this.barrasChartInstance.destroy();
      this.barrasChartInstance = undefined;
    }

    this.barrasChartInstance = new Chart(this.barrasCanvas.nativeElement, {
      type: 'bar',
      data: {
        labels: this.data.historicoUltimosMeses.map(h => h.mesNombre),
        datasets: [
          { label: 'Ingresos (S/)', data: this.data.historicoUltimosMeses.map(h => Number(h.ingresos)), backgroundColor: '#10b981', borderRadius: 4 },
          { label: 'Gastos (S/)', data: this.data.historicoUltimosMeses.map(h => Number(h.gastos)), backgroundColor: '#f43f5e', borderRadius: 4 }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            labels: { color: '#cbd5e1' }
          }
        },
        scales: {
          x: { ticks: { color: '#94a3b8' }, grid: { color: 'rgba(255,255,255,0.05)' } },
          y: { ticks: { color: '#94a3b8' }, grid: { color: 'rgba(255,255,255,0.05)' } }
        }
      }
    });
  }
}
