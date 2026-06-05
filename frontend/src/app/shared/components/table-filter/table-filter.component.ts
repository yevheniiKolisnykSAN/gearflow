import { Component, ElementRef, inject, input, output, signal } from '@angular/core';
import { Button } from 'primeng/button';
import { FormInputComponent } from '../form-input/form-input.component';
import { MultiSelectComponent } from '../multi-select/multi-select.component';
import { Popover } from 'primeng/popover';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-table-filter',
  imports: [Button, FormInputComponent, MultiSelectComponent, Popover, ReactiveFormsModule],
  templateUrl: './table-filter.component.html',
  styleUrl: './table-filter.component.scss',
})
export class TableFilterComponent {
  private readonly elementRef = inject(ElementRef);

  readonly columnLabel = input.required<string>();
  readonly type = input<'text' | 'multiselect'>('text');
  readonly values = input<Record<string, any>[]>([]);
  readonly optionLabel = input<string>('name');
  readonly optionValue = input<string>('id');

  readonly onFilterChanged = output<any>();

  readonly filterControl = new FormControl<any>(null);
  readonly currentFilter = signal<any>(null);

  onSearch(popover: Popover) {
    const val = this.filterControl.value;
    const isEmpty = val === null || val === '' || (Array.isArray(val) && val.length === 0);

    if (isEmpty) {
      this.elementRef.nativeElement.focus();
      popover.toggle(popover);
      return;
    }

    this.currentFilter.set(val);
    this.onFilterChanged.emit(val);
    this.filterControl.reset();
    popover.toggle(popover);
  }

  onDelete(popover: Popover) {
    this.filterControl.reset();
    this.currentFilter.set(null);
    this.onFilterChanged.emit(null);
    popover.toggle(popover);
  }

  onShow() {
    this.filterControl.setValue(this.currentFilter() ?? null);
  }
}