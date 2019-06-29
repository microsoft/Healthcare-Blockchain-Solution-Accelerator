import { Role } from './role';

export interface ApplicationUser {
    id: number;
    name: string;
    imgsrc: string;
    role: Role;
}
