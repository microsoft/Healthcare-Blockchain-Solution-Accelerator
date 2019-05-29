import { Component, OnInit } from '@angular/core';
import { AutheticationService } from '../services/authetication.service';
import { ApplicationUser } from '../models/application-user';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';
import { Role } from '../models/role';

@Component({
  selector: 'app-profile-selection',
  templateUrl: './profile-selection.component.html',
  styleUrls: ['./profile-selection.component.css']
})
export class ProfileSelectionComponent implements OnInit {
  appUsers: ApplicationUser[];

  constructor(private userService: UserService, private authService: AutheticationService, private router: Router) {
  }

  ngOnInit() {
    this.appUsers = this.userService.getAllApplicationUsers();
  }

  userLogin(user: ApplicationUser): void {
    this.authService.login(user);

    if (user.role === Role.CaseWorker) {
      this.router.navigate(['/find-profile']);
    } else {
      this.router.navigate(['/dashboard']);
    }
  }

}
