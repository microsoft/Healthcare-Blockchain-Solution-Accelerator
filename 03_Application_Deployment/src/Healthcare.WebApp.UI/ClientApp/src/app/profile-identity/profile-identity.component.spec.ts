import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileIdentityComponent } from './profile-identity.component';

describe('ProfileIdentityComponent', () => {
  let component: ProfileIdentityComponent;
  let fixture: ComponentFixture<ProfileIdentityComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProfileIdentityComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileIdentityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
