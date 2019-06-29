import { Component, OnInit } from '@angular/core';
import { AutheticationService } from '../services/authetication.service';
import { Role } from '../models/role';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {
  public isUserLogged: boolean;
  public isCaseWorker: boolean;
  public isStateApprover: boolean;
  public isEnrollmentBroker: boolean;
  private subscription: Subscription;

  constructor(private authService: AutheticationService) { }

  ngOnInit() {
    this.subscription = this.authService.userSelected$.subscribe(currentUser => {
      this.isUserLogged = currentUser !== null;
      this.isCaseWorker = currentUser !== null && currentUser.role === Role.CaseWorker;
      this.isStateApprover = currentUser !== null && currentUser.role === Role.StateApprover;
      this.isEnrollmentBroker = currentUser !== null && currentUser.role === Role.EnrollmentBroker;
    });
  }

}
