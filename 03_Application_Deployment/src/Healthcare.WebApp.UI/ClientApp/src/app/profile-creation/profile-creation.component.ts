import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
// tslint:disable-next-line:max-line-length
import { ProfileService, HealthcarePlanItem, DocProof, BasicProfileItem, Address, Profile, HealthcarePlan, HealthcarePlanCarePlanEligibilityStatus, HealthcarePlanHealthcareStatus, ContractTransaction } from '../services/profile.service';
import { ProfileCreationViewModel } from '../models/profileCreationViewModel';
import { HealthcareService } from '../services/healthcare.service';
import { ProfileStatus } from '../models/profile-status';
import { Guid } from 'guid-typescript';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-profile-creation',
  templateUrl: './profile-creation.component.html',
  styleUrls: ['./profile-creation.component.css']
})
export class ProfileCreationComponent implements OnInit {
  public isUploaded = false;
  public availablePlans: HealthcarePlanItem[];
  public model: ProfileCreationViewModel;
  imgUrl = location.origin + '/assets/upload_placeholder_270x150.png';
  private uploadedFile: File;
  public docProof: DocProof;

  constructor(private router: Router, private profileService: ProfileService, private healthcareService: HealthcareService) { }

  ngOnInit() {
    this.availablePlans = this.healthcareService.getAvailablePlans();

    this.model = new ProfileCreationViewModel();
  }

  changeListener($event): void {
    this.readFile($event.target);
  }

  async readFile(inputValue: any) {
    if (inputValue.files && inputValue.files[0]) {
      this.uploadedFile = inputValue.files[0];

      const reader = new FileReader();
      reader.readAsDataURL(this.uploadedFile);
      reader.onload = (e) => {
        this.imgUrl = reader.result.toString();
        this.isUploaded = true;
      };


    }
  }

  upload() {
    // this.profileService.updateProofDocument();
    this.isUploaded = true;
  }

  async onSubmit() {
    const address: Address = new Address();
    address.init({
      city: this.model.city,
      street: this.model.street,
      state: this.model.state,
      zip: this.model.zip
    });
    const middleName = (this.model.middleName == null) ? "" : this.model.middleName;
    //const middleName = (this.model.middleName.length > 0) ? this.model.middleName : "";
    const userProfile: BasicProfileItem = new BasicProfileItem();
    userProfile.init({
      address: address,
      name: `${this.model.firstName} ${middleName} ${this.model.lastName}`, // TODO - fix this
      citizenShip: true, // TODO - hardcoded, need to change
      dateOfBirth: new Date(this.model.dateOfBirth),
      fedIncome: Number(this.model.federalIncome),
      stateIncome: Number(this.model.stateIncome),
      applicationType: 0
    });

    const profile: Profile = new Profile();
    profile.init({
      basicProfile: userProfile,
      citizenIdentifier: Guid.raw(),
      activeState: environment.state, //TODO - hardcoded, need to change
      currentHealthcarePlan: new HealthcarePlan(),
      stateApprover: '',
      status: ProfileStatus.Created,
      preferredHealthcarePlan: this.model.preferredHealthcarePlan,
      description: ''
    });

    $('#modalcreationprocessing').modal('show');

    const transaction = await this.profileService.deployProfile(profile).toPromise();

    if (this.isUploaded) {
      // tslint:disable-next-line:max-line-length
      const updateDoc = await this.profileService.updateProofDocument(this.uploadedFile, transaction.businessContractDTO.citizenIdentifier).toPromise();
      this.docProof = updateDoc;
      console.log(this.docProof);

      if (this.runRules(transaction)) {
        const compTransaction = await this.profileService.updateProfile(transaction.businessContractDTO.citizenIdentifier, this.docProof).toPromise();

        $('#modalSubmitSuccessful').modal('show');
      } else {
        alert('This did not conform to the MVP, Please meet Rule Requirements.'); // TODO for MVP only
      }
    } else {

      $('#modalcreationprocessing').modal('hide');

      $('#modalCreateSuccessful').modal('show');
    }
  }

  runRules(transaction: ContractTransaction) {
    // TODO - rule logic here
    const healthcarePlan: HealthcarePlan = new HealthcarePlan();
    healthcarePlan.init({
      healthcareStatus: HealthcarePlanHealthcareStatus.NotAssigned,
      plan: null, // set by Enrollment Broker
      ownerState: environment.state, //TODO - hardcoded, need to change
      enrollmentBorkerAssigedState: environment.state, //TODO - hardcoded, need to change
      carePlanEligibilityStatus: HealthcarePlanCarePlanEligibilityStatus.Mandatory,  // set by ESC Rules
      planApprover: null,
      approved: null, // set by State Approver
      assignedDate: null, // set by Enrollment Broker
      enrolledDate: null, // set by State Approver isApproved
      approvedDate: null, // set by State Approver isApproved
      eligible: true    // TODO - set by rules; Medicare eligible
    });

    transaction.businessContractDTO.currentHealthcarePlan = healthcarePlan;

    return true;
  }

  createModal() {
    $('#modalCreateSuccessful').modal('hide');
    this.router.navigate(['/dashboard']);
  }

  successModal() {
    $('#modalSubmitSuccessful').modal('hide');
    this.router.navigate(['/dashboard']);
  }
}
