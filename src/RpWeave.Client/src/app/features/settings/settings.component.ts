import { Component } from '@angular/core';
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatListModule } from "@angular/material/list";
import { UserManagementComponent } from "./user-management/user-management.component";

@Component({
  selector: 'app-settings',
  imports: [MatSidenavModule, MatListModule, UserManagementComponent],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss',
})
export class SettingsComponent {
  allTabs = ['User Management'];
  selectedTab: string = '';

  ngOnInit() {
    this.selectedTab = this.allTabs[0];
  }

  switchTab(newTab: string) {
    this.selectedTab = newTab;
  }
}
