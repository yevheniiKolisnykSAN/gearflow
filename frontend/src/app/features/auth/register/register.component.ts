import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Card } from 'primeng/card';
import { FormInputComponent } from '../../../shared/form-input/form-input.component';
import { Button } from 'primeng/button';
import { form} from '@angular/forms/signals';
import {
  ValidatorType,
  validatorWithMessage,
} from '../../../shared/helpers/validators';
import { markForm } from '../../../shared/helpers/helpers';
import { AuthService } from '../../../core/services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  imports: [FormsModule, Card, FormInputComponent, Button, RouterLink],
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toastService = inject(ToastService);

  registerModel = signal<RegisterData>({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
  });

  registerForm = form(this.registerModel, (schemaPath) => {
    validatorWithMessage(ValidatorType.Required, schemaPath.firstName);
    validatorWithMessage(ValidatorType.MaxLength, schemaPath.firstName, { maxLength: 50 });

    validatorWithMessage(ValidatorType.Required, schemaPath.lastName);
    validatorWithMessage(ValidatorType.MaxLength, schemaPath.lastName, { maxLength: 50 });

    validatorWithMessage(ValidatorType.Required, schemaPath.email);
    validatorWithMessage(ValidatorType.MaxLength, schemaPath.email, { maxLength: 50 });
    validatorWithMessage(ValidatorType.Email, schemaPath.email);

    validatorWithMessage(ValidatorType.Required, schemaPath.password);
    validatorWithMessage(ValidatorType.MaxLength, schemaPath.password, { maxLength: 50 });
  });

  submit() {
    if (this.registerForm().invalid()) {
      markForm(this.registerForm);
      return;
    }

    this.authService.register(this.registerForm().value()).subscribe((res) => {
      this.toastService.showSuccess('Register Successful');
      this.router.navigateByUrl('/login');
    });
  }
}

interface RegisterData {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}
