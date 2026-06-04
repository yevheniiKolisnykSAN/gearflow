import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {path: 'login', canActivate: [guestGuard], loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)},
  {path: 'register', canActivate: [guestGuard], loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)},
  {path: 'equipment-list', canActivate: [authGuard], loadComponent: () => import('./features/equipment/equipment-list/equipment-list.component').then(m => m.EquipmentListComponent)},
  {path: '**', redirectTo: '/login', pathMatch: 'full' }
];
