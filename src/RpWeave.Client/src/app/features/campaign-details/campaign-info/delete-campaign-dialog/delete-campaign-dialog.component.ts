import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from "@angular/material/dialog";

@Component({
  selector: 'app-delete-campaign-dialog',
  imports: [MatDialogModule, MatButtonModule],
  templateUrl: './delete-campaign-dialog.component.html',
  styleUrl: './delete-campaign-dialog.component.scss',
})
export class DeleteCampaignDialogComponent {
  dialogRef = inject(MatDialogRef<DeleteCampaignDialogComponent>);

  onCancel() {
    this.dialogRef.close();
  }
}
