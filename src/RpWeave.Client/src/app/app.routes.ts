import { Routes } from '@angular/router';
import { LandingComponent } from './features/landing/landing.component';
import { LoginComponent } from './features/login/login.component';

export const routes: Routes = [
    { path: 'landing', component: LandingComponent, title: 'RP Weave' },
    { path: 'login', component: LoginComponent, title: 'Log in to RP Weave' },
    { path: '', pathMatch: 'full', redirectTo: 'landing'}
];
