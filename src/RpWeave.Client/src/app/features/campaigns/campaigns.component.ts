import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { CampaignsService } from './campaigns.service';
import { CampaignModel } from './models/campaign.model';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from "@angular/material/icon";

@Component({
    selector: 'app-campaigns',
    imports: [MatCardModule, MatButtonModule, MatIconModule],
    templateUrl: './campaigns.component.html',
    styleUrl: './campaigns.component.scss'
})
export class CampaignsComponent {
  protected campaigns: CampaignModel[] = [];

  constructor(private campaignsService: CampaignsService)
  { }

  async ngOnInit() {
    const campaignList = await this.campaignsService.listCampaignsAsync();
    this.campaigns = campaignList.campaigns;
  }
}
