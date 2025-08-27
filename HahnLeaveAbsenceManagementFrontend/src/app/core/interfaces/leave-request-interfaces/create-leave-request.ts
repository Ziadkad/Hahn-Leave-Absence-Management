import {LeaveType} from "./leave-type";

export interface CreateLeaveRequest {
  type: LeaveType;
  startDate: Date;
  endDate: Date;
  description: string;
}
