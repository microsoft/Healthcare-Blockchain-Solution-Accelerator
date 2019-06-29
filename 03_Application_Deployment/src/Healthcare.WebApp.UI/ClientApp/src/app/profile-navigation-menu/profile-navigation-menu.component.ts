import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-profile-navigation-menu',
  templateUrl: './profile-navigation-menu.component.html',
  styleUrls: ['./profile-navigation-menu.component.css']
})
export class ProfileNavigationMenuComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  goToElement(id: string): void {
    const element = document.querySelector(id);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

}
