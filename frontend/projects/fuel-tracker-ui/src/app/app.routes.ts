import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./components/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () => import('./components/register.component').then((m) => m.RegisterComponent),
  },
  {
    path: '',
    loadComponent: () => import('./components/layout.component').then((m) => m.LayoutComponent),
    canActivate: [authGuard],
    children: [
      {
        path: 'cars',
        loadComponent: () =>
          import('./components/car-list.component').then((m) => m.CarListComponent),
      },
      {
        path: 'cars/:id',
        loadComponent: () =>
          import('./components/car-detail.component').then((m) => m.CarDetailComponent),
      },
      {
        path: 'statistics',
        loadComponent: () =>
          import('./components/statistics.component').then((m) => m.StatisticsComponent),
      },
      {
        path: '',
        redirectTo: 'cars',
        pathMatch: 'full',
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'cars',
  },
];
