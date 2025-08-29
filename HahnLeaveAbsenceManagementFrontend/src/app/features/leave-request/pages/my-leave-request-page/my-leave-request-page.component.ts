import {Component, effect} from '@angular/core';
import {LeaveRequestWithUser} from "../../../../core/interfaces/leave-request-interfaces/leave-request-with-user";
import {LeaveRequestFilters} from "../../../../core/interfaces/leave-request-interfaces/leave-request-filters";
import {PageQueryRequest} from "../../../../core/interfaces/common-interfaces/page-query-request";
import {LeaveRequestService} from "../../../../core/services/leave-request-service/leave-request.service";
import {UserService} from "../../../../core/services/user-service/user.service";
import {AuthService} from "../../../../core/services/auth-service/auth.service";
import {ToastrService} from "ngx-toastr";
import {PageQueryResult} from "../../../../core/interfaces/common-interfaces/page-query-result";
import {LeaveType} from "../../../../core/interfaces/leave-request-interfaces/leave-type";
import {LeaveStatus} from "../../../../core/interfaces/leave-request-interfaces/leave-status";

@Component({
  selector: 'app-my-leave-request-page',
  standalone: false,
  templateUrl: './my-leave-request-page.component.html',
  styleUrl: './my-leave-request-page.component.css'
})
export class MyLeaveRequestPageComponent {
  leaveRequests: LeaveRequestWithUser[] = [];

  isLoading = false;
  errorMessage: string | null = null;





  pageQuery: PageQueryRequest = { page: 1, pageSize: 10 };
  totalItems = 0;

  LeaveType = LeaveType;
  LeaveStatus = LeaveStatus;

  userId : string | undefined = undefined;



  filters: LeaveRequestFilters = {
    userId: this.userId,
    type: undefined,
    status: undefined,
    startDateFrom: undefined,
    startDateTo: undefined,
    endDateFrom: undefined,
    endDateTo: undefined
  };

  constructor(private readonly leaveRequestService: LeaveRequestService,
              private readonly userService: UserService,
              private readonly authService: AuthService,
              private readonly toastr: ToastrService,) {
    effect(()=>{
      const currentUser = this.authService.currentUser();
      this.userId = currentUser?.id;
    })
  }

  ngOnInit(): void {
    this.loadLeaveRequests();
  }

  loadLeaveRequests(): void {
    this.isLoading = true;
    this.errorMessage = null;

    const opts = this.buildQueryOptions();

    this.leaveRequestService.getAllLeaveRequests(opts).subscribe({
      next: (result: PageQueryResult<LeaveRequestWithUser>) => {
        this.leaveRequests = result.items ?? [];
        this.totalItems = result.totalCount ?? 0;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load leave requests.';
        this.isLoading = false;
      }
    });
  }



  private buildQueryOptions() {
    const f = this.filters;

    return {
      userId: f.userId,
      type: f.type,
      status: f.status,
      startDateFrom: f.startDateFrom,
      startDateTo: f.startDateTo,
      endDateFrom: f.endDateFrom,
      endDateTo: f.endDateTo,
      pageQuery: { ...this.pageQuery }
    };
  }




  cancelLeave(row: LeaveRequestWithUser) {
    this.leaveRequestService.updateLeaveRequestStatus(row.id,{id : row.id,status: LeaveStatus.Cancelled}).subscribe({
      next: (res) => {
        this.toastr.success('Leave request status updated successfully!', 'Success');
        this.loadLeaveRequests();
      },
      error: (e) =>{
        this.toastr.error('Failed to cancel leave. Please inform your HR manager');
      }
    })
  }

  applyFilters(): void {
    this.pageQuery.page = 1;
    this.loadLeaveRequests();
  }

  resetFilters(): void {
    this.filters = {
      userId: undefined,
      type: undefined,
      status: undefined,
      startDateFrom: undefined,
      startDateTo: undefined,
      endDateFrom: undefined,
      endDateTo: undefined
    };
    this.pageQuery.page = 1;
    this.loadLeaveRequests();
  }

  onEndDateToChange(value: string) {
    this.filters.endDateTo = value ? new Date(value + 'T00:00:00') : undefined;
  }

  protected readonly isNaN = isNaN;
  protected readonly Math = Math;
}
