import { Component } from '@angular/core';
import { CreateCampaignRequest } from './models/create-campaign-request.model';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { CreateCampaignService } from './create-campaign.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-campaign',
  imports: [FormsModule, MatInputModule, MatButtonModule, MatCheckboxModule],
  templateUrl: './create-campaign.component.html',
  styleUrl: './create-campaign.component.scss',
})
export class CreateCampaignComponent {
  file: File | undefined;

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

  constructor(
    private createCampaignService: CreateCampaignService,
    private snackBar: MatSnackBar,
    private router: Router)
  { }

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

  onFileSelected(event: any) {
    this.file = event.target.files[0];

    console.log(this.file);
  }

  async submit() {
    try {
      const formData = new FormData();

      if(this.file)
        formData.append('Pdf', this.file);

      formData.append('Data', JSON.stringify(this.request));

      await this.createCampaignService.createCampaign(formData);
    }
    catch(error: any) {
      this.snackBar.open(error.error, 'OK');
    }
  }

  cancel() {
    this.router.navigate(['campaigns']);
  }
}
