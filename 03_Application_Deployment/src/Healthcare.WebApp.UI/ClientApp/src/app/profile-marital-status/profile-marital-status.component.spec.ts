import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileMaritalStatusComponent } from './profile-marital-status.component';

describe('ProfileMaritalStatusComponent', () => {
  let component: ProfileMaritalStatusComponent;
  let fixture: ComponentFixture<ProfileMaritalStatusComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProfileMaritalStatusComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileMaritalStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
