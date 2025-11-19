import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CampaignModel } from './models/campaign.model';
import { CampaignDetailsService } from './campaign-details.service';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { ChatMessageModel } from './models/chat-message.model';
import { PostChatMessageModel } from './models/post-chat-message.model';
import { FormsModule } from '@angular/forms';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';

@Component({
  selector: 'app-campaign-details',
  imports: [FormsModule, MatProgressSpinnerModule, MatListModule, MatCardModule, MatInputModule, MatIconModule, MatButtonModule],
  templateUrl: './campaign-details.component.html',
  styleUrl: './campaign-details.component.scss',
})
export class CampaignDetailsComponent {
  protected campaign: CampaignModel | undefined;
  protected chatMessages: ChatMessageModel[] = [];
  protected modelThinking: boolean = false;
  protected message: string = '';

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

  async sendMessage() {
    const nextOrder = Math.max(...this.chatMessages.map(x => x.order));

    const chatMessage: ChatMessageModel = {
      order: nextOrder,
      content: this.message,
      type: 'user'
    }

    this.chatMessages.push(chatMessage);

    const postChatMessage: PostChatMessageModel = {
      collectionName: this.campaign!.vectorCollectionName!,
      prompt: this.message
    }

    this.message = '';
    this.modelThinking = true;
    const response = await this.campaignDetailsService.postChatMessageAsync(postChatMessage);

    const responseChatMessage: ChatMessageModel = {
      order: nextOrder + 1,
      content: response.response,
      type: 'model'
    }

    this.chatMessages.push(responseChatMessage);
    this.chatMessages = this.chatMessages.sort((a, b) => a.order - b.order);
    this.modelThinking = false;
  }
}
