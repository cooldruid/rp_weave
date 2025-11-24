import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatInputModule } from "@angular/material/input";
import { AddUserDialogResponse } from './models/add-user-dialog-response.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import {MatSelectModule} from '@angular/material/select';

@Component({
  selector: 'app-add-user-dialog',
  imports: [MatDialogModule, MatInputModule, FormsModule, MatButtonModule, MatSelectModule],
  templateUrl: './add-user-dialog.component.html',
  styleUrl: './add-user-dialog.component.scss',
})
export class AddUserDialogComponent {
  readonly dialogRef = inject(MatDialogRef<AddUserDialogComponent>);

  username = '';
  password = '';
  repeatPassword = '';
  role = '';

  constructor(private snackBar: MatSnackBar) 
  { }

  onCancel() {
    this.dialogRef.close();
  }

  onSubmit() {
    if(this.password != this.repeatPassword) {
      this.snackBar.open('Passwords do not match.', 'OK');
      return;
    }

    const response: AddUserDialogResponse = {
      username: this.username,
      password: this.password,
      role: this.role
    }

    this.dialogRef.close(response);
  }
}
