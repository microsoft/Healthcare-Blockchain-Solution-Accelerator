import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileFindComponent } from './profile-find.component';

describe('ProfileFindComponent', () => {
  let component: ProfileFindComponent;
  let fixture: ComponentFixture<ProfileFindComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProfileFindComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileFindComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
