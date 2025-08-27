import { Injectable } from '@angular/core';
import {environment} from "../../../../environments/environment.development";
import {HttpClient} from "@angular/common/http";
import {User} from "../../interfaces/user-interfaces/user";
import {Observable} from "rxjs";
import {AddLeavesRequest} from "../../interfaces/user-interfaces/add-leaves-request";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly baseUrl = `${environment.apiUrl}/User`;

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.baseUrl}`);
  }

  addLeaves(userId: string, payload: AddLeavesRequest): Observable<any> {
    return this.http.put(`${this.baseUrl}/AddLeaves/${userId}`, payload);
  }
}
