import { AbstractControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

export enum ValidatorType {
  Required,
  Email,
  MaxLength,
}

interface Params {
  maxLength?: number;
}

export const validatorWithMessage = (type: ValidatorType, params?: Params): ValidatorFn => {
  switch (type) {
    case ValidatorType.Required:
      return (control: AbstractControl): ValidationErrors | null =>
        Validators.required(control) ? { required: { message: 'This field is required' } } : null;
    case ValidatorType.Email:
      return (control: AbstractControl): ValidationErrors | null =>
        Validators.email(control) ? { email: { message: 'Invalid email address' } } : null;
    case ValidatorType.MaxLength:
      return (control: AbstractControl): ValidationErrors | null => {
        const maxLen = params?.maxLength ?? 0;
        return Validators.maxLength(maxLen)(control)
          ? { maxlength: { message: `Max length ${maxLen} characters` } }
          : null;
      };
  }
};
