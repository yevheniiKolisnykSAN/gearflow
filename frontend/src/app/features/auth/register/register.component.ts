import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Card } from 'primeng/card';
import { FormInputComponent } from '../../../shared/components/form-input/form-input.component';
import { Button } from 'primeng/button';
import { ValidatorType, validatorWithMessage } from '../../../shared/helpers/validators';
import { markForm } from '../../../shared/helpers/helpers';
import { AuthService } from '../../../core/services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  imports: [ReactiveFormsModule, Card, FormInputComponent, Button, RouterLink],
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toastService = inject(ToastService);

  registerForm = new FormGroup({
    firstName: new FormControl('', [
      validatorWithMessage(ValidatorType.Required),
      validatorWithMessage(ValidatorType.MaxLength, { maxLength: 50 }),
    ]),
    lastName: new FormControl('', [
      validatorWithMessage(ValidatorType.Required),
      validatorWithMessage(ValidatorType.MaxLength, { maxLength: 50 }),
    ]),
    email: new FormControl('', [
      validatorWithMessage(ValidatorType.Required),
      validatorWithMessage(ValidatorType.MaxLength, { maxLength: 50 }),
      validatorWithMessage(ValidatorType.Email),
    ]),
    password: new FormControl('', [
      validatorWithMessage(ValidatorType.Required),
      validatorWithMessage(ValidatorType.MaxLength, { maxLength: 50 }),
    ]),
  });

  submit() {
    if (this.registerForm.invalid) {
      markForm(this.registerForm);
      return;
    }

    const { firstName, lastName, email, password } = this.registerForm.value;
    this.authService
      .register({ firstName: firstName!, lastName: lastName!, email: email!, password: password! })
      .subscribe(() => {
        this.toastService.showSuccess('Register Successful');
        this.router.navigateByUrl('/login');
      });
  }
}
