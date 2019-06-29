import { Pipe, PipeTransform } from '@angular/core';
import { ProfileStatus } from '../models/profile-status';
import { HealthcarePlanHealthcareStatus, HealthcarePlanCarePlanEligibilityStatus } from '../services/profile.service';

@Pipe({ name: 'profileStatus' })
export class ProfileStatusPipe implements PipeTransform {
  transform(pStatus: ProfileStatus): string {
    return ProfileStatus[pStatus];
  }
}

@Pipe({ name: 'healthcareStatus' })
export class HealthcareStatusPipe implements PipeTransform {
  transform(hStatus: HealthcarePlanHealthcareStatus): string {
    return HealthcarePlanHealthcareStatus[hStatus];
  }
}

@Pipe({ name: 'eligibilityStatus' })
export class EligilibityStatusPipe implements PipeTransform {
  transform(hStatus: HealthcarePlanCarePlanEligibilityStatus): string {
    return HealthcarePlanCarePlanEligibilityStatus[hStatus];
  }
}


