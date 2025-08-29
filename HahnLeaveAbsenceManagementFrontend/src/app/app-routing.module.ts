import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {DashboardPageComponent} from "./features/dashboard/pages/dashboard-page/dashboard-page.component";
import {LoginPageComponent} from "./features/auth/pages/login-page/login-page.component";
import {RegisterPageComponent} from "./features/auth/pages/register-page/register-page.component";
import {guestGuard} from "./core/guards/guest-guard/guest.guard";
import {UserListPageComponent} from "./features/user/pages/user-list-page/user-list-page.component";
import {
  LeaveRequestListPageComponent
} from "./features/leave-request/pages/leave-request-list-page/leave-request-list-page.component";
import {
  MyLeaveRequestPageComponent
} from "./features/leave-request/pages/my-leave-request-page/my-leave-request-page.component";
import {RequestNewLeaveComponent} from "./features/leave-request/pages/request-new-leave/request-new-leave.component";
import {HomePageComponent} from "./features/home/pages/home-page/home-page.component";
import {authGuard} from "./core/guards/auth-guard/auth.guard";
import {roleGuard} from "./core/guards/role-guard/role.guard";
import {UserRole} from "./core/interfaces/user-interfaces/user-Role";

const routes: Routes = [
  { path: '', component: HomePageComponent },
  { path: 'dashboard', component: DashboardPageComponent,canActivate:[authGuard,roleGuard],  data: { roles: [UserRole.HumanResourcesManager] } },
  { path: 'login', component: LoginPageComponent , canActivate:[guestGuard]},
  { path: 'register', component: RegisterPageComponent, canActivate:[guestGuard]},
  { path: 'users', component: UserListPageComponent, canActivate:[authGuard,roleGuard],  data: { roles: [UserRole.HumanResourcesManager] }},
  { path: 'leaveRequests', component: LeaveRequestListPageComponent, canActivate:[authGuard]},
  { path: 'myleaverequests', component: MyLeaveRequestPageComponent, canActivate:[authGuard]},
  { path: 'requestnewleave', component: RequestNewLeaveComponent, canActivate:[authGuard]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
