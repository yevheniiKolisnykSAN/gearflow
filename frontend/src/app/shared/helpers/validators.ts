import {
  email,
  PathKind,
  required,
  SchemaPath,
  SchemaPathTree,
  validate,
} from '@angular/forms/signals';
import Child = PathKind.Child;

export const customMaxLength = (value: string, length: number) => {
  return value?.length > length
    ? { kind: 'maxLength', message: `Max length ${length} characters` }
    : null;
};

export const validatorWithMessage = (
  type: ValidatorType,
  schemaPath: SchemaPath<string, 1, Child>,
  params?: Params,
) => {
  switch (type) {
    case ValidatorType.Required:
      return required(schemaPath, { message: 'This field is required' });
    case ValidatorType.Email:
      return email(schemaPath, { message: 'Invalid email address' });
    case ValidatorType.MaxLength:
      return validate(schemaPath, ({ value }) => customMaxLength(value(), params?.maxLength ?? 0));
  }
};

export enum ValidatorType {
  Required,
  Email,
  MaxLength,
}

interface Params {
  maxLength?: number;
}
