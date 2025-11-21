import { Component } from '@angular/core';
import { CreateCampaignRequest } from './models/create-campaign-request.model';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-create-campaign',
  imports: [FormsModule, MatInputModule, MatButtonModule, MatCheckboxModule],
  templateUrl: './create-campaign.component.html',
  styleUrl: './create-campaign.component.scss',
})
export class CreateCampaignComponent {
  request: CreateCampaignRequest = {
    name: '',
    description: '',
    createEmbeddings: false,
    chapterFontSize: undefined,
    subChapterFontSize: undefined,
    headerFontSize: undefined,
    ignoreFooter: undefined
  }

  showOptionalFields = false;

  onCreateEmbeddingsChanged(isChecked: boolean) {
    this.request.createEmbeddings = isChecked;
    this.showOptionalFields = isChecked;

    if(!isChecked) {
      this.request.chapterFontSize = undefined;
      this.request.subChapterFontSize = undefined;
      this.request.headerFontSize = undefined;
      this.request.ignoreFooter = undefined;
    }
  }
}
