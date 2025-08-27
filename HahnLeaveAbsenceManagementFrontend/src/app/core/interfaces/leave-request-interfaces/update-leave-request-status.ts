import {LeaveStatus} from "./leave-status";

export interface UpdateLeaveRequestStatus {
  id: string;
  status: LeaveStatus;
}
