import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { TableModule } from 'primeng/table';
import { Button } from 'primeng/button';
import { LocationService } from '../../core/services/location.service';
import { Location } from '../../shared/models/common.models';
import { ConfirmService } from '../../core/services/confirm.service';
import { ToastService } from '../../core/services/toast.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { FormInputComponent } from '../../shared/components/form-input/form-input.component';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Popover } from 'primeng/popover';
import { ValidatorType, validatorWithMessage } from '../../shared/helpers/validators';

@Component({
  selector: 'app-locations',
  imports: [
    TableModule,
    Button,
    FormInputComponent,
    FormsModule,
    ReactiveFormsModule,
    Popover,
  ],
  templateUrl: './locations.component.html',
  styleUrl: './locations.component.scss',
})
export class LocationsComponent implements OnInit {
  public destroyRef = inject(DestroyRef);

  private readonly locationService = inject(LocationService);
  private readonly confirmService = inject(ConfirmService);
  private toastService = inject(ToastService);

  public locations = signal<Location[]>([]);
  public loading = signal(false);
  public createFormControl = new FormControl('', [validatorWithMessage(ValidatorType.Required)]);

  ngOnInit() {
    this.loadData();
  }

  public loadData(): void {
    this.loading.set(true);
    this.locationService
      .getList()
      .pipe(
        map((locations) => {
          locations.forEach((l) => (l.isEditing = false));
          return locations;
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((locations) => {
        this.locations.set(locations);
        this.loading.set(false);
      });
  }

  onDelete(item: Location) {
    this.confirmService.show(
      `Are you sure you what to delete this location (${item.name})?`,
      () => {
        this.locationService.deleteItem(item.id).subscribe(() => {
          this.toastService.showSuccess(`Location ${item.name} deleted!`);
          this.loadData();
        });
      },
    );
  }

  toggleEditing(id: number, state: boolean) {
    this.locations.update((locations) =>
      locations.map((l) => {
        if (l.id !== id) return l;
        return {
          ...l,
          isEditing: state,
          editControl: state ? new FormControl(l.name) : undefined,
        };
      }),
    );
  }

  saveEditing(location: Location) {
    const name = location.editControl?.value?.trim();
    if (!name) return;

    if (name === location.name) {
      this.locations.update((locations) =>
        locations.map((l) =>
          l.id === location.id ? { ...l, name, isEditing: false, editControl: undefined } : l,
        ),
      );

      return;
    }

    location.name = name;

    this.locationService.updateItem(location).subscribe(() => {
      this.toastService.showSuccess(`Location ${location.name} updated!`);
      this.loadData();
    });
  }

  create(popover: Popover) {
    this.createFormControl.markAsTouched();

    if (this.createFormControl.invalid) return;

    popover.hide();

    this.loading.set(true);

    this.locationService.createItem(this.createFormControl.value!).subscribe(() => {
      this.createFormControl.reset();
      this.loadData();
    });
  }
}
