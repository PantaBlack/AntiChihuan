import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { IAConsejo, RecomendacionIA } from '../../core/models/finance.models';

@Component({
  selector: 'app-ia',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h4 class="fw-bold text-white mb-1"><i class="bi bi-robot text-warning me-2"></i>Asistente Financiero IA (Google Gemini)</h4>
        <p class="text-muted small mb-0">Diagnóstico automatizado de hábitos de consumo y detección de gastos hormiga</p>
      </div>
      <button (click)="solicitarDiagnostico()" [disabled]="cargando" class="btn btn-warning fw-bold text-dark px-4">
        <i class="bi bi-sparkles me-1"></i>
        {{ cargando ? 'Analizando con Gemini...' : 'Generar Diagnóstico IA' }}
      </button>
    </div>

    @if (diagnostico) {
      <div class="card card-dark p-4 mb-4 border-warning">
        <div class="d-flex justify-content-between align-items-center mb-2">
          <h5 class="fw-bold text-warning mb-0">Resultado del Diagnóstico: Riesgo {{ diagnostico.nivelRiesgoChihuan }}</h5>
          <span class="badge bg-warning text-dark">Ahorro Mensual Sugerido: S/ {{ diagnostico.estimadoAhorroMensualSugerido | number:'1.2-2' }}</span>
        </div>
        <p class="text-light">{{ diagnostico.analisisGeneral }}</p>

        <h6 class="fw-bold text-white mt-3">Recomendaciones Accionables de Ahorro:</h6>
        <ul class="small text-light">
          @for (rec of diagnostico.recomendacionesAhorro; track rec) {
            <li>{{ rec }}</li>
          }
        </ul>
      </div>
    }

    <div class="card card-dark p-4">
      <h6 class="fw-bold text-white mb-3">Historial de Auditorías Generadas por la IA</h6>
      @for (item of historial; track item.id) {
        <div class="p-3 rounded-3 mb-2" style="background-color: #1a233a; border: 1px solid #2a374f;">
          <div class="fw-bold text-warning small">{{ item.titulo }}</div>
          <div class="small text-light mt-1">{{ item.mensaje }}</div>
        </div>
      }
    </div>
  `
})
export class IaComponent implements OnInit {
  diagnostico?: IAConsejo;
  historial: RecomendacionIA[] = [];
  cargando = false;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getHistorialIA().subscribe(res => (this.historial = res));
  }

  solicitarDiagnostico(): void {
    this.cargando = true;
    this.api.generarDiagnosticoIA().subscribe(res => {
      this.diagnostico = res.diagnostico;
      this.historial = res.historial;
      this.cargando = false;
    });
  }
}
