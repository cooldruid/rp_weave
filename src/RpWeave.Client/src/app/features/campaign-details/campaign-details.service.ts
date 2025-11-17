import { Injectable } from "@angular/core";
import { RpWeaveClient } from "../../core/clients/rpweave.client";
import { CampaignModel } from "./models/campaign.model";
import { PostChatMessageModel } from "./models/post-chat-message.model";
import { ChatMessageResponseModel } from "./models/chat-message-response.model";

@Injectable({providedIn: 'root'})
export class CampaignDetailsService {
    constructor(private client: RpWeaveClient) 
    { }

    public async getCampaignDetailsAsync(id: string) : Promise<CampaignModel> {
        return await this.client.getAsync(`api/campaign/${id}`, {});
    }

    public async postChatMessageAsync(chatMessage: PostChatMessageModel) : Promise<ChatMessageResponseModel> {
        return await this.client.postAsync<PostChatMessageModel, ChatMessageResponseModel>('api/ai/prompt', chatMessage);
    }
}