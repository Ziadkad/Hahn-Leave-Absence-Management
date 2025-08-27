import {UserRole} from "../user-interfaces/user-Role";

export interface LoginResponse {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  leavesLeft: number;
  token: string ;
  tokenExpiresAt: Date;
}
