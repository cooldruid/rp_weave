import { Injectable } from "@angular/core";
import { RpWeaveClient } from "../../core/clients/rpweave.client";
import { CampaignListModel } from "./models/campaignlist.model";

@Injectable({providedIn: 'root'})
export class CampaignsService {
    constructor(private client: RpWeaveClient) 
    { }

    public async listCampaignsAsync() : Promise<CampaignListModel> {
        return await this.client.getAsync<CampaignListModel>('api/campaign', {});
    }

    public async deleteCampaignAsync(id: string) {
        return await this.client.deleteAsync(`api/campaign/${id}`);
    }
}