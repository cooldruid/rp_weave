import { Injectable } from "@angular/core";
import { RpWeaveClient } from "../../../core/clients/rpweave.client";
import { CreateCampaignResponseModel } from "./models/create-campaign-response.model";

@Injectable({providedIn: 'root'})
export class CreateCampaignService {
    constructor(private client: RpWeaveClient) 
    { }

    public async createCampaign(data: FormData) {
        return await this.client.postAsync<FormData, CreateCampaignResponseModel>('/api/campaign/create', data);
    }
}