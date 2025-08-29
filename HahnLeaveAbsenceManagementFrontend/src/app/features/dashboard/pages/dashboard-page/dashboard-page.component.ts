import {Component} from '@angular/core';
import {LeaveStatus} from "../../../../core/interfaces/leave-request-interfaces/leave-status";
import {finalize, forkJoin, map} from "rxjs";
import * as console from "node:console";
import {LeaveRequestService} from "../../../../core/services/leave-request-service/leave-request.service";
import {LeaveRequestWithUser} from "../../../../core/interfaces/leave-request-interfaces/leave-request-with-user";


@Component({
  selector: 'app-dashboard-page',
  standalone: false,
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.css'
})
export class DashboardPageComponent {
  isPendingLoading : boolean = false;
  pendingErrorMessage : string | null = null;

  isMonthApprovedLoading : boolean = false;
  monthApprovedErrorMessage : string | null = null;

  isOutTodayLoading = false;
  outTodayErrorMessage: string | null = null;

  outTodayRequests: LeaveRequestWithUser[] = [];
  outTodayPeopleCount = 0;

  monthApprovedLeaveRequests : LeaveRequestWithUser[] = [];
  MonthApprovedCount : number = 0;

  pendingLeaveRequests: LeaveRequestWithUser[] = [];
  pendingCount : number = 0;

  constructor(private readonly leaveRequestService : LeaveRequestService) {
  }

  getPendingLeaveRequests() {
    this.isPendingLoading = true;
    this.pendingErrorMessage = null;

    const base = {
      userId: undefined,
      type: undefined,
      status: LeaveStatus.Pending,
      startDateFrom: undefined,
      startDateTo: undefined,
      endDateFrom: undefined,
      endDateTo: undefined,
      pageQuery: { page: 1, pageSize: 100000 },
    } as const;

    this.leaveRequestService.getAllLeaveRequests(base).subscribe({
      next: (data) => {
        const items = data?.items ?? [];
        this.pendingLeaveRequests = items;
        this.pendingCount = data.totalCount;
      },
      error: (err) => {
        console.error(err);
        this.pendingErrorMessage =
          typeof err?.error === 'string'
            ? err.error
            : err?.message ?? 'Failed to load pending leave requests.';
        this.isPendingLoading = false;
      },
      complete: () => {
        this.isPendingLoading = false;
      },
    })
  }

  nextMonthApprovedLeaveRequests() {
    this.isMonthApprovedLoading = true;
    this.pendingErrorMessage = null;
    const d = new Date();
    const today =   new Date(d.getFullYear(), d.getMonth(), d.getDate());
    const base = {
      userId: undefined,
      type: undefined,
      status: LeaveStatus.Approved,
      startDateFrom: today,
      startDateTo: new Date(today.getDay()+30),
      endDateFrom: undefined,
      endDateTo: undefined,
      pageQuery: { page: 1, pageSize: 100000 },
    } as const;

    this.leaveRequestService.getAllLeaveRequests(base).subscribe({
      next: (data) => {
        const items = data?.items ?? [];
        this.monthApprovedLeaveRequests = items;
        this.pendingCount = data.totalCount;
      },
      error: (err) => {
        console.error(err);
        this.monthApprovedErrorMessage =
          typeof err?.error === 'string'
            ? err.error
            : err?.message ?? 'Failed to load pending leave requests.';
        this.isMonthApprovedLoading = false;
      },
      complete: () => {
        this.isMonthApprovedLoading = false;
      },
    })
  }

  getPeopleOutToday() {
    this.isOutTodayLoading = true;
    this.outTodayErrorMessage = null;

    const now = new Date();
    const start = new Date(now.getFullYear(), now.getMonth(), now.getDate()); // 00:00 today
    const end = new Date(start);
    end.setDate(end.getDate() + 1);

    const base = {
      userId: undefined,
      type: undefined,
      status: LeaveStatus.Approved,
      startDateFrom: undefined,
      startDateTo: end,
      endDateFrom: start,
      endDateTo: undefined,
      pageQuery: { page: 1, pageSize: 100000 },
    } as const;

    this.leaveRequestService.getAllLeaveRequests(base).subscribe({
      next: (data) => {
        const items = data?.items ?? [];
        const inRange = items.filter(r => {
          const s = new Date((r as any).startDate);
          const e = new Date((r as any).endDate);
          return s < end && e >= start;
        });

        this.outTodayRequests = inRange;
        this.outTodayPeopleCount = inRange.length;
      },
      error: (err) => {
        console.error(err);
        this.outTodayErrorMessage =
          typeof err?.error === 'string' ? err.error :
            err?.message ?? 'Failed to load people out today.';
      },
      complete: () => {
        this.isOutTodayLoading = false;
      }
    });
  }

}
