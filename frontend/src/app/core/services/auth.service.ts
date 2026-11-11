import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthResponse } from '../models/finance.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl =
    typeof window !== 'undefined' && window.location.port === '4200'
      ? 'http://localhost:8080/api/auth'
      : '/api/auth';

  currentUser = signal<AuthResponse | null>(this.getStoredUser());

  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { email, password }).pipe(
      tap(res => this.saveSession(res))
    );
  }

  register(nombreCompleto: string, email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, { nombreCompleto, email, password }).pipe(
      tap(res => this.saveSession(res))
    );
  }

  logout(): void {
    localStorage.removeItem('antichihuan_token');
    localStorage.removeItem('antichihuan_user');
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('antichihuan_token');
  }

  private saveSession(res: AuthResponse): void {
    localStorage.setItem('antichihuan_token', res.token);
    localStorage.setItem('antichihuan_user', JSON.stringify(res));
    this.currentUser.set(res);
  }

  private getStoredUser(): AuthResponse | null {
    const raw = localStorage.getItem('antichihuan_user');
    return raw ? JSON.parse(raw) : null;
  }
}
