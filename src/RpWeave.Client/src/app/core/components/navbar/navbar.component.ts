import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { UserService } from '../../services/user.service';
import { NavigationEnd, Router } from '@angular/router';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [MatToolbarModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  showNavbar: boolean = true;
  private hiddenRoutes = ['/landing', '/login'];

  username: string | undefined;
  userSubscription: Subscription | undefined;

  constructor(
    private userService: UserService,
    router: Router) {
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

  ngOnDestroy() {
    this.userSubscription?.unsubscribe();
  }
}
