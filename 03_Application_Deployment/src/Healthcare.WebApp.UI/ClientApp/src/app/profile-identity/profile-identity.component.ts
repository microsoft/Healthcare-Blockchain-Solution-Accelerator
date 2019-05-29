import { Component, OnInit, Input, AfterViewInit } from '@angular/core';
import { DatePipe, formatDate } from '@angular/common';
import { UserService } from '../services/user.service';
// tslint:disable-next-line:max-line-length
import { ProfileService, ContractTransaction, DocProof, HealthcarePlanItem, HealthcarePlan, HealthcarePlanCarePlanEligibilityStatus, HealthcarePlanHealthcareStatus } from '../services/profile.service';
import { Observable, ReplaySubject } from 'rxjs';
import { HealthcareService } from '../services/healthcare.service';
import { Router } from '@angular/router';
import { AutheticationService } from '../services/authetication.service';
import { Role } from '../models/role';
import { Subscription } from 'rxjs';
import { ProfileStatus } from '../models/profile-status';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-profile-identity',
  templateUrl: './profile-identity.component.html',
  styleUrls: ['./profile-identity.component.css']
})

export class ProfileIdentityComponent implements OnInit, AfterViewInit {
  @Input() model: ContractTransaction;
  public identityEditable: boolean;
  public image: any;
  public isUploaded = false;
  private uploadedFile: File;
  public docProof: DocProof;
  public verified = false;
  public availablePlans: HealthcarePlanItem[];
  imgUrl = location.origin + '/assets/upload_placeholder_270x150.png';
  imgUrlE = location.origin + '/assets/upload_placeholder_270x150.png';
  bdateFormatted: any;
  eligibilityStatuses = HealthcarePlanCarePlanEligibilityStatus;
  healthcareStatuses = HealthcarePlanHealthcareStatus;
  currEligibilityStatus = '';

  public isUserLogged: boolean;
  public isCaseWorker: boolean;
  public isStateApprover: boolean;
  public isEnrollmentBroker: boolean;
  private subscription: Subscription;
  private imageExists = false;

  constructor(private userService: UserService, private router: Router, private profileService: ProfileService, private healthcareService: HealthcareService, private authService: AutheticationService) {
  }

  async ngOnInit() {
    // TODO - dup'd from menu component; should share across instead
    this.subscription = this.authService.userSelected$.subscribe(currentUser => {
      this.isUserLogged = currentUser !== null;
      this.isCaseWorker = currentUser !== null && currentUser.role === Role.CaseWorker;
      this.isStateApprover = currentUser !== null && currentUser.role === Role.StateApprover;
      this.isEnrollmentBroker = currentUser !== null && currentUser.role === Role.EnrollmentBroker;
    });

    this.availablePlans = this.healthcareService.getAvailablePlans();

    if (!this.model) {
      this.userService.contractSelected$.subscribe(async con => {
        this.model = con;
        if (this.model.businessContractDTO.status === ProfileStatus.Completed) {
          this.docProof = this.model.businessContractDTO.identifyProofs;
          const docProofUrl = await this.profileService.getProofDocumentUrl(this.model.businessContractDTO.citizenIdentifier).toPromise();

          if (docProofUrl.length > 0) {
            this.imgUrl = docProofUrl;
            this.imgUrlE = docProofUrl;
            this.imageExists = true;
          }
        } else {
          // Profile changed states; doesn't have Preferred Healthcare Plan set for new Active State
          if (!this.model.businessContractDTO.preferredHealthcarePlan) {
            this.model.businessContractDTO.preferredHealthcarePlan = new HealthcarePlan();
          }
          this.currEligibilityStatus = "Not Assigned";
          this.model.businessContractDTO.currentHealthcarePlan = new HealthcarePlan();
          this.imgUrl = location.origin + '/assets/upload_placeholder_270x150.png';
          this.imgUrlE = location.origin + '/assets/upload_placeholder_270x150.png';
        }
      });
    }
  }

  ngAfterViewInit() {
    if (this.isCaseWorker) {
      $('.isEnrollmentBroker').removeClass('d-block').addClass('d-none');

      $('.isStateApprover').removeClass('d-block').addClass('d-none');

      $('.isCaseWorker').removeClass('d-none').addClass('d-block');

    } else if (this.isStateApprover) {
      $('.isEnrollmentBroker').removeClass('d-block').addClass('d-none');

      $('.isCaseWorker').removeClass('d-block').addClass('d-none');

      $('.isStateApprover').removeClass('d-none').addClass('d-block');

    } else if (this.isEnrollmentBroker) {
      $('.isCaseWorker').removeClass('d-block').addClass('d-none');

      $('.isStateApprover').removeClass('d-block').addClass('d-none');

      $('.isEnrollmentBroker').removeClass('d-none').addClass('d-block');
    }

    if (this.model.businessContractDTO.status == ProfileStatus.Completed) {
      $('.isComplete').removeClass('d-none').addClass('d-block');
      this.currEligibilityStatus = 'Mandatory'; // TODO - hardcoded 'Mandatory' for MVP
    } else {
      $('.isComplete').removeClass('d-block').addClass('d-none');
      this.currEligibilityStatus = 'Not Assigned';
    }
  }

  /*identityEditEnable() {
    this.identityEditable = true;
  }*/

  editForm() {
    $('#profileEdit').removeClass('d-none').addClass('d-block');
    $('#profileReadOnly').removeClass('d-block').addClass('d-none');

    const planName = this.model.businessContractDTO.preferredHealthcarePlan.name;
    const select = document.getElementById('preferredHealthcarePlanE') as HTMLSelectElement;
    select.childNodes.forEach(_node => {
      const node = _node as HTMLOptionElement;
      if (node.text === planName) {
        node.selected = true;
      }
    });
  }

  cancelUpdate() {
    // TODO - revert fields to match read-only fields
    $('#profileEdit').removeClass('d-block').addClass('d-none');
    $('#profileReadOnly').removeClass('d-none').addClass('d-block');
    //return false;
  }

  async onSubmit() {
    // TODO - should we be calling deployProfile again??
    //const transaction = await this.profileService.deployProfile(this.model.businessContractDTO).toPromise();

    if (this.isUploaded) {
      $('#modalprocessing').modal('show');

      // tslint:disable-next-line:max-line-length
      const updateDoc = await this.profileService.updateProofDocument(this.uploadedFile, this.model.businessContractDTO.citizenIdentifier).toPromise();
      this.docProof = updateDoc;
      console.log(this.docProof);
      const compTransaction = await this.profileService.updateProfile(this.model.businessContractDTO.citizenIdentifier, this.docProof).toPromise();
      
      $('#modalprocessing').modal('hide');

      $('#modalSubmitSuccessful').modal('show');
    }
  }

  async verifyDocument() {
    //$('#modalprocessing').modal('show');

    this.verified = await this.profileService.validateProofDocumentHash(this.model.businessContractDTO.citizenIdentifier, this.docProof.hash).toPromise();
    
    //$('#modalprocessing').modal('hide');

    if (this.verified) {
      $('#modalVerify').modal('show');
    }
  }

  changeDate($event) {
    this.model.businessContractDTO.basicProfile.dateOfBirth = $event;
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
        this.imgUrlE = reader.result.toString();
        this.isUploaded = true;

        if (this.imageExists) {
          alert('Alert! A new Proof Document will be uploaded.');
        }
      };
    }
  }

  //noPlanSelected() {
  //  return !this.items.some(item => item.chosen);
  //}


  updateModal() {
    $('#modalUpdateSuccessful').modal('hide');
    this.router.navigate(['/dashboard']);
  }

  successModal() {
    $('#modalSubmitSuccessful').modal('hide');
    this.router.navigate(['/dashboard']);
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

  async onAssign() {
    // TODO - need to set anything? status, dates?

    $('#modalprocessing').modal('show');
    // tslint:disable-next-line:max-line-length
    const transaction = await this.profileService.assignHealthCarePlan(this.model.businessContractDTO.citizenIdentifier, this.model.businessContractDTO.currentHealthcarePlan).toPromise();

    $('#modalprocessing').modal('hide');
    $('#modalSubmitSuccessful').modal('show');
    //this.router.navigate(['/dashboard']);
  }

  async approvePlan() {
    // TODO - need to set anything? status, dates?

    $('#modalprocessing').modal('show');
    const transaction = await this.profileService.approveHealthcarePlan(this.model.businessContractDTO.citizenIdentifier).toPromise();
    $('#modalprocessing').modal('hide');
    $('#modalSubmitSuccessful').modal('show');
   // this.router.navigate(['/dashboard']);
  }
}
