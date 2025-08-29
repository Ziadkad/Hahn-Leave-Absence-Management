import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SidebarComponent } from './layout/sidebar/sidebar.component';
import { LoginPageComponent } from './features/auth/pages/login-page/login-page.component';
import { RegisterPageComponent } from './features/auth/pages/register-page/register-page.component';
import { RegisterFormComponent } from './features/auth/components/register-form/register-form.component';
import { LoginFormComponent } from './features/auth/components/login-form/login-form.component';
import { DashboardPageComponent } from './features/dashboard/pages/dashboard-page/dashboard-page.component';
import {ToastrModule} from "ngx-toastr";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import { UserListPageComponent } from './features/user/pages/user-list-page/user-list-page.component';
import { LeaveRequestListPageComponent } from './features/leave-request/pages/leave-request-list-page/leave-request-list-page.component';
import {appConfig} from "./core/interceptors/appConfig";
import { MyLeaveRequestPageComponent } from './features/leave-request/pages/my-leave-request-page/my-leave-request-page.component';
import {FullCalendarModule} from "@fullcalendar/angular";
import { RequestNewLeaveComponent } from './features/leave-request/pages/request-new-leave/request-new-leave.component';

@NgModule({
  declarations: [
    AppComponent,
    SidebarComponent,
    LoginPageComponent,
    RegisterPageComponent,
    RegisterFormComponent,
    LoginFormComponent,
    DashboardPageComponent,
    UserListPageComponent,
    LeaveRequestListPageComponent,
    MyLeaveRequestPageComponent,
    RequestNewLeaveComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      timeOut: 3000,
      positionClass: 'toast-top-right',
      preventDuplicates: true
    }),
    FullCalendarModule,
  ],
  providers: [appConfig],
  bootstrap: [AppComponent]
})
export class AppModule { }
