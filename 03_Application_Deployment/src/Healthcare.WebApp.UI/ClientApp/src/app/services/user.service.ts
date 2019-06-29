import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { Subject, Observable, BehaviorSubject } from 'rxjs';
import { Status } from '../models/status';
import { USE_VALUE } from '@angular/core/src/di/injector';
import { ApplicationUser } from '../models/application-user';
import { Role } from '../models/role';
import { ContractTransaction } from './profile.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private contractSelectedSource: BehaviorSubject<ContractTransaction>;
  public contractSelected$: Observable<ContractTransaction>;

  private users: User[];

  constructor() {
    // Observable string sources
    this.contractSelectedSource = new BehaviorSubject<ContractTransaction>(null);
    this.contractSelected$ = this.contractSelectedSource.asObservable();

    this.users = [
      // tslint:disable:max-line-length
      {
        id: 1, name: 'Jessica Christianson', citizenIdentifier: 123456789012, city: 'New York', state: 'NY', dob: new Date('03/15/1982'), caseNum: 12345, status: Status.Status0, userDriverLicense: {
          firstName: 'Jessica',
          lastName: 'Christianson',
          middleName: 'P',
          sex: 'F',
          licenseNumber: 'P0172523751',
          dob: new Date('03/15/1982'),
          country: 'USA',
          postCode: 64415
        }
      },
      { id: 2, name: 'Jane Doe', citizenIdentifier: 123456789012, city: 'New York', state: 'NY', dob: new Date('01/01/1990'), caseNum: 12345, status: Status.Status0, userDriverLicense: null },
      { id: 3, name: 'Jorge Doe', citizenIdentifier: 123456789012, city: 'New York', state: 'NY', dob: new Date('01/01/1990'), caseNum: 12345, status: Status.Status0, userDriverLicense: null },
      { id: 4, name: 'Jason Doe', citizenIdentifier: 123456789012, city: 'New York', state: 'NY', dob: new Date('01/01/1990'), caseNum: 12345, status: Status.Status0, userDriverLicense: null },
      { id: 5, name: 'Jasmine Doe', citizenIdentifier: 123456789012, city: 'New York', state: 'NY', dob: new Date('01/01/1990'), caseNum: 12345, status: Status.Status0, userDriverLicense: null }
    ];
  }

  public getAllUsers(): User[] {
    return this.users;
  }

  public getAllApplicationUsers(): ApplicationUser[] {
    return [{
      id: 1,
      name: 'Brandon Williams',
      imgsrc: 'assets/user-icon.png',
      role: Role.StateApprover
    },
    {
      id: 2,
      name: 'Jessica Smith',
      imgsrc: 'assets/user-icon.png',
      role: Role.CaseWorker
    },
    {
      id: 3,
      name: 'John Doe',
      imgsrc: 'assets/user-icon.png',
      role: Role.EnrollmentBroker
    }];
  }

  public selectContract(contract: ContractTransaction) {
    this.contractSelectedSource.next(contract);
  }

  public getUserByCitizenNumber(userId: number): User {
    const filteredUser = this.users.find(us => us.citizenIdentifier === userId);
    return filteredUser;
  }
}
