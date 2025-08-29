import {Component, OnInit} from '@angular/core';
import {LeaveStatus} from "../../../../core/interfaces/leave-request-interfaces/leave-status";
import {LeaveRequestService} from "../../../../core/services/leave-request-service/leave-request.service";
import {LeaveRequestWithUser} from "../../../../core/interfaces/leave-request-interfaces/leave-request-with-user";
import {
  ApexAxisChartSeries,
  ApexChart, ApexDataLabels, ApexGrid,
  ApexLegend,
  ApexNonAxisChartSeries,
  ApexResponsive, ApexStroke,
  ApexTooltip, ApexXAxis, ApexYAxis
} from "ng-apexcharts";
import {LeaveType} from "../../../../core/interfaces/leave-request-interfaces/leave-type";



type DonutOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  labels: string[];
  legend: ApexLegend;
  responsive?: ApexResponsive[];
  tooltip?: ApexTooltip;
};

type BarOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  legend: ApexLegend;
  tooltip?: ApexTooltip;
  grid?: ApexGrid;
  yaxis?: ApexYAxis;
};

type LineOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  stroke: ApexStroke;
  legend: ApexLegend;
  tooltip?: ApexTooltip;
  yaxis?: ApexYAxis;
  grid?: ApexGrid;
};


@Component({
  selector: 'app-dashboard-page',
  standalone: false,
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.css'
})

export class DashboardPageComponent implements OnInit {
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

  pendingDonut: DonutOptions = {
    series: [],
    chart: { type: 'donut', height: 260 },
    labels: [],
    legend: { position: 'bottom' },
    responsive: [],
    tooltip: {}
  };

  approvedNext30Bar: BarOptions = {
    series: [], // OK: empty array conforms to ApexAxisChartSeries
    chart: { type: 'bar', height: 300 },
    xaxis: { categories: [] },
    dataLabels: { enabled: true },
    legend: { position: 'top' },
    grid: { strokeDashArray: 3 },
    yaxis: { min: 0, forceNiceScale: true },
    tooltip: {}
  };

  outNext30Line: LineOptions = {
    series: [], // OK
    chart: { type: 'line', height: 280, zoom: { enabled: false } },
    xaxis: { categories: [] },
    dataLabels: { enabled: false },
    stroke: { curve: 'smooth', width: 3 },
    legend: { position: 'top' },
    grid: { strokeDashArray: 3 },
    yaxis: { min: 0, forceNiceScale: true },
    tooltip: {}
  };

  constructor(private readonly leaveRequestService : LeaveRequestService) {
  }

  ngOnInit(): void {
    this.getPendingLeaveRequests();
    this.getPeopleOutToday();
    this.nextMonthApprovedLeaveRequests();
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
        this.pendingCount = data.totalCount ?? items.length;
        this.refreshPendingDonut(); // add this
      },
      error: (err) => {
        console.error(err);
        this.pendingErrorMessage =
          typeof err?.error === 'string' ? err.error
            : err?.message ?? 'Failed to load pending leave requests.';
        this.isPendingLoading = false;
      },
      complete: () => { this.isPendingLoading = false; },
    });
  }

  nextMonthApprovedLeaveRequests() {
    this.isMonthApprovedLoading = true;
    this.monthApprovedErrorMessage = null; // was: pendingErrorMessage

    const today = this.startOfToday();
    const in30 = new Date(today);
    in30.setDate(in30.getDate() + 30); // was: new Date(today.getDay()+30)

    const base = {
      userId: undefined,
      type: undefined,
      status: LeaveStatus.Approved,
      startDateFrom: today,
      startDateTo: in30,
      endDateFrom: undefined,
      endDateTo: undefined,
      pageQuery: { page: 1, pageSize: 100000 },
    } as const;

    this.leaveRequestService.getAllLeaveRequests(base).subscribe({
      next: (data) => {
        const items = data?.items ?? [];
        this.monthApprovedLeaveRequests = items;
        this.MonthApprovedCount = data?.totalCount ?? items.length; // was: pendingCount
        this.refreshApprovedNext30Bar();
        this.refreshOutNext30Line();
      },
      error: (err) => {
        console.error(err);
        this.monthApprovedErrorMessage =
          typeof err?.error === 'string' ? err.error :
            err?.message ?? 'Failed to load approved leave requests.';
        this.isMonthApprovedLoading = false;
      },
      complete: () => {
        this.isMonthApprovedLoading = false;
      },
    });
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
        this.refreshOutNext30Line();
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



  private refreshPendingDonut() {
    const counts = this.countByType(this.pendingLeaveRequests);
    const order  = this.enumNamesInOrder(LeaveType); // ["Vacation","Sick","Unpaid"]

    this.pendingDonut = {
      series: order.map(n => counts[n] ?? 0),
      labels: order,
      chart: { type: 'donut', height: 260 },
      legend: { position: 'bottom' },
      responsive: [{ breakpoint: 768, options: { chart: { height: 240 }, legend: { position: 'bottom' } } }],
      tooltip: { y: { formatter: (v: number) => `${v} req` } },
    };
  }

  private refreshApprovedNext30Bar() {
    const counts = this.countByType(this.monthApprovedLeaveRequests);
    const order  = this.enumNamesInOrder(LeaveType); // ["Vacation","Sick","Unpaid"]

    this.approvedNext30Bar = {
      series: [{ name: 'Approved (next 30 days)', data: order.map(n => counts[n] ?? 0) }],
      chart: { type: 'bar', height: 300 },
      xaxis: { categories: order, labels: { rotate: 0 } },
      dataLabels: { enabled: true },
      legend: { position: 'top' },
      grid: { strokeDashArray: 3 },
      tooltip: { y: { formatter: (v: number) => `${v} req` } },
      yaxis: { min: 0, forceNiceScale: true },
    };
  }


  private refreshOutNext30Line() {
    const start = this.startOfToday();
    const days = Array.from({ length: 30 }, (_, i) => {
      const s = new Date(start);
      s.setDate(s.getDate() + i);
      const e = new Date(s);
      e.setDate(e.getDate() + 1);
      return { s, e, label: this.shortDate(s) };
    });

    const seriesData = days.map(({ s, e }) =>
      this.monthApprovedLeaveRequests.filter((r) => {
        const rs = new Date((r as any).startDate);
        const re = new Date((r as any).endDate);
        return rs < e && re >= s; // overlaps day
      }).length
    );

    this.outNext30Line = {
      series: [{ name: 'People out', data: seriesData }],
      chart: { type: 'line', height: 280, zoom: { enabled: false } },
      xaxis: { categories: days.map((d) => d.label) },
      dataLabels: { enabled: false },
      stroke: { curve: 'smooth', width: 3 },
      legend: { position: 'top' },
      grid: { strokeDashArray: 3 },
      yaxis: { min: 0, forceNiceScale: true },
      tooltip: { y: { formatter: (v: number) => `${v} people` } },
    };
  }

  private countByType(items: LeaveRequestWithUser[]): Record<string, number> {
    const map: Record<string, number> = {};
    for (const r of items) {
      const label = this.enumName(LeaveType, (r as any).type); // << use enum name
      map[label] = (map[label] ?? 0) + 1;
    }
    return map;
  }


  private startOfToday(): Date {
    const d = new Date();
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
  }

  private shortDate(d: Date): string {
    // e.g., 29 Aug
    return d.toLocaleDateString(undefined, { day: '2-digit', month: 'short' });
  }


  // ---- helpers
  private enumName(enumObj: any, value: number | string | null | undefined): string {
    if (value === null || value === undefined) return 'Unknown';
    const name = enumObj?.[value as any];
    return typeof name === 'string' ? name : String(value);
  }

  private enumNamesInOrder(enumObj: any): string[] {
    // For numeric TS enums this yields ["Vacation","Sick","Unpaid"] in numeric order
    return Object.keys(enumObj)
      .filter(k => isNaN(Number(k)))
      .sort((a, b) => enumObj[a] - enumObj[b]);
  }

  private countByStatus(items: LeaveRequestWithUser[]): Record<string, number> {
    const map: Record<string, number> = {};
    for (const r of items) {
      const label = this.enumName(LeaveStatus, (r as any).status);
      map[label] = (map[label] ?? 0) + 1;
    }
    return map;
  }


  readonly LeaveType = LeaveType;
  readonly LeaveStatus = LeaveStatus;

  readonly statusClass: Record<LeaveStatus, string> = {
    [LeaveStatus.Pending]:  'bg-amber-50 text-amber-700 ring-1 ring-amber-200',
    [LeaveStatus.Approved]: 'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200',
    [LeaveStatus.Rejected]: 'bg-rose-50 text-rose-700 ring-1 ring-rose-200',
    [LeaveStatus.Cancelled]: 'bg-slate-50 text-slate-700 ring-1 ring-slate-200',
  };
}
