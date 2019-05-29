import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { User } from '../models/user';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';
import { ProfileService, ContractTransaction } from '../services/profile.service';
import { environment } from 'src/environments/environment';
import { AutheticationService } from '../services/authetication.service';
import { Role } from '../models/role';
import { Subscription } from 'rxjs';
//import { map, filter } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  public isUserLogged: boolean;
  public isCaseWorker: boolean;
  public isStateApprover: boolean;
  public isEnrollmentBroker: boolean;
  private subscription: Subscription;

  approvals: User[];
  contracts: ContractTransaction[];
  page = 1;
  dtOptions: DataTables.Settings = {};
  dtTrigger: Subject<any> = new Subject();

  constructor(private userService: UserService, private router: Router, private profileService: ProfileService, private authService: AutheticationService) { }

  ngOnInit() {
    // TODO - dup'd from menu component; should share across instead
    this.subscription = this.authService.userSelected$.subscribe(currentUser => {
      this.isUserLogged = currentUser !== null;
      this.isCaseWorker = currentUser !== null && currentUser.role === Role.CaseWorker;
      this.isStateApprover = currentUser !== null && currentUser.role === Role.StateApprover;
      this.isEnrollmentBroker = currentUser !== null && currentUser.role === Role.EnrollmentBroker;
    });

    const state = environment.state;

    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 5,
      processing: true
    };

    // TODO - limit by user auth (only see citizens assigned to current user)
    if (this.isCaseWorker) {
      this.profileService.getCitizenProfiles(state).subscribe(con => {
        this.contracts = con;
        this.dtTrigger.next();
      });
    } else if (this.isEnrollmentBroker) {
      // TODO - need Citizen Profiles that are completed, but healthcare plan not assigned
      //this.profileService.getCompletedCitizenProfiles(state)
      //  .pipe(filter(con => con.))
      //  .subscribe(con => {
      //  this.contracts = con;
      //  this.dtTrigger.next();
      //});
      this.profileService.getCompletedCitizenProfiles(state)
        .subscribe(con => {
          this.contracts = con;
          this.dtTrigger.next();
        });
    } else if (this.isStateApprover) {
      this.profileService.getCitizenProfilesNotApprovedHealthcareplan(state).subscribe(con => {
        this.contracts = con;
        this.dtTrigger.next();
      });
    } else {
      // TODO - have better blank page/feedback
    }
  }

  ngOnDestroy(): void {
    this.dtTrigger.unsubscribe();
  }

  addUser(contract: ContractTransaction) {
    this.userService.selectContract(contract);

    this.router.navigate(['/profile/', contract.businessContractDTO.citizenIdentifier]);
  }
}
