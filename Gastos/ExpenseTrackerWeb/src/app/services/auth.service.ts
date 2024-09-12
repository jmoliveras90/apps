import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { User } from '../interfaces/user';
import { ApiBaseService } from './api-base.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService extends ApiBaseService {
  private suffix = 'auth';
  private fullUrl = `${this.apiUrl}/${this.suffix}`;

  constructor(private http: HttpClient) {
    super();
  }

  register(userData: {
    username: string;
    email: string;
    password: string;
  }): Observable<User> {
    return this.http.post<User>(`${this.fullUrl}/register`, userData);
  }

  login(credentials: {
    usernameOrEmail: string;
    password: string;
  }): Observable<User> {
    return this.http.post<User>(`${this.fullUrl}/login`, credentials).pipe(
      tap((user: User) => {
        localStorage.setItem('user', JSON.stringify(user));
      })
    );
  }

  logout(): void {
    localStorage.removeItem('user');
  }

  getUserFromLocalStorage(): User | null {
    const userData = localStorage.getItem('user');
    return userData ? JSON.parse(userData) : null;
  }
}
