import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { ProfileService, ContractTransaction } from '../services/profile.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-profile-find',
  templateUrl: './profile-find.component.html',
  styleUrls: ['./profile-find.component.css']
})
export class ProfileFindComponent implements OnInit {
  //filteredIncompletedProfiles: ContractTransaction[];
  //numberResults: number;
  //showResultsTable = false;
  currCitizenIdentifier: string;
  currCitizenProfile: ContractTransaction;
  currUserState = environment.state;

  constructor(private userService: UserService, private router: Router, private profileService: ProfileService) { }

  ngOnInit() {
  }

  async onSubmit(f: NgForm) {
    this.currCitizenIdentifier = f.value['searchName'];
    this.currCitizenIdentifier = this.currCitizenIdentifier.trim();

    $('#modalprocessing').modal('show');

    this.currCitizenProfile = await this.profileService.getCitizenProfile(this.currCitizenIdentifier).toPromise();

    $('#modalprocessing').modal('hide');

    if (this.currCitizenProfile) {
      if (this.currCitizenProfile.businessContractDTO.activeState != this.currUserState) {
        $('#modalDiffState').modal('show');
      } else {
        this.userService.selectContract(this.currCitizenProfile);
        this.router.navigate(['/profile/', this.currCitizenIdentifier]);
      }
    } else {
      alert('No profile was found for Citizen Identifier "' + this.currCitizenIdentifier + '". Please try again.');
      //this.router.navigate(['/approvals']);
    }

    //const state = environment.state;
    //const incompletedProfiles = await this.profileService.getIncompletedCitizenProfiles(state).toPromise();

    //const transactions = incompletedProfiles.filter(prof => {
    //  // tslint:disable-next-line:max-line-length
    //  return prof.businessContractDTO.citizenIdentifier !== undefined && prof.businessContractDTO.citizenIdentifier !== null && prof.businessContractDTO.citizenIdentifier.indexOf(citizenIdentifier) >= 0;
    //});

    //if (transactions && transactions.length === 1) {
    //  this.userService.selectContract(transactions[0]);
    //  this.router.navigate(['/profile/', citizenIdentifier]);
    //} else if (transactions && transactions.length > 1) {
    //  this.filteredIncompletedProfiles = transactions;
    //  this.numberResults = this.filteredIncompletedProfiles.length;
    //  this.showResultsTable = true;
    //} else {
    //  alert('We did not find the User. You will be redirect to the Approvals page');
    //  this.router.navigate(['/approvals']);
    //}
  }

  async ChangeState() {
    $('#modalprocessing').modal('show');

    this.currCitizenProfile = await this.profileService.resetActiveState(this.currCitizenIdentifier, this.currUserState).toPromise();
    this.userService.selectContract(this.currCitizenProfile);

    $('#modalprocessing').modal('hide');

    this.router.navigate(['/profile/', this.currCitizenIdentifier]);
  
  }

  //addUser(contract: ContractTransaction) {
  //  this.userService.selectContract(contract);

  //  this.router.navigate(['/profile/', contract.businessContractDTO.citizenIdentifier]);
  //}

}
