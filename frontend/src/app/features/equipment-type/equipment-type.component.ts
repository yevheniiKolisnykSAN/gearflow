import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { Button } from 'primeng/button';
import { FormInputComponent } from '../../shared/components/form-input/form-input.component';
import { Popover } from 'primeng/popover';
import { TableModule } from 'primeng/table';
import { ConfirmService } from '../../core/services/confirm.service';
import { ToastService } from '../../core/services/toast.service';
import { EquipmentType } from '../../shared/models/common.models';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ValidatorType, validatorWithMessage } from '../../shared/helpers/validators';
import { map } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EquipmentTypeService } from '../../core/services/equipment-type.service';

@Component({
  selector: 'app-equipment-type',
  imports: [Button, FormInputComponent, Popover, TableModule, ReactiveFormsModule],
  templateUrl: './equipment-type.component.html',
  styleUrl: './equipment-type.component.scss',
})
export class EquipmentTypeComponent implements OnInit {
  public destroyRef = inject(DestroyRef);

  private readonly equipmentTypeService = inject(EquipmentTypeService);
  private readonly confirmService = inject(ConfirmService);
  private toastService = inject(ToastService);

  public equipmentTypes = signal<EquipmentType[]>([]);
  public loading = signal(false);
  public createFormControl = new FormControl('', [validatorWithMessage(ValidatorType.Required)]);

  ngOnInit() {
    this.loadData();
  }

  public loadData(): void {
    this.loading.set(true);
    this.equipmentTypeService
      .getList()
      .pipe(
        map((equipmentTypes) => {
          equipmentTypes.forEach((t) => (t.isEditing = false));
          return equipmentTypes;
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((equipmentTypes) => {
        this.equipmentTypes.set(equipmentTypes);
        this.loading.set(false);
      });
  }

  onDelete(item: EquipmentType) {
    this.confirmService.show(
      `Are you sure you what to delete this equipment type (${item.name})?`,
      () => {
        this.equipmentTypeService.deleteItem(item.id).subscribe(() => {
          this.toastService.showSuccess(`Equipment type ${item.name} deleted!`);
          this.loadData();
        });
      },
    );
  }

  toggleEditing(id: number, state: boolean) {
    this.equipmentTypes.update((equipmentTypes) =>
      equipmentTypes.map((t) => {
        if (t.id !== id) return t;
        return {
          ...t,
          isEditing: state,
          editControl: state ? new FormControl(t.name) : undefined,
        };
      }),
    );
  }

  saveEditing(equipmentType: EquipmentType) {
    const name = equipmentType.editControl?.value?.trim();
    if (!name) return;

    if (name === equipmentType.name) {
      this.equipmentTypes.update((equipmentTypes) =>
        equipmentTypes.map((l) =>
          l.id === equipmentType.id ? { ...l, name, isEditing: false, editControl: undefined } : l,
        ),
      );

      return;
    }

    equipmentType.name = name;

    this.equipmentTypeService.updateItem(equipmentType).subscribe(() => {
      this.toastService.showSuccess(`Equipment type ${equipmentType.name} updated!`);
      this.loadData();
    });
  }

  create(popover: Popover) {
    this.createFormControl.markAsTouched();

    if (this.createFormControl.invalid) return;

    popover.hide();

    this.loading.set(true);

    this.equipmentTypeService.createItem(this.createFormControl.value!).subscribe(() => {
      this.createFormControl.reset();
      this.loadData();
    });
  }
}
