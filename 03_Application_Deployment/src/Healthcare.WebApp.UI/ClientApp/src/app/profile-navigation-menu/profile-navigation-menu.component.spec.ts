import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileNavigationMenuComponent } from './profile-navigation-menu.component';

describe('ProfileNavigationMenuComponent', () => {
  let component: ProfileNavigationMenuComponent;
  let fixture: ComponentFixture<ProfileNavigationMenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProfileNavigationMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileNavigationMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
