import { Component, input } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { CampaignModel } from '../models/campaign.model';
import { MatCardModule } from "@angular/material/card";
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-campaign-info',
  imports: [MatListModule, MatCardModule, MatButtonModule],
  templateUrl: './campaign-info.component.html',
  styleUrl: './campaign-info.component.scss',
})
export class CampaignInfoComponent {
  campaign = input<CampaignModel>();
}
