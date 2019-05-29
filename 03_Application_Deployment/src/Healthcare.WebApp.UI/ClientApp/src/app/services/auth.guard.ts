import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AutheticationService } from './authetication.service';
import { Role } from '../models/role';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private authenticationService: AutheticationService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const currentUser = this.authenticationService.getLoggedUser();
        if (currentUser) {
            // if (route.routeConfig.path === 'login') {
            //     switch (currentUser.role) {
            //         case Role.CaseWorker:
            //             this.router.navigate(['/approvals']);
            //             return true;
            //         case Role.StateApprover:
            //             this.router.navigate(['/find-profile']);
            //             return true;
            //         default:
            //             this.router.navigate(['/login']);
            //             return false;
            //     }
            // }
            // authorised so return true
            return true;
        }

        // not logged in so redirect to login page with the return url
        this.router.navigate(['/login']);
        return false;
    }
}