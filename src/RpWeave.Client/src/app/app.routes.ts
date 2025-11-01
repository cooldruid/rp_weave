import { Routes } from '@angular/router';
import { LandingComponent } from './features/landing/landing.component';
import { LoginComponent } from './features/login/login.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    { path: 'landing', component: LandingComponent, title: 'RP Weave' },
    { path: 'login', component: LoginComponent, title: 'Log in to RP Weave' },
    { path: 'dashboard', component: DashboardComponent, title: 'RP Weave', canActivate: [authGuard] },
    { path: '', pathMatch: 'full', redirectTo: 'landing'}
];
