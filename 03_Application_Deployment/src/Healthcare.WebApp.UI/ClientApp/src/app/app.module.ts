import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { DataTablesModule } from 'angular-datatables';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MenuComponent } from './menu/menu.component';
import { ProfileComponent } from './profile/profile.component';
import { ProfileIdentityComponent } from './profile-identity/profile-identity.component';
import { ProfileNavigationMenuComponent } from './profile-navigation-menu/profile-navigation-menu.component';
import { ProfileMaritalStatusComponent } from './profile-marital-status/profile-marital-status.component';
import { ProfileProofCitizenshipComponent } from './profile-proof-citizenship/profile-proof-citizenship.component';
import { ProfileSelectionComponent } from './profile-selection/profile-selection.component';
import { ProfileFindComponent } from './profile-find/profile-find.component';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ProfileCreationComponent } from './profile-creation/profile-creation.component';
import * as bootstrap from 'bootstrap';
import * as $ from 'jquery';
import { ProfileStatusPipe, HealthcareStatusPipe, EligilibityStatusPipe } from './helpers/profile-status-pipe';
import { environment } from 'src/environments/environment';
import { API_BASE_URL } from './services/profile.service';
import { DashboardComponent } from './dashboard/dashboard.component';


@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    ProfileComponent,
    ProfileIdentityComponent,
    ProfileNavigationMenuComponent,
    ProfileMaritalStatusComponent,
    ProfileProofCitizenshipComponent,
    ProfileSelectionComponent,
    ProfileFindComponent,
    ProfileCreationComponent,
    ProfileStatusPipe,
    HealthcareStatusPipe,
    EligilibityStatusPipe,
    DashboardComponent
  ],
  imports: [
    BrowserModule,
    DataTablesModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [HttpClient, { provide: API_BASE_URL, useFactory: getBaseUrl }],
  bootstrap: [AppComponent]
})
export class AppModule { }

export function getBaseUrl(): string {
  return environment.baseUrl;
}
