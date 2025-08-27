import { HttpInterceptorFn } from '@angular/common/http';
import {inject} from "@angular/core";
import {LocalStorageService} from "../../services/local-storage-service/local-storage.service";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const localStorage = inject(LocalStorageService);
  const token = localStorage.getItem<string>('auth_token');

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq);
};
