import {UserRole} from "./user-Role";

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  leavesLeft: number;
}
