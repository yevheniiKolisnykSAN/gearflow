import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router)
  const toastService = inject(ToastService)

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {

      if (window.opener) {
        return throwError(() => error);
      }

      switch (error.status) {
        case 401:
        case 403:
          router.navigate(['/auth/login'])
          break
        default:
          console.log('error', error);
          break
      }
      toastService.showError(error.error.error)
      return throwError(() => error);
    })
  );
};
