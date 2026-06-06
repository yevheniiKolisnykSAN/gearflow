import { Component, computed, forwardRef, inject, Injector, input, OnInit, signal } from '@angular/core';
import {
  ControlValueAccessor,
  FormsModule,
  NG_VALUE_ACCESSOR,
  NgControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { InputText } from 'primeng/inputtext';
import { Textarea } from 'primeng/textarea';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-form-input',
  imports: [InputText, Textarea, ReactiveFormsModule, FormsModule, InputNumberModule],
  templateUrl: './form-input.component.html',
  styleUrl: './form-input.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FormInputComponent),
      multi: true,
    },
  ],
})
export class FormInputComponent implements ControlValueAccessor, OnInit {
  readonly type = input<'text' | 'email' | 'password' | 'textarea' | 'number'>('text');
  readonly id = Math.random().toString(36).slice(2);
  readonly label = input('');
  readonly placeholder = input('');

  readonly showPassword = signal(false);
  readonly value = signal<any>('');
  readonly disabled = signal(false);

  readonly inputType = computed(() =>
    this.type() === 'password' && this.showPassword() ? 'text' : this.type(),
  );

  private onChange: (v: any) => void = () => {};
  private onTouched: () => void = () => {};
  private readonly injector = inject(Injector);
  private ngControl: NgControl | null = null;

  ngOnInit(): void {
    this.ngControl = this.injector.get(NgControl, null, { self: true, optional: true });
  }

  get showErrors(): boolean {
    const ctrl = this.ngControl?.control;
    return !!ctrl && (ctrl.touched || ctrl.dirty) && !!ctrl.errors;
  }

  get firstError(): string {
    const ctrl = this.ngControl?.control;
    if (!ctrl?.errors) return '';
    const firstKey = Object.keys(ctrl.errors)[0];
    const err = ctrl.errors[firstKey];
    return typeof err === 'string' ? err : (err?.message ?? firstKey);
  }

  writeValue(val: any): void {
    this.value.set(val ?? '');
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  onInput(event: Event): void {
    const val = (event.target as HTMLInputElement).value;
    this.value.set(val);
    this.onChange(val);
  }

  onNumberChange(val: number | null): void {
    this.value.set(val);
    this.onChange(val);
  }

  onBlur(): void {
    this.onTouched();
  }

  toggleVisibility(): void {
    this.showPassword.update((v) => !v);
  }
}
