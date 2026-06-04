import { inject, Injectable, signal } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { Observable, tap } from 'rxjs';
import { UserModel } from '../../shared/models/user.model';
import { Router } from '@angular/router';
import { loginRequest } from '../auth-config';
import { MsalService } from '@azure/msal-angular';

@Injectable({
  providedIn: 'root',
})
export class AuthService extends BaseApiService {
  private readonly router = inject(Router);
  private readonly msalService = inject(MsalService);

  public currentUser = signal<UserModel | null>(null);

  public register(data: RegisterRequest): Observable<boolean> {
    return this.post<boolean>('/auth/register', data);
  }

  public login(data: LoginRequest): Observable<void> {
    return this.post<void>('/auth/login', data);
  }

  public loginWithMicrosoft(): Promise<void> {
    return this.msalService.instance.loginRedirect(loginRequest);
  }

  public loginWithMicrosoftToken(idToken: string): Observable<void> {
    return this.post<void>('/auth/microsoft', { token: idToken });
  }

  public me(): Observable<UserModel> {
    return this.get<UserModel>('/auth/me');
  }

  public logout(): Observable<void> {
    return this.post<void>('/auth/logout', {}).pipe(
      tap(() => {
        this.currentUser.set(null);
        this.router.navigate(['/login']);
      }),
    );
  }
}

interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

interface LoginRequest {
  email: string;
  password: string;
}
