import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { DashboardComponent } from './dashboard/dashboard.component'
import { ProfileComponent } from './profile/profile.component';
import { ProfileSelectionComponent } from './profile-selection/profile-selection.component';
import { ProfileFindComponent } from './profile-find/profile-find.component';
import { AuthGuard } from './services/auth.guard';
import { ProfileCreationComponent } from './profile-creation/profile-creation.component';

const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent, data: { pageTitle: 'Dashboard' }, canActivate: [AuthGuard] },
  { path: 'profile/:id', component: ProfileComponent, data: { pageTitle: 'Profile' }, canActivate: [AuthGuard] },
  { path: 'login', component: ProfileSelectionComponent, data: { pageTitle: 'Login' } },
  { path: 'find-profile', component: ProfileFindComponent, data: { pageTitle: 'Find Citizen' }, canActivate: [AuthGuard] },
  { path: 'create-profile', component: ProfileCreationComponent, data: { pageTitle: 'Create Profile' }, canActivate: [AuthGuard] },
  { path: '', redirectTo: 'login', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
