import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserService } from './core/services/user.service';

@Component({
    selector: 'app-root',
    imports: [MatListModule, MatToolbarModule, MatButtonModule, MatIconModule, MatSidenavModule, RouterModule],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss'
})
export class AppComponent {
  showNavbar: boolean = true;
  private hiddenRoutes = ['/landing', '/login'];

  username: string | undefined;
  userSubscription: Subscription | undefined;

  constructor(
    private userService: UserService,
    private router: Router) {
    router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.showNavbar = this.userService.user != undefined && !this.hiddenRoutes.includes(event.urlAfterRedirects);
      }
    });
  }

  ngOnInit() {
    this.userSubscription = this.userService.user$.subscribe(user => {
      this.username = user?.name
    });
  }

  navigateToCampaigns() {
    this.router.navigate(['campaigns']);
  }

  ngOnDestroy() {
    this.userSubscription?.unsubscribe();
  }
}
