import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { TodoApiService } from '../todo-api.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'todo_auth_token';
  private readonly emailKey = 'todo_auth_email';
  private readonly loggedInSubject = new BehaviorSubject<boolean>(this.hasToken());

  readonly isLoggedIn$ = this.loggedInSubject.asObservable();

  constructor(private readonly api: TodoApiService) {}

  get isLoggedIn(): boolean {
    return this.hasToken();
  }

  get currentUser(): string {
    return localStorage.getItem(this.emailKey) ?? '';
  }

  login(email: string, password: string): Observable<{ token: string; email: string }> {
    return this.api.login(email, password).pipe(
      tap((response) => this.storeSession(response.token, response.email)),
    );
  }

  register(email: string, password: string): Observable<{ token: string; email: string }> {
    return this.api.register(email, password).pipe(
      tap((response) => this.storeSession(response.token, response.email)),
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.emailKey);
    this.loggedInSubject.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private storeSession(token: string, email: string): void {
    localStorage.setItem(this.tokenKey, token);
    localStorage.setItem(this.emailKey, email);
    this.loggedInSubject.next(true);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
}
