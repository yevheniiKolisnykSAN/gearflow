import { FormGroup } from '@angular/forms';

export const markForm = (form: FormGroup) => {
  form.markAllAsTouched();
  form.markAsDirty();
};
