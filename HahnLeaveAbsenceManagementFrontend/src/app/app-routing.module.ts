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

const routes: Routes = [
  { path: 'dashboard', component: DashboardPageComponent },
  { path: 'login', component: LoginPageComponent},
  { path: 'register', component: RegisterPageComponent},
  { path: 'users', component: UserListPageComponent},
  { path: 'leaveRequests', component: LeaveRequestListPageComponent},
  { path: 'myleaverequests', component: MyLeaveRequestPageComponent},
  { path: 'requestnewleave', component: RequestNewLeaveComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
