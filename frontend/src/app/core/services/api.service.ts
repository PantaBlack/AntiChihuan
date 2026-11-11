import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Categoria,
  DashboardData,
  IAConsejo,
  Presupuesto,
  RecomendacionIA,
  Transaccion
} from '../models/finance.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl =
    typeof window !== 'undefined' && window.location.port === '4200'
      ? 'http://localhost:8080/api'
      : '/api';

  constructor(private http: HttpClient) {}

  getDashboard(mes?: number, anio?: number): Observable<DashboardData> {
    let params = new HttpParams();
    if (mes) params = params.set('mes', mes);
    if (anio) params = params.set('anio', anio);
    return this.http.get<DashboardData>(`${this.baseUrl}/dashboard`, { params });
  }

  getTransacciones(filtros?: { mes?: number; anio?: number; tipo?: string; soloHormiga?: boolean }): Observable<Transaccion[]> {
    let params = new HttpParams();
    if (filtros?.mes) params = params.set('mes', filtros.mes);
    if (filtros?.anio) params = params.set('anio', filtros.anio);
    if (filtros?.tipo) params = params.set('tipo', filtros.tipo);
    if (filtros?.soloHormiga) params = params.set('soloHormiga', filtros.soloHormiga);
    return this.http.get<Transaccion[]>(`${this.baseUrl}/transacciones`, { params });
  }

  crearTransaccion(payload: any): Observable<Transaccion> {
    return this.http.post<Transaccion>(`${this.baseUrl}/transacciones`, payload);
  }

  eliminarTransaccion(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/transacciones/${id}`);
  }

  exportarCsv(): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/transacciones/exportar-csv`, { responseType: 'blob' });
  }

  getCategorias(tipo?: string): Observable<Categoria[]> {
    let params = new HttpParams();
    if (tipo) params = params.set('tipo', tipo);
    return this.http.get<Categoria[]>(`${this.baseUrl}/categorias`, { params });
  }

  crearCategoria(payload: Partial<Categoria>): Observable<Categoria> {
    return this.http.post<Categoria>(`${this.baseUrl}/categorias`, payload);
  }

  eliminarCategoria(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/categorias/${id}`);
  }

  getPresupuestos(mes?: number, anio?: number): Observable<Presupuesto[]> {
    let params = new HttpParams();
    if (mes) params = params.set('mes', mes);
    if (anio) params = params.set('anio', anio);
    return this.http.get<Presupuesto[]>(`${this.baseUrl}/presupuestos`, { params });
  }

  crearPresupuesto(payload: any): Observable<Presupuesto> {
    return this.http.post<Presupuesto>(`${this.baseUrl}/presupuestos`, payload);
  }

  eliminarPresupuesto(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/presupuestos/${id}`);
  }

  getHistorialIA(): Observable<RecomendacionIA[]> {
    return this.http.get<RecomendacionIA[]>(`${this.baseUrl}/ia/historial`);
  }

  generarDiagnosticoIA(): Observable<{ diagnostico: IAConsejo; historial: RecomendacionIA[] }> {
    return this.http.post<{ diagnostico: IAConsejo; historial: RecomendacionIA[] }>(
      `${this.baseUrl}/ia/generar-diagnostico`,
      {}
    );
  }
}
