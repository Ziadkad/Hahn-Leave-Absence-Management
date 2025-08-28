import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {DashboardPageComponent} from "./features/dashboard/pages/dashboard-page/dashboard-page.component";
import {LoginPageComponent} from "./features/auth/pages/login-page/login-page.component";
import {RegisterPageComponent} from "./features/auth/pages/register-page/register-page.component";
import {guestGuard} from "./core/guards/guest-guard/guest.guard";

const routes: Routes = [
  { path: 'dashboard', component: DashboardPageComponent },
  { path: 'login', component: LoginPageComponent},
  { path: 'register', component: RegisterPageComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
