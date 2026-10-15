import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of, finalize, map, delay } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  UserResponse,
} from '../shared/models/api.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private http = inject(HttpClient);
  private router = inject(Router);
  currentUser = signal<LoginResponse | null>(this.getUserFromStorage());
  isVerifying = signal(false);

  register(request: RegisterRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.apiUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap((response) => {
        this.setUser(response);
      }),
    );
  }

  logout(): void {
    localStorage.removeItem('user');
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.currentUser()?.token || null;
  }

  verifySession(): Observable<boolean> {
    this.isVerifying.set(true);

    return this.http.get<UserResponse>(`${environment.apiUrl}/auth/me`).pipe(
      delay(1000), // Ensure loader is visible for at least 1s for nice UX
      tap((user) => {
        // Update currentUser with the latest data from the backend
        const current = this.currentUser();
        if (current) {
          this.setUser({ ...current, ...user });
        }
      }),
      catchError(() => {
        // Only logout if we actually thought we were logged in
        if (this.getToken()) {
          this.logout();
        }
        return of(false);
      }),
      finalize(() => this.isVerifying.set(false)),
      map((res) => res !== false),
    );
  }

  private setUser(user: LoginResponse): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }

  private getUserFromStorage(): LoginResponse | null {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      try {
        return JSON.parse(userJson);
      } catch {
        return null;
      }
    }
    return null;
  }
}
