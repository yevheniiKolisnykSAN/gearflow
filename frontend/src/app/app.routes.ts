import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin-guard';

export const routes: Routes = [
  {path: 'login', canActivate: [guestGuard], loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)},
  {path: 'register', canActivate: [guestGuard], loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)},
  {
    path: 'equipment',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./features/equipment/equipment-list/equipment-list.component').then(m => m.EquipmentListComponent)
      },
      {
        path: 'create',
        canActivate: [adminGuard],
        loadComponent: () => import('./features/equipment/equipment-form/equipment-form.component').then(m => m.EquipmentFormComponent)
      },
      {
        path: ':id/edit',
        canActivate: [adminGuard],
        loadComponent: () => import('./features/equipment/equipment-form/equipment-form.component').then(m => m.EquipmentFormComponent)
      }
    ]
  },
  {
    path: 'locations',
    canActivate: [authGuard],
    loadComponent: () => import('./features/locations/locations.component').then(m => m.LocationsComponent)
  },
  {
    path: 'equipment-types',
    canActivate: [authGuard],
    loadComponent: () => import('./features/equipment-type/equipment-type.component').then(m => m.EquipmentTypeComponent)
  },
  {path: '**', redirectTo: '/login', pathMatch: 'full' }
];
