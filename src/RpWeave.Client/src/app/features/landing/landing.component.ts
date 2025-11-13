import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { Router } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [MatButtonModule, MatDividerModule],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent {
  showDivider = true;

  constructor(private router: Router, private breakpointObserver: BreakpointObserver)
  { 
    this.breakpointObserver.observe([Breakpoints.Small, Breakpoints.XSmall])
      .subscribe(result => {
        this.showDivider = !result.matches; // vertical only on medium+ screens
      });
  }

  navigateToLogin() {
    this.router.navigate(['login']);
  }

  navigateToSignup() {
    this.router.navigate(['signup']);
  }
}
