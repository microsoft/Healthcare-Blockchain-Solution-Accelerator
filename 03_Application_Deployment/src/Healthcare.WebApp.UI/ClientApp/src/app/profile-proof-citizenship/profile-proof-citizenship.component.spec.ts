import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileProofCitizenshipComponent } from './profile-proof-citizenship.component';

describe('ProfileProofCitizenshipComponent', () => {
  let component: ProfileProofCitizenshipComponent;
  let fixture: ComponentFixture<ProfileProofCitizenshipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProfileProofCitizenshipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileProofCitizenshipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
