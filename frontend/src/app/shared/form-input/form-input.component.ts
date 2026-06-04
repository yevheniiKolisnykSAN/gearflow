import { Component, computed, input, signal } from '@angular/core';
import { InputText } from 'primeng/inputtext';
import { Textarea } from 'primeng/textarea';
import { FieldTree, FormField } from '@angular/forms/signals';

@Component({
  selector: 'app-form-input',
  imports: [InputText, Textarea, FormField],
  templateUrl: './form-input.component.html',
  styleUrl: './form-input.component.scss',
})
export class FormInputComponent {
  readonly type = input<'text' | 'email' | 'password' | 'textarea' | 'number'>('text');
  readonly id = Math.random().toString(36).slice(2);
  readonly label = input('');
  readonly placeholder = input('');
  readonly field = input.required<FieldTree<string>>();

  readonly showPassword = signal(false);

  readonly inputType = computed(() =>
    this.type() === 'password' && this.showPassword() ? 'text' : this.type(),
  );

  private readonly fieldState = computed(() => this.field()());
  readonly errors = computed(() => this.fieldState().errors());
  readonly touched = computed(() => this.fieldState().touched());
  readonly dirty = computed(() => this.fieldState().dirty());
  readonly showErrors = computed(() => {
    return (this.touched() || this.dirty()) && this.errors().length > 0
  });

  toggleVisibility(): void {
    this.showPassword.update((v) => !v);
  }
}
