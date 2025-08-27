import {UserRole} from "../user-interfaces/user-Role";

export interface RegisterPayload
{
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}
