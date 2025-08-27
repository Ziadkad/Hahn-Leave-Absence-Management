import {computed, Injectable, signal} from '@angular/core';
import {environment} from "../../../../environments/environment.development";
import {LoginResponse} from "../../interfaces/auth-interfaces/login-response";
import {HttpClient} from "@angular/common/http";
import {LocalStorageService} from "../local-storage-service/local-storage.service";
import {LoginPayload} from "../../interfaces/auth-interfaces/login-payload";
import {catchError, Observable, tap, throwError} from "rxjs";
import {RegisterPayload} from "../../interfaces/auth-interfaces/register-payload";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly baseUrl = `${environment.apiUrl}/Auth`;
  private readonly tokenKey = 'auth_token';
  private readonly userKey = 'current_user';
  private logoutTimer: any = null;

  private readonly _currentUser = signal<LoginResponse | null>(null);
  readonly currentUser = this._currentUser;
  readonly isAuthenticated = computed(() => !!this._currentUser());

  constructor(
    private readonly http: HttpClient,
    private readonly localStorageService: LocalStorageService
  ) {
    this.loadAuthState();
  }


  login(payload: LoginPayload): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, payload).pipe(
      tap(res => {
        this.localStorageService.setItem(this.tokenKey, res.token);
        this.localStorageService.setItem(this.userKey, res);
        this._currentUser.set(res);
        this.loadAuthState();
        const exp = this.getTokenExpiration(res.token);
        if (exp) {
          this.scheduleAutoLogout(exp);
        }
      }),
      catchError(() => {
        this.logout();
        return throwError(() => new Error('Login failed'));
      })
    );
  }

  register(payload: RegisterPayload): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.baseUrl}/register`, payload)
      .pipe(
        tap((res) => {
          this.localStorageService.setItem(this.tokenKey, res.token);
          this.localStorageService.setItem(this.userKey, res);
          this._currentUser.set(res);
          this.loadAuthState();
          const exp = this.getTokenExpiration(res.token); // -> Date | null
          if (exp) {
            this.scheduleAutoLogout(exp);
          }
        }),
        catchError((err) => {
          this.logout();
          const msg =
            err?.error?.message ??
            err?.message ??
            'Registration failed';
          return throwError(() => new Error(msg));
        })
      );
  }


  private loadAuthState(): void {
    const token = this.localStorageService.getItem<string>(this.tokenKey);
    const user = this.localStorageService.getItem<LoginResponse>(this.userKey);
    const exp = token ? this.getTokenExpiration(token) : null;

    if (token && user && exp && exp > Math.floor(Date.now() / 1000)) {
      this._currentUser.set(user);
      this.scheduleAutoLogout(exp);
    } else {
      this.logout();
    }
  }


  logout(): void {
    this.localStorageService.removeItem(this.tokenKey);
    this.localStorageService.removeItem(this.userKey);
    this._currentUser.set(null);
    if (this.logoutTimer) {
      clearTimeout(this.logoutTimer);
      this.logoutTimer = null;
    }
  }

  private decodeToken(token: string): any | null {
    try {
      return JSON.parse(atob(token.split('.')[1]));
    } catch {
      return null;
    }
  }

  private isTokenValid(token: string): boolean {
    const payload = this.decodeToken(token);
    const now = Math.floor(Date.now() / 1000);
    return payload?.exp && payload.exp > now;
  }

  private getTokenExpiration(token: string): number | null {
    const payload = this.decodeToken(token);
    return payload?.exp ?? null;
  }

  private scheduleAutoLogout(expirationUnix: number): void {
    const now = Math.floor(Date.now() / 1000);
    const expiresIn = expirationUnix - now;

    if (expiresIn > 0) {
      if (this.logoutTimer) {
        clearTimeout(this.logoutTimer);
      }

      this.logoutTimer = setTimeout(() => {
        this.logout();
        console.warn('User has been automatically logged out due to token expiration.');
      }, expiresIn * 1000);
    }
  }


}
