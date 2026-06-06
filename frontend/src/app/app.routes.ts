import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin-guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/register/register.component').then((m) => m.RegisterComponent),
  },
  {
    path: 'equipment',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/equipment/equipment-list/equipment-list.component').then(
            (m) => m.EquipmentListComponent,
          ),
      },
      {
        path: 'create',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/equipment/equipment-form/equipment-form.component').then(
            (m) => m.EquipmentFormComponent,
          ),
      },
      {
        path: ':id',
        loadComponent: () =>
          import('./features/equipment/equipment-item/equipment-item.component').then(
            (m) => m.EquipmentItemComponent,
          ),
      },
      {
        path: ':id/edit',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/equipment/equipment-form/equipment-form.component').then(
            (m) => m.EquipmentFormComponent,
          ),
      },
    ],
  },
  {
    path: 'reservation',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/reservations/reservation-list/reservation-list.component').then(
            (m) => m.ReservationListComponent,
          ),
      },
      {
        path: 'pending',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/reservations/pending-reservations/pending-reservations.component').then(
            (m) => m.PendingReservationsComponent,
          ),
      },
    ],
  },
  {
    path: 'locations',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/locations/locations.component').then((m) => m.LocationsComponent),
  },
  {
    path: 'equipment-types',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/equipment-type/equipment-type.component').then(
        (m) => m.EquipmentTypeComponent,
      ),
  },
  {
    path: 'statistic',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./features/statistic/statistic.component').then((m) => m.StatisticComponent),
  },
  { path: '**', redirectTo: '/login', pathMatch: 'full' },
];
