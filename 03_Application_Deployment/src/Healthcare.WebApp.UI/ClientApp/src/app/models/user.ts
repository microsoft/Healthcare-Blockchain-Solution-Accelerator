import { Status } from './status';
import { UserDriverLicense } from './user-driver-license';

export interface User {
    id: number;
    name: string;
    citizenIdentifier: number;
    dob: Date;
    city: string;
    state: string;
    caseNum: number;
    status: Status;
    userDriverLicense: UserDriverLicense;
}
