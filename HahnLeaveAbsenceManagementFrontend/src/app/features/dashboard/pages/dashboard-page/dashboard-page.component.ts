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


  pendingDonut!: Partial<DonutOptions>;
  approvedNext30Bar!: Partial<BarOptions>;
  outNext30Line!: Partial<LineOptions>;

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



  private refreshPendingDonut() {
    const counts = this.countByType(this.pendingLeaveRequests);
    const labels = Object.keys(counts);
    const series = labels.map((k) => counts[k]);

    this.pendingDonut = {
      series,
      labels,
      chart: { type: 'donut', height: 260 },
      legend: { position: 'bottom' },
      responsive: [{ breakpoint: 768, options: { chart: { height: 240 }, legend: { position: 'bottom' } } }],
      tooltip: { y: { formatter: (v: number) => `${v} req` } },
    };
  }

  private refreshApprovedNext30Bar() {
    const counts = this.countByType(this.monthApprovedLeaveRequests);
    const categories = Object.keys(counts);
    const data = categories.map((k) => counts[k]);

    this.approvedNext30Bar = {
      series: [{ name: 'Approved (next 30 days)', data }],
      chart: { type: 'bar', height: 300 },
      xaxis: { categories, labels: { rotate: 0 } },
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
      const t: string = (r as any).type ?? 'Unknown';
      map[t] = (map[t] ?? 0) + 1;
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

}
