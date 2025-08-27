import {LeaveType} from "./leave-type";
import {LeaveStatus} from "./leave-status";

export interface LeaveRequest {
  id: string;
  type: LeaveType;
  startDate: Date;
  endDate: Date;
  businessDays: number;
  status: LeaveStatus;
  description: string;
}
