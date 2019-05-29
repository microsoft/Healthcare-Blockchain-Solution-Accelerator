import { Component, OnInit } from '@angular/core';
import { ApplicationUser } from './models/application-user';
import { ActivatedRoute, NavigationEnd, Router, RoutesRecognized } from '@angular/router';
import { AutheticationService } from './services/authetication.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  currentUser: ApplicationUser;
  companyTitle: string;
  pageTitle: string;
  isUserLogged: boolean;
  userSubscription: Subscription;

  constructor(private router: Router, private authService: AutheticationService) {
  }

  ngOnInit() {
    this.companyTitle = 'Contoso';

    this.pageTitle = 'PlaceHolder';

    this.currentUser = this.authService.getLoggedUser();

    this.userSubscription = this.authService.userSelected$.subscribe(us => {
      this.currentUser = us;
      this.isUserLogged = this.currentUser !== null;
    });

    this.router.events.subscribe((data) => {
      if (data instanceof RoutesRecognized && data.state.root.firstChild !== null) {
        this.pageTitle = data.state.root.firstChild.data['pageTitle'];
      }
    });
  }

  userLogout() {
    this.authService.logout();
    this.isUserLogged = false;
    this.router.navigate(['/']);
  }
}
