import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteCampaignDialogComponent } from './delete-campaign-dialog.component';

describe('DeleteCampaignDialogComponent', () => {
  let component: DeleteCampaignDialogComponent;
  let fixture: ComponentFixture<DeleteCampaignDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteCampaignDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteCampaignDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
