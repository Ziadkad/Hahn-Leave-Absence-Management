import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth-service/auth.service';
import {UserRole} from "../../../../core/interfaces/user-interfaces/user-Role";
import {RegisterPayload} from "../../../../core/interfaces/auth-interfaces/register-payload";

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.css']
})
export class RegisterFormComponent implements OnInit {
  registerForm!: FormGroup;
  isSubmitting = false;
  errorMessage: string | null = null;

  roles = [
    { label: 'Human Resources Manager', value: UserRole.HumanResourcesManager },
    { label: 'Employee', value: UserRole.Employee },
  ];

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly toastr: ToastrService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      role: [null as UserRole | null, [Validators.required]],
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    const { firstName, lastName, email, password, role } = this.registerForm.value;

    const payload: RegisterPayload = {
      firstName,
      lastName,
      email,
      password,
      role,
    };

    this.authService.register(payload).subscribe({
      next: (res) => {
        this.isSubmitting = false;
        this.toastr.success(`Welcome ${res.firstName}!`, 'Registration Successful', { timeOut: 2000 });
        this.router.navigate(['']);
      },
      error: (err) => {
        this.isSubmitting = false;
        const msg = err?.message ?? 'Registration failed';
        this.errorMessage = msg;
        this.toastr.error(msg, 'Registration Error', { timeOut: 2500 });
        console.error('Registration error:', err);
      }
    });
  }
}
