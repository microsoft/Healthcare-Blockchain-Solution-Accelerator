import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { User } from '../models/user';
import { Status } from '../models/status';
import { UserService } from '../services/user.service';
import { Subscription } from 'rxjs';
import { ContractTransaction } from '../services/profile.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})

export class ProfileComponent implements OnInit, OnDestroy {
  public contract: ContractTransaction;
  subscription: Subscription;

  constructor(private userService: UserService) {
  }

  ngOnInit() {
    this.subscription = this.userService.contractSelected$.subscribe(con => {
      this.contract = con;
    });
  }

  ngOnDestroy() {
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
  }
}
