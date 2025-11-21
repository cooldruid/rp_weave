import { Component } from '@angular/core';
import { UserManagementItem } from './models/user-management-item.model';
import { UserManagementService } from './user-management.service';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatDialog } from '@angular/material/dialog';
import { AddUserDialogComponent } from './add-user-dialog/add-user-dialog.component';
import { AddUserDialogResponse } from './add-user-dialog/models/add-user-dialog-response.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-user-management',
  imports: [MatTableModule, MatButtonModule, MatIconModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss',
})
export class UserManagementComponent {
  protected users: UserManagementItem[] = [];
  displayedColumns: string[] = ['username', 'role'];

  constructor(
    private userManagementService: UserManagementService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  )
  { }

  async ngOnInit() {
    await this.getUsers();
  }

  openAddUserDialog() {
    const dialogRef = this.dialog.open(AddUserDialogComponent, {});

    dialogRef.afterClosed().subscribe(async result => {
      await this.addUser(result);
      await this.getUsers();
    })
  }

  async getUsers() {
    const userList = await this.userManagementService.listUsers();

    this.users = userList.users;
  }

  async addUser(user: AddUserDialogResponse) {
    try {
      await this.userManagementService.addUser(user);
    }
    catch(error: any) {
      this.snackBar.open(error.error, 'OK');
    }
  }
}
