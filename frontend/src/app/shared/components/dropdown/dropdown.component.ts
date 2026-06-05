import { Component, forwardRef, inject, Injector, input, OnInit, signal } from '@angular/core';
import {
  ControlValueAccessor,
  FormsModule,
  NG_VALUE_ACCESSOR,
  NgControl,
} from '@angular/forms';
import { Select } from 'primeng/select';

@Component({
  selector: 'app-dropdown',
  imports: [Select, FormsModule],
  templateUrl: './dropdown.component.html',
  styleUrl: './dropdown.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DropdownComponent),
      multi: true,
    },
  ],
})
export class DropdownComponent implements ControlValueAccessor, OnInit {
  readonly label = input<string>();
  readonly optionValue = input.required<string>();
  readonly optionLabel = input.required<string>();
  readonly values = input<Record<string, any>[]>([]);

  readonly value = signal<any>(null);
  readonly disabled = signal(false);

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
    this.value.set(val ?? null);
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

  onValueChange(val: any): void {
    this.value.set(val);
    this.onChange(val);
  }

  onBlur(): void {
    this.onTouched();
  }
}
