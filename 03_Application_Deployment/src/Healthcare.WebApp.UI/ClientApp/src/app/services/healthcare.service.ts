import { Injectable } from '@angular/core';
import { HealthcarePlanItem } from './profile.service';

@Injectable({
    providedIn: 'root'
})
export class HealthcareService {

    private availablePlans: HealthcarePlanItem[];

    constructor() {
        const plan1 = new HealthcarePlanItem;
        plan1.init({ id: '1', name: 'New York Quality Healthcare Corporation' }); //12007

        const plan2 = new HealthcarePlanItem;
        plan2.init({ id: '2', name: 'Health Insurance Plan of Greater New York, Inc.' }); //10451

        const plan3 = new HealthcarePlanItem;
        plan3.init({ id: '3', name: 'New York Quality Healthcare Corporation' }); //13166

        this.availablePlans = [
            plan1,
            plan2,
            plan3
        ];

    }

    public getAvailablePlans(): HealthcarePlanItem[] {
        return this.availablePlans;
    }
}
