import {CanActivateFn, Router} from '@angular/router';
import {inject} from "@angular/core";
import {AuthService} from "../../services/auth-service/auth.service";
import {UserRole} from "../../interfaces/user-interfaces/user-Role";

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const user = authService.currentUser();
  const allowedRoles = route.data?.['roles'] as UserRole[];

  if (user && allowedRoles?.includes(user.role)) {
    return true;
  }

  router.navigate([''], {
    queryParams: { returnUrl: state.url }
  });

  return false;
};
