import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignInfoComponent } from './campaign-info.component';

describe('CampaignInfoComponent', () => {
  let component: CampaignInfoComponent;
  let fixture: ComponentFixture<CampaignInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CampaignInfoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CampaignInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
