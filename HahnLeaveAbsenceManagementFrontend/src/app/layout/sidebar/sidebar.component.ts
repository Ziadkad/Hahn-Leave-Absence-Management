import {Component, effect} from '@angular/core';
import {AuthService} from "../../core/services/auth-service/auth.service";
import {Router} from "@angular/router";
import {UserRole} from "../../core/interfaces/user-interfaces/user-Role";
import {LoginResponse} from "../../core/interfaces/auth-interfaces/login-response";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  isAuthenticated = this.authService.isAuthenticated();
  userRole: UserRole | undefined;
  currentUser: LoginResponse | null = null;
  constructor(private readonly authService: AuthService,
              private readonly router: Router,
              private readonly toastr: ToastrService,) {
    effect(()=>{
      this.isAuthenticated = this.authService.isAuthenticated();
      this.currentUser = this.authService.currentUser();
      this.userRole = this.currentUser?.role;
    })
  }

  logout() {
    this.authService.logout();
    this.toastr.success('Logged out');
    this.router.navigate(['']);
  }

  protected readonly UserRole = UserRole;
}
