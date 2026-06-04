import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Button } from 'primeng/button';
import { Card } from 'primeng/card';
import { FormInputComponent } from '../../../shared/form-input/form-input.component';
import { form } from '@angular/forms/signals';
import { ValidatorType, validatorWithMessage } from '../../../shared/helpers/validators';
import { Router, RouterLink } from '@angular/router';
import { markForm } from '../../../shared/helpers/helpers';
import { AuthService } from '../../../core/services/auth.service';
import { switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  imports: [FormsModule, Button, Card, FormInputComponent, RouterLink],
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loginModel = signal<LoginData>({
    email: '',
    password: '',
  });

  loginForm = form(this.loginModel, (schemaPath) => {
    validatorWithMessage(ValidatorType.Required, schemaPath.email);
    validatorWithMessage(ValidatorType.Email, schemaPath.email);

    validatorWithMessage(ValidatorType.Required, schemaPath.password);
  });

  public submit() {
    if (this.loginForm().invalid()) {
      markForm(this.loginForm);
      return;
    }

    this.authService
      .login(this.loginForm().value())
      .pipe(
        switchMap(() => {
          return this.authService.me().pipe(
            tap((res) => {
              this.authService.currentUser.set(res);
            }),
          );
        }),
      )
      .subscribe((res) => {
        console.log('res', res);
        this.router.navigate(['/equipment-list']);
      });
  }

  public loginWithMicrosoft() {
    this.authService.loginWithMicrosoft();
  }
}

interface LoginData {
  email: string;
  password: string;
}
