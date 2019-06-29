import { HealthcarePlanItem } from "../services/profile.service";

export class ProfileCreationViewModel {
  firstName: string;
  middleName: string;
    lastName: string;
    dateOfBirth: string;
    citizenship: string;
    street: string;
    city: string;
    state: string;
    zip: string;
    federalIncome: string;
    stateIncome: string;
    preferredHealthcarePlan: HealthcarePlanItem;
}
