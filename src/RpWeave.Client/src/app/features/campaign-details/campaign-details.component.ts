import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CampaignModel } from './models/campaign.model';
import { CampaignDetailsService } from './campaign-details.service';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-campaign-details',
  imports: [MatListModule],
  templateUrl: './campaign-details.component.html',
  styleUrl: './campaign-details.component.scss',
})
export class CampaignDetailsComponent {
  protected campaign: CampaignModel | undefined;

  constructor(
    private route: ActivatedRoute,
    private campaignDetailsService: CampaignDetailsService)
  { }

  async ngOnInit() {
    this.route.params.subscribe(async (params) => {
      const id = params['id'];

      this.campaign = await this.campaignDetailsService.getCampaignDetailsAsync(id);
    });
  }
}
