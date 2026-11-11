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
          <h2 class="fw-bold mb-1 text-white">Hola, {{ data.usuarioNombre }} 👋</h2>
          <p class="text-muted small mb-0">Resumen financiero para <strong class="text-warning">{{ data.nombreMesActual }} {{ data.anioActual }}</strong></p>
        </div>
        <a routerLink="/ia" class="btn btn-warning btn-sm fw-bold text-dark px-3">
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
      <div class="row g-3 mb-4">
        <div class="col-sm-6 col-xl-3">
          <div class="card card-dark p-3 h-100">
            <span class="small text-muted">Ingresos del Mes</span>
            <h3 class="fw-bold text-success mt-1">S/ {{ data.totalIngresosMes | number:'1.2-2' }}</h3>
          </div>
        </div>
        <div class="col-sm-6 col-xl-3">
          <div class="card card-dark p-3 h-100">
            <span class="small text-muted">Gastos del Mes</span>
            <h3 class="fw-bold text-danger mt-1">S/ {{ data.totalGastosMes | number:'1.2-2' }}</h3>
          </div>
        </div>
        <div class="col-sm-6 col-xl-3">
          <div class="card card-dark p-3 h-100">
            <span class="small text-muted">Balance Disponible</span>
            <h3 class="fw-bold mt-1" [ngClass]="data.balanceMes >= 0 ? 'text-white' : 'text-danger'">
              S/ {{ data.balanceMes | number:'1.2-2' }}
            </h3>
          </div>
        </div>
        <div class="col-sm-6 col-xl-3">
          <div class="card card-dark p-3 h-100">
            <span class="small text-muted">Fuga Gastos Hormiga 🐜</span>
            <h3 class="fw-bold text-warning mt-1">S/ {{ data.totalGastosHormigaMes | number:'1.2-2' }}</h3>
            <span class="small text-muted">{{ data.porcentajeGastosHormiga }}% del gasto total</span>
          </div>
        </div>
      </div>

      <!-- Gráficos Chart.js -->
      <div class="row g-4 mb-4">
        <div class="col-lg-5">
          <div class="card card-dark p-4 h-100">
            <h6 class="fw-bold text-white mb-3">Distribución de Gastos por Categoría</h6>
            <div style="height: 250px;">
              <canvas #donaCanvas></canvas>
            </div>
          </div>
        </div>
        <div class="col-lg-7">
          <div class="card card-dark p-4 h-100">
            <h6 class="fw-bold text-white mb-3">Ingresos vs Gastos (Últimos 6 Meses)</h6>
            <div style="height: 250px;">
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
  @ViewChild('donaCanvas') donaCanvas?: ElementRef<HTMLCanvasElement>;
  @ViewChild('barrasCanvas') barrasCanvas?: ElementRef<HTMLCanvasElement>;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getDashboard().subscribe(res => {
      this.data = res;
      setTimeout(() => this.renderCharts(), 50);
    });
  }

  private renderCharts(): void {
    if (!this.data) return;
    if (this.donaCanvas) {
      new Chart(this.donaCanvas.nativeElement, {
        type: 'doughnut',
        data: {
          labels: this.data.gastosPorCategoria.map(c => c.categoria),
          datasets: [{
            data: this.data.gastosPorCategoria.map(c => c.monto),
            backgroundColor: this.data.gastosPorCategoria.map(c => c.colorHex)
          }]
        },
        options: { responsive: true, maintainAspectRatio: false }
      });
    }
    if (this.barrasCanvas) {
      new Chart(this.barrasCanvas.nativeElement, {
        type: 'bar',
        data: {
          labels: this.data.historicoUltimosMeses.map(h => h.mesNombre),
          datasets: [
            { label: 'Ingresos (S/)', data: this.data.historicoUltimosMeses.map(h => h.ingresos), backgroundColor: '#10b981' },
            { label: 'Gastos (S/)', data: this.data.historicoUltimosMeses.map(h => h.gastos), backgroundColor: '#f43f5e' }
          ]
        },
        options: { responsive: true, maintainAspectRatio: false }
      });
    }
  }
}
