import { inject, Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { catchError, firstValueFrom, from, map, of, switchMap, tap } from 'rxjs';
import { MsalService } from '@azure/msal-angular';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private readonly authService = inject(AuthService);
  private readonly msalService = inject(MsalService);

  public init(): Promise<void> {
    return firstValueFrom(
      from(this.msalService.instance.initialize()).pipe(
        switchMap(() => from(this.msalService.instance.handleRedirectPromise())),
        switchMap((redirectResult) => {
          if (redirectResult?.idToken) {
            return this.authService.loginWithMicrosoftToken(redirectResult.idToken).pipe(
              switchMap(() => this.authService.me())
            );
          }
          return this.authService.me();
        }),
        tap((user) => {
          this.authService.currentUser.set(user);
        }),
        catchError(() => {
          this.authService.currentUser.set(null);
          return of(null);
        }),
        map(() => undefined)
      )
    );
  }
}
