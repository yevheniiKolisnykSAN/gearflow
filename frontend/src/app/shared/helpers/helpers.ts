import { FieldTree } from '@angular/forms/signals';

export const markForm = <T>(form: FieldTree<T, string | number>) => {
  form().markAsDirty();
  form().markAsTouched();

  Object.values(form).forEach((value: any) => {
    if (typeof value === 'function') {
      value().markAsDirty?.();
      value().markAsTouched?.();
    }
  });
};
