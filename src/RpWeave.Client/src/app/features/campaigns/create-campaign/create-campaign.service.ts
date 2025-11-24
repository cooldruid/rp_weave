import { Injectable } from "@angular/core";
import { RpWeaveClient } from "../../../core/clients/rpweave.client";

@Injectable({providedIn: 'root'})
export class CreateCampaignService {
    constructor(private client: RpWeaveClient) 
    { }

    public async createCampaign(data: FormData) {
        await this.client.postAsync('api/campaign/create', data);
    }
}