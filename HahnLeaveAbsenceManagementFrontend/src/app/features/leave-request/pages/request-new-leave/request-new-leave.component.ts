import {Component, effect, OnInit, ViewChild} from '@angular/core';
import {FullCalendarComponent} from "@fullcalendar/angular";
import {LeaveRequestService} from "../../../../core/services/leave-request-service/leave-request.service";
import {LeaveRequestWithUser} from "../../../../core/interfaces/leave-request-interfaces/leave-request-with-user";
import {AuthService} from "../../../../core/services/auth-service/auth.service";
import {ToastrService} from "ngx-toastr";
import {LeaveStatus} from "../../../../core/interfaces/leave-request-interfaces/leave-status";
import {finalize, forkJoin, map} from "rxjs";
import {CalendarOptions, DateSelectArg, EventInput} from "@fullcalendar/core";
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';
import {LeaveType} from "../../../../core/interfaces/leave-request-interfaces/leave-type";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {CreateLeaveRequest} from "../../../../core/interfaces/leave-request-interfaces/create-leave-request";
import {UserService} from "../../../../core/services/user-service/user.service";


@Component({
  selector: 'app-request-new-leave',
  standalone: false,
  templateUrl: './request-new-leave.component.html',
  styleUrl: './request-new-leave.component.css'
})
export class RequestNewLeaveComponent implements OnInit {

  @ViewChild('calendarRef') calendarRef!: FullCalendarComponent;

  myLeavesRequest : LeaveRequestWithUser[] = [];
  userId : string | undefined = undefined;
  leavesLeft : number = 0;

  isLoading = false;
  isSubmitting = false;
  errorMessage: string | null = null;

  events: EventInput[] = [];

  createForm!: FormGroup;
  leaveTypeOptions = Object.keys(LeaveType).filter(k => isNaN(+k)) as Array<keyof typeof LeaveType>;



  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, interactionPlugin],
    initialView: 'dayGridMonth',
    headerToolbar: { left: 'prev,next today', center: 'title', right: '' },
    height: 'auto',
    expandRows: true,
    weekends: true,
    selectable: true,
    selectMirror: true,
    unselectAuto: true,
    select: (info) => this.onCalendarSelect(info),
    eventClick: (info) => this.onEventClick(info.event.extendedProps as any),
    eventDisplay: 'block'
  };

  constructor(private readonly leaveRequestService: LeaveRequestService,
              private readonly authService: AuthService,
              private readonly userService: UserService,
              private readonly toastr: ToastrService,
              private readonly fb: FormBuilder
  ) {
    effect(()=>{
      const currentUser = this.authService.currentUser();
      this.userId = currentUser?.id;
    })
  }

  ngOnInit(): void {
    this.getMyLeaveRequests();
    this.getLeavesLeft();
    this.createForm = this.fb.group(
      {
        type: [null as LeaveType | null, Validators.required],
        startDate: [null, Validators.required],
        endDate: [null, Validators.required],
        description: [''],
      },
      { validators: this.endAfterStartValidator }
    );

  }


  getLeavesLeft(){
    this.userService.getLeavesLeft().subscribe({
      next: data => {
        this.leavesLeft = data;
      },
      error: err => {
        console.log(err);
      }
    })
  }

  getMyLeaveRequests() {
    this.isLoading = true;
    this.errorMessage = null;

    const base = {
      userId: this.userId,
      type: undefined,
      startDateFrom: undefined,
      startDateTo: undefined,
      endDateFrom: undefined,
      endDateTo: undefined,
      pageQuery: { page: 1, pageSize: 100000 },
    } as const;

    const approved$ = this.leaveRequestService.getAllLeaveRequests({
      ...base,
      status: LeaveStatus.Approved,
    });

    const pending$ = this.leaveRequestService.getAllLeaveRequests({
      ...base,
      status: LeaveStatus.Pending,
    });

    forkJoin([approved$, pending$])
      .pipe(
        map(([approvedRes, pendingRes]) => [
          ...(approvedRes.items ?? []),
          ...(pendingRes.items ?? []),
        ]),
        finalize(() => (this.isLoading = false))
      )
      .subscribe({
        next: items => {
          this.myLeavesRequest = items;
          this.events = this.mapLeavesToEvents(items);

          const api = this.calendarRef?.getApi();
          if (api) {
            api.removeAllEventSources();
            api.addEventSource(this.events);
            api.refetchEvents();
          }
        },
        error: err => {
          console.error(err);
          this.errorMessage = 'Failed to load leave requests / Calendar.';
        },
      });
  }



  private mapLeavesToEvents(items: LeaveRequestWithUser[]): EventInput[] {
    return items.map((lr) => {
      const statusColor = this.statusColor(lr.status)
      return {
        id: lr.id,
        title: `${LeaveType[lr.type]}`,
        start: lr.startDate,
        end: this.addOneDayForAllDay(lr.endDate),
        allDay: true,
        backgroundColor: statusColor.bg,
        borderColor: statusColor.bg,
        textColor: statusColor.text,
        extendedProps: {
          ...lr
        }
      } as EventInput;
    });
  }

  private addOneDayForAllDay(date: Date | string): Date {
    const d = new Date(date);
    d.setDate(d.getDate() + 1);
    return d;
  }

  private statusColor(status: LeaveStatus): { bg: string; text: string } {
    switch (status) {
      case LeaveStatus.Approved:
        return { bg: '#10B981', text: '#FFFFFF' };
      case LeaveStatus.Pending:
        return { bg: '#F59E0B', text: '#1F2937' };
      default:
        return { bg: '#F59E0B', text: '#1F2937' };
    }
  }

  private onEventClick(lr: LeaveRequestWithUser) {
    const name = lr.user.firstName + lr.user.lastName;
    this.toastr.info(
      `${name}  ${LeaveType[lr.type]} ${new Date(lr.startDate).toDateString()} → ${new Date(lr.endDate).toDateString()}`,
      'Leave details',
      { timeOut: 4000 }
    );
  }

  onCreateSubmit() {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const raw = this.createForm.value;
    const payload: CreateLeaveRequest = {
      type: Number(raw.type) as LeaveType,
      startDate: new Date(raw.startDate),
      endDate: new Date(raw.endDate),
      description: raw.description?.trim(),
    };

    this.isSubmitting = true;
    this.leaveRequestService
      .createLeaveRequest(payload)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => {
          this.toastr.success('Leave request created.', 'Success', { timeOut: 3000 });
          this.getMyLeaveRequests();
          this.getLeavesLeft();
          this.createForm.reset({ type: null, startDate: null, endDate: null, description: '' });
          const api = this.calendarRef?.getApi();
          api?.unselect?.();
        },
        error: (err) => {
          console.error(err);
          const apiMessage =
            err.error?.message ||
            err.error ||
            'Failed to create leave request.';
          this.toastr.error(apiMessage, 'Error');
        }
      });
  }


  onCalendarSelect(info: DateSelectArg) {
    const start = new Date(info.start);
    const endExclusive = new Date(info.end);
    const endInclusive = new Date(endExclusive);
    endInclusive.setDate(endInclusive.getDate() - 1);

    const startLocal = new Date(start.getFullYear(), start.getMonth(), start.getDate());
    const endLocal = new Date(endInclusive.getFullYear(), endInclusive.getMonth(), endInclusive.getDate());

    this.createForm.patchValue({
      startDate: startLocal,
      endDate: endLocal
    });

    this.toastr.info(
      `${startLocal.toDateString()} → ${endLocal.toDateString()}`,
      'Range selected',
      { timeOut: 2000 }
    );
  }

  get typeCtrl() {
    return this.createForm.get('type');
  }


  private endAfterStartValidator(group: AbstractControl) {
    const start = group.get('startDate')?.value;
    const end = group.get('endDate')?.value;
    if (!start || !end) return null;

    const startD = new Date(start);
    const endD = new Date(end);
    return endD < startD ? { endBeforeStart: true } : null;
  }


  protected readonly LeaveType = LeaveType;
}
