import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { IAConsejo, RecomendacionIA } from '../../core/models/finance.models';

@Component({
  selector: 'app-ia',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
      <div>
        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
          <h4 class="fw-bold text-white mb-0 fs-5 fs-md-4"><i class="bi bi-robot text-warning me-2"></i>Asistente Financiero AntiChihuan</h4>
          <span class="badge bg-primary text-white px-2 py-1" style="font-size: 0.75rem;">
            <i class="bi bi-stars me-1 text-warning"></i>Google Gemini 3.8 Flash
          </span>
        </div>
        <p class="text-muted small mb-0">Agente de diagnóstico predictivo y detección de fugas de capital con IA</p>
      </div>
      <button (click)="solicitarDiagnostico()" [disabled]="cargando" class="btn btn-warning fw-bold text-dark px-3 px-sm-4 shadow">
        <i class="bi bi-sparkles me-1"></i>
        {{ cargando ? 'Consultando Google Gemini 3.8...' : 'Generar Diagnóstico Gemini 3.8' }}
      </button>
    </div>

    @if (diagnostico) {
      <div class="card card-dark p-3 p-md-4 mb-4 border-warning shadow-lg">
        <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3 pb-3 border-bottom" style="border-color: #2a374f !important;">
          <div class="d-flex align-items-center gap-2">
            <span class="badge rounded-pill px-3 py-1 text-white" style="background: linear-gradient(135deg, #4f46e5, #06b6d4);">
              <i class="bi bi-cpu-fill me-1"></i>Modelo: {{ diagnostico.motorInferencia || 'Google Gemini 3.8 Flash' }}
            </span>
            <span class="badge rounded-pill px-3 py-1 bg-success-subtle text-success border border-success" style="font-size: 0.75rem;">
              <i class="bi bi-check-circle-fill me-1"></i>Inferencia LLM Activa
            </span>
          </div>
          <span class="badge bg-warning text-dark fs-6 px-3 py-2 fw-bold rounded-pill">
            Ahorro Sugerido: S/ {{ diagnostico.estimadoAhorroMensualSugerido | number:'1.2-2' }}
          </span>
        </div>

        <div class="d-flex align-items-center gap-2 mb-2">
          <span class="badge px-3 py-1 rounded-pill fw-bold"
                [ngClass]="{
                  'bg-danger text-white': diagnostico.nivelRiesgoChihuan === 'Crítico' || diagnostico.nivelRiesgoChihuan === 'Critica',
                  'bg-warning text-dark': diagnostico.nivelRiesgoChihuan === 'Alto' || diagnostico.nivelRiesgoChihuan === 'Alta',
                  'bg-info text-dark': diagnostico.nivelRiesgoChihuan === 'Moderado' || diagnostico.nivelRiesgoChihuan === 'Medio',
                  'bg-success text-white': diagnostico.nivelRiesgoChihuan === 'Bajo'
                }">
            Riesgo {{ diagnostico.nivelRiesgoChihuan }}
          </span>
          <h5 class="fw-bold text-white mb-0">Evaluación Preventiva del Mes</h5>
        </div>
        <p class="text-light mt-2 mb-3" style="font-size: 1.05rem; line-height: 1.6;">{{ diagnostico.analisisGeneral }}</p>

        @if (diagnostico.gastosInnecesariosDetectados && diagnostico.gastosInnecesariosDetectados.length > 0) {
          <div class="alert alert-danger py-2 px-3 small rounded-3 mb-3 d-flex align-items-center gap-2" style="background-color: rgba(244, 63, 94, 0.15); border-color: rgba(244, 63, 94, 0.3); color: #fda4af;">
            <i class="bi bi-exclamation-triangle-fill flex-shrink-0"></i>
            <div><strong>Fugas de Capital:</strong> {{ diagnostico.gastosInnecesariosDetectados.join(', ') }}</div>
          </div>
        }

        <h6 class="fw-bold text-white mt-3 mb-2"><i class="bi bi-lightbulb-fill text-warning me-1"></i>Recomendaciones Accionables de Ahorro:</h6>
        <ul class="small text-light mb-0 list-unstyled ps-1">
          @for (rec of diagnostico.recomendacionesAhorro; track rec) {
            <li class="mb-2 d-flex align-items-start gap-2">
              <i class="bi bi-arrow-right-circle-fill text-warning mt-1"></i>
              <span>{{ rec }}</span>
            </li>
          }
        </ul>
      </div>
    }

    <div class="card card-dark p-4">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h6 class="fw-bold text-white mb-0"><i class="bi bi-clock-history text-info me-2"></i>Historial de Auditorías Generadas por Gemini AI</h6>
        <span class="badge bg-secondary text-white">{{ historial.length }} registros</span>
      </div>
      @for (item of historial; track item.id) {
        <div class="p-3 rounded-3 mb-2" style="background-color: #1a233a; border: 1px solid #2a374f;">
          <div class="d-flex justify-content-between align-items-center">
            <span class="fw-bold text-warning small">{{ item.titulo }}</span>
            <span class="badge bg-dark text-muted border border-secondary" style="font-size: 0.7rem;">{{ item.fechaGeneracion | date:'medium' }}</span>
          </div>
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
    this.api.generarDiagnosticoIA().subscribe({
      next: res => {
        this.diagnostico = res.diagnostico;
        this.historial = res.historial;
        this.cargando = false;
      },
      error: () => {
        this.cargando = false;
      }
    });
  }
}
