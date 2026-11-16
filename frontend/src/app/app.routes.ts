import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { HomeComponent } from './features/home/home.component';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { TransaccionesComponent } from './features/transacciones/transacciones.component';
import { PresupuestosComponent } from './features/presupuestos/presupuestos.component';
import { CategoriasComponent } from './features/categorias/categorias.component';
import { IaComponent } from './features/ia/ia.component';

export const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'transacciones', component: TransaccionesComponent, canActivate: [authGuard] },
  { path: 'presupuestos', component: PresupuestosComponent, canActivate: [authGuard] },
  { path: 'categorias', component: CategoriasComponent, canActivate: [authGuard] },
  { path: 'ia', component: IaComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '' }
];
