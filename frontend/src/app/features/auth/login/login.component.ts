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
import { Popover } from 'primeng/popover';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  imports: [ReactiveFormsModule, Button, Card, FormInputComponent, RouterLink, Popover],
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  public usersList = [
    'jan.kowalski@gearflow.com',
    'anna.wisniewska@gearflow.com',
    'piotr.nowak@gearflow.com',
    'marta.zielinska@gearflow.com',
    'tomasz.wojcik@gearflow.com',
    'karolina.kaminska@gearflow.com',
    'michal.lewandowski@gearflow.com',
    'agnieszka.dabrowska@gearflow.com',
    'bartosz.mazur@gearflow.com',
    'natalia.piotrowska@gearflow.com',
    'krzysztof.grabowski@gearflow.com',
    'ewa.nowicka@gearflow.com'
  ]

  loginForm = new FormGroup({
    email: new FormControl('', [
      validatorWithMessage(ValidatorType.Required),
      validatorWithMessage(ValidatorType.Email),
    ]),
    password: new FormControl('', [validatorWithMessage(ValidatorType.Required)]),
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

  copy(user: string) {
    void navigator.clipboard.writeText(user);
  }
}
