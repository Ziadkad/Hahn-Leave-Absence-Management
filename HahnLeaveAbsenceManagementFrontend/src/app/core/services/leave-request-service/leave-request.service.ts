import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {environment} from "../../../../environments/environment";
import {CreateLeaveRequest} from "../../interfaces/leave-request-interfaces/create-leave-request";
import {Observable} from "rxjs";
import {LeaveRequest} from "../../interfaces/leave-request-interfaces/leave-request";
import {UpdateLeaveRequestStatus} from "../../interfaces/leave-request-interfaces/update-leave-request-status";
import {LeaveType} from "../../interfaces/leave-request-interfaces/leave-type";
import {LeaveStatus} from "../../interfaces/leave-request-interfaces/leave-status";
import {PageQueryRequest} from "../../interfaces/common-interfaces/page-query-request";
import {PageQueryResult} from "../../interfaces/common-interfaces/page-query-result";
import {LeaveRequestWithUser} from "../../interfaces/leave-request-interfaces/leave-request-with-user";

@Injectable({
  providedIn: 'root'
})
export class LeaveRequestService {
  private readonly baseUrl = `${environment.apiUrl}/LeaveRequest`;

  constructor(private http: HttpClient) {}

  createLeaveRequest(payload: CreateLeaveRequest): Observable<LeaveRequest> {
    return this.http.post<LeaveRequest>(`${this.baseUrl}`, payload);
  }

  updateLeaveRequestStatus(id: string, command: UpdateLeaveRequestStatus): Observable<LeaveRequest> {
    return this.http.put<LeaveRequest>(`${this.baseUrl}/${id}`, command);
  }

  getAllLeaveRequests(options: {
    userId?: string;
    type?: LeaveType;
    startDateFrom?: Date;
    startDateTo?: Date;
    endDateFrom?: Date;
    endDateTo?: Date;
    status?: LeaveStatus;
    pageQuery?: PageQueryRequest;
  }): Observable<PageQueryResult<LeaveRequestWithUser>> {
    let params = new HttpParams();

    if (options.userId) params = params.set('userId', options.userId);
    if (options.type != null) params = params.set('type', options.type.toString());
    if (options.status != null) params = params.set('status', options.status.toString());

    if (options.startDateFrom) params = params.set('startDateFrom', options.startDateFrom.toISOString());
    if (options.startDateTo) params = params.set('startDateTo', options.startDateTo.toISOString());
    if (options.endDateFrom) params = params.set('endDateFrom', options.endDateFrom.toISOString());
    if (options.endDateTo) params = params.set('endDateTo', options.endDateTo.toISOString());

    if (options.pageQuery?.page != null) params = params.set('pageQuery.pageNumber', options.pageQuery.page);
    if (options.pageQuery?.pageSize != null) params = params.set('pageQuery.pageSize', options.pageQuery.pageSize);


    return this.http.get<PageQueryResult<LeaveRequestWithUser>>(`${this.baseUrl}`, { params });
  }

}
