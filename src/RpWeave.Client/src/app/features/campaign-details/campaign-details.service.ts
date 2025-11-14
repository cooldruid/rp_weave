import { Injectable } from "@angular/core";
import { RpWeaveClient } from "../../core/clients/rpweave.client";
import { CampaignModel } from "./models/campaign.model";

@Injectable({providedIn: 'root'})
export class CampaignDetailsService {
    constructor(private client: RpWeaveClient) 
    { }

    public async getCampaignDetailsAsync(id: string) : Promise<CampaignModel> {
        return await this.client.getAsync(`api/campaign/${id}`, {});
    }
}