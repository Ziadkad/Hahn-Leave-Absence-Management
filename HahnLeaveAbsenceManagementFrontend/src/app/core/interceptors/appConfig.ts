import { provideHttpClient, withInterceptors } from '@angular/common/http';
import {authInterceptor} from "./auth-interceptor/auth.interceptor";

export const appConfig = [
  provideHttpClient(
    withInterceptors([
      authInterceptor,
    ])
  )
];
