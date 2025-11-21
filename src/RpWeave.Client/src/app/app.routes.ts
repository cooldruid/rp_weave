import { Routes } from '@angular/router';
import { LandingComponent } from './features/landing/landing.component';
import { LoginComponent } from './features/login/login.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { authGuard } from './core/guards/auth.guard';
import { SignupComponent } from './features/signup/signup.component';
import { CampaignsComponent } from './features/campaigns/campaigns.component';
import { CampaignDetailsComponent } from './features/campaign-details/campaign-details.component';
import { UserService } from './core/services/user.service';
import { inject } from '@angular/core';
import { SettingsComponent } from './features/settings/settings.component';
import { adminGuard } from './core/guards/admin.guard';
import { ChangePasswordComponent } from './features/change-password/change-password.component';

export const routes: Routes = [
    { path: 'landing', component: LandingComponent, title: 'RP Weave' },
    { path: 'login', component: LoginComponent, title: 'Log in to RP Weave' },
    { path: 'signup', component: SignupComponent, title: 'Sign up to RP Weave' },
    { path: 'settings', component: SettingsComponent, title: 'RP Weave', canActivate: [adminGuard] },
    { path: 'campaigns', component: CampaignsComponent, title: 'RP Weave', canActivate: [authGuard] },
    { path: 'dashboard', component: DashboardComponent, title: 'RP Weave', canActivate: [authGuard] },
    { path: 'change-password', component: ChangePasswordComponent, title: 'RP Weave', canActivate: [authGuard] },
    { path: 'campaign-details/:id', component: CampaignDetailsComponent, title: 'RP Weave', canActivate: [authGuard] },
    { path: '', pathMatch: 'full', 
        redirectTo: () => {
            const userService = inject(UserService);
            if(userService.user) {
                return 'campaigns';
            }
            return 'landing';
        }}
];
