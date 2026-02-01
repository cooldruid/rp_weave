import { Component, inject } from '@angular/core';
import { MatDialogModule, MatDialogRef } from "@angular/material/dialog";

@Component({
  selector: 'app-delete-campaign-dialog',
  imports: [MatDialogModule],
  templateUrl: './delete-campaign-dialog.component.html',
  styleUrl: './delete-campaign-dialog.component.scss',
})
export class DeleteCampaignDialogComponent {
  dialogRef = inject(MatDialogRef<DeleteCampaignDialogComponent>);

  onCancel() {
    this.dialogRef.close();
  }
}
