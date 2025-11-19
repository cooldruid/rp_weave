import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { LoginService } from './login.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../core/services/user.service';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-login',
    imports: [MatCardModule, MatInputModule, MatFormFieldModule, MatButtonModule, FormsModule],
    templateUrl: './login.component.html',
    styleUrl: './login.component.scss'
})
export class LoginComponent {
  username = '';
  password = '';
  snackBar = inject(MatSnackBar);

  constructor(
    private loginService: LoginService,
    private userService: UserService,
    private router: Router)
  { }

  async login() {
    try {
      await this.loginService.loginAsync(this.username, this.password);

      this.router.navigate(['campaigns'])
    }
    catch(error: any) {
      this.snackBar.open(error.error, 'OK');
    }
  }

  cancelLogin() {
    this.router.navigate(['']);
  }
}
