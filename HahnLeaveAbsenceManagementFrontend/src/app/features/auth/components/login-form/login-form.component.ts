import { Component } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../../../../core/services/auth-service/auth.service";
import {Router} from "@angular/router";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.css'
})
export class LoginFormComponent {
  loginForm!: FormGroup;
  errorMessage: string | null = null;
  isSubmitting = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly toastr: ToastrService,
    private readonly router: Router
  ) {
  }

  ngOnInit() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }
  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    this.authService.login(this.loginForm.value).subscribe({
      next: (res) => {
        this.isSubmitting = false;
        if (!res) {
          this.errorMessage = 'Invalid email or password.';
          this.toastr.error(this.errorMessage, 'Login Failed', { timeOut: 2000 });
        } else {
          this.toastr.success(`Welcome ${res.firstName}!`, 'Login Successful',{ timeOut: 2000 });
          console.log('Login successful', res);
          this.router.navigate(['']);
        }
      },
      error: (err) => {
        this.isSubmitting = false;
        console.error('Login error:', err);
        this.errorMessage = 'Invalid email or password.';
        this.toastr.error('An error occurred. Please try again.', 'Login Error',{ timeOut: 2000 });
      }
    });
  }
}
