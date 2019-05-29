import { Injectable } from '@angular/core';
import { ApplicationUser } from '../models/application-user';
import { AngularWaitBarrier } from 'blocking-proxy/built/lib/angular_wait_barrier';
import { BehaviorSubject, Observable } from 'rxjs';
import { routerNgProbeToken } from '@angular/router/src/router_module';

@Injectable({
  providedIn: 'root'
})
export class AutheticationService {
  private userSelectedSource: BehaviorSubject<ApplicationUser>;
  public userSelected$: Observable<ApplicationUser>;

  constructor() {
    this.userSelectedSource = new BehaviorSubject<ApplicationUser>(JSON.parse(localStorage.getItem('currentUser')));
    this.userSelected$ = this.userSelectedSource.asObservable();
  }

  login(user: ApplicationUser) {
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.userSelectedSource.next(user);
    return user;
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.userSelectedSource.next(null);
  }

  getLoggedUser(): ApplicationUser {
    return this.userSelectedSource.value;
  }
}
