import {LeaveStatus} from "./leave-status";
import {LeaveType} from "./leave-type";

export interface LeaveRequestFilters {
  userId?: string;
  type?: LeaveType;
  status?: LeaveStatus;
  startDateFrom?: Date;
  startDateTo?: Date;
  endDateFrom?: Date;
  endDateTo?: Date;
}
