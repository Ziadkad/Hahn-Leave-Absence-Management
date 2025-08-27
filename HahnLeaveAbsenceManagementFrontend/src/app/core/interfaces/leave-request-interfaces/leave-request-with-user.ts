import {LeaveType} from "./leave-type";
import {LeaveStatus} from "./leave-status";
import {User} from "../user-interfaces/user";

export interface LeaveRequestWithUser {
  id: string;
  type: LeaveType;
  startDate: Date;
  endDate: Date;
  businessDays: number;
  status: LeaveStatus;
  description: string;
  user: User;
}
