import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from "@angular/material/card";
import { MatInputModule } from "@angular/material/input";
import { ChangePasswordService } from './change-password.service';
import { Router } from '@angular/router';
import { ChangePasswordModel } from './models/change-password.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-change-password',
  imports: [MatCardModule, MatInputModule, FormsModule, MatButtonModule],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss',
})
export class ChangePasswordComponent {
  oldPassword = '';
  newPassword = '';
  repeatNewPassword = '';

  constructor(
    private changePasswordService: ChangePasswordService,
    private router: Router,
    private snackBar: MatSnackBar)
  { }

  cancel() {
    this.router.navigate(['']);
  }

  async confirm() {
    try {
      if(this.newPassword != this.repeatNewPassword) {
        this.snackBar.open('Passwords do not match.', 'OK');
        return;
      }

      const request: ChangePasswordModel = {
        oldPassword: this.oldPassword,
        newPassword: this.newPassword
      };

      await this.changePasswordService.changePassword(request);
      this.router.navigate(['']);
    }
    catch(error: any) {
      this.snackBar.open(error.error, 'OK');
    }
  }
}
