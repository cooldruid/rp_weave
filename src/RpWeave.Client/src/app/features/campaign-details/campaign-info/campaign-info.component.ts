import { Component, inject, input } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { CampaignModel } from '../models/campaign.model';
import { MatCardModule } from "@angular/material/card";
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { DeleteCampaignDialogComponent } from './delete-campaign-dialog/delete-campaign-dialog.component';
import { CampaignDetailsService } from '../campaign-details.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-campaign-info',
  imports: [MatListModule, MatCardModule, MatButtonModule],
  templateUrl: './campaign-info.component.html',
  styleUrl: './campaign-info.component.scss',
})
export class CampaignInfoComponent {
  campaign = input<CampaignModel>();
  dialog = inject(MatDialog);
  service = inject(CampaignDetailsService);
  router = inject(Router);
  snackbar = inject(MatSnackBar);

  onDeleteCampaign() {
    const dialogRef = this.dialog.open(DeleteCampaignDialogComponent);

    dialogRef.afterClosed().subscribe(async res => {
      if(res === true) {
        try {
          await this.service.deleteCampaignAsync(this.campaign()!.id);
          this.router.navigate(['campaigns']);
        }
        catch(error: any) {
          this.snackbar.open(error.error, 'OK');
        }
      }
    })
  }
}
