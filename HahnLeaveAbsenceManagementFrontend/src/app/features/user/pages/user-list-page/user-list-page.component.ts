import { Component } from '@angular/core';
import {User} from "../../../../core/interfaces/user-interfaces/user";
import {UserService} from "../../../../core/services/user-service/user.service";
import {UserRole} from "../../../../core/interfaces/user-interfaces/user-Role";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-user-list-page',
  standalone: false,
  templateUrl: './user-list-page.component.html',
  styleUrl: './user-list-page.component.css'
})
export class UserListPageComponent {
  users: User[] = [];
  isLoading = false;
  errorMessage: string | null = null;


  // Modal state
  isMutating = false;
  showAddDialog = false;
  selectedUser: User | null = null;
  daysToAdd: number | null = null;
  inlineError: string | null = null;

  constructor(private readonly userService: UserService,
              private readonly toastr: ToastrService,) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading = true;
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        this.users = res;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load users.';
        this.isLoading = false;
      }
    });
  }



  openAddLeaves(u: User) {
    this.selectedUser = u;
    this.daysToAdd = null;
    this.inlineError = null;
    this.showAddDialog = true;
  }

  cancelAddLeaves() {
    this.showAddDialog = false;
    this.selectedUser = null;
    this.daysToAdd = null;
    this.inlineError = null;
  }

  submitAddLeaves() {
    this.inlineError = null;

    const days = Number(this.daysToAdd);
    if (!Number.isFinite(days) || days <= 0) {
      this.inlineError = 'Please enter a positive number of days.';
      return;
    }

    if (!this.selectedUser) return;
    this.isMutating = true;
    const userId = this.selectedUser.id;
    this.userService.addLeaves(userId, { userId, days }).subscribe({
      next: () => {
        this.loadUsers();
        this.toastr.success(
          `Added ${days} day(s) to ${this.selectedUser?.firstName} ${this.selectedUser?.lastName}`,
          'Success'
        );
        this.isMutating = false;
        this.cancelAddLeaves();
      },
      error: (e) => {
        this.inlineError = e.message || 'Failed to add leaves. Please try again.';
        this.isMutating = false;
      }
    });
  }

  protected readonly UserRole = UserRole;
}
