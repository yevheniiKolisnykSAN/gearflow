import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Button } from 'primeng/button';
import { Card } from 'primeng/card';
import { FormInputComponent } from '../../../shared/components/form-input/form-input.component';
import { ValidatorType, validatorWithMessage } from '../../../shared/helpers/validators';
import { Router, RouterLink } from '@angular/router';
import { markForm } from '../../../shared/helpers/helpers';
import { AuthService } from '../../../core/services/auth.service';
import { switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  imports: [ReactiveFormsModule, Button, Card, FormInputComponent, RouterLink],
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loginForm = new FormGroup({
    email: new FormControl('', [
      validatorWithMessage(ValidatorType.Required),
      validatorWithMessage(ValidatorType.Email),
    ]),
    password: new FormControl('', [
      validatorWithMessage(ValidatorType.Required),
    ]),
  });

  public submit() {
    if (this.loginForm.invalid) {
      markForm(this.loginForm);
      return;
    }

    const { email, password } = this.loginForm.value;
    this.authService
      .login({ email: email!, password: password! })
      .pipe(
        switchMap(() =>
          this.authService.me().pipe(
            tap((res) => {
              this.authService.currentUser.set(res);
            }),
          ),
        ),
      )
      .subscribe(() => {
        this.router.navigate(['/equipment']);
      });
  }

  public loginWithMicrosoft() {
    this.authService.loginWithMicrosoft();
  }
}
