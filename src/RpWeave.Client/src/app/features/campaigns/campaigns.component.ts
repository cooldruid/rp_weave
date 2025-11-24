import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { CampaignsService } from './campaigns.service';
import { CampaignModel } from './models/campaign.model';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from "@angular/material/icon";
import { ClickStopPropagation } from "../../core/directives/click-stop-propagation.directive";
import { Router } from '@angular/router';

@Component({
    selector: 'app-campaigns',
    imports: [MatCardModule, MatButtonModule, MatIconModule, ClickStopPropagation],
    templateUrl: './campaigns.component.html',
    styleUrl: './campaigns.component.scss'
})
export class CampaignsComponent {
  protected campaigns: CampaignModel[] = [];

  constructor(
    private campaignsService: CampaignsService,
    private router: Router)
  { }

  async ngOnInit() {
    const campaignList = await this.campaignsService.listCampaignsAsync();
    this.campaigns = campaignList.campaigns;
  }

  onCampaignClick(id: string) {
    this.router.navigate(['campaign-details/' + id]);
  }

  navigateToAddCampaign() {
    this.router.navigate(['campaigns/add']);
  }

  onCampaignDelete() {
    console.log('Not implemented yet');
  }
}
