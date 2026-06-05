import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { Button } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { EquipmentService } from '../../../core/services/equipment.service';
import { Equipment, EquipmentStatus, EquipmentType, Location } from '../../../shared/models/common.models';
import { RouterLink } from '@angular/router';
import { ConfirmService } from '../../../core/services/confirm.service';
import { ToastService } from '../../../core/services/toast.service';
import { TableFilterComponent } from '../../../shared/components/table-filter/table-filter.component';
import { forkJoin, startWith } from 'rxjs';
import { LocationService } from '../../../core/services/location.service';
import { EquipmentTypeService } from '../../../core/services/equipment-type.service';
import { IconField } from 'primeng/iconfield';
import { InputIcon } from 'primeng/inputicon';
import { InputText } from 'primeng/inputtext';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-equipment-list',
  imports: [Button, TableModule, RouterLink, TableFilterComponent, IconField, InputIcon, InputText, ReactiveFormsModule],
  templateUrl: './equipment-list.component.html',
  styleUrl: './equipment-list.component.scss',
})
export class EquipmentListComponent implements OnInit {
  private readonly equipmentService = inject(EquipmentService);
  public readonly authService = inject(AuthService);
  private readonly confirmService = inject(ConfirmService);
  private readonly toastService = inject(ToastService);
  private readonly locationService = inject(LocationService);
  private readonly equipmentTypeService = inject(EquipmentTypeService);

  public equipments = signal<Equipment[]>([]);
  public loading = signal(false);
  public filters = signal<Record<string, string>>({});
  public locations = signal<Location[]>([]);
  public statuses = signal<EquipmentStatus[]>([]);
  public equipmentTypes = signal<EquipmentType[]>([]);

  public MaxLoanDaysValues = [
    { name: '1', value: 1 },
    { name: '3', value: 3 },
    { name: '7', value: 7 },
    { name: '14', value: 14 },
    { name: '30', value: 30 },
  ];

  readonly searchControl = new FormControl('');
  readonly search = toSignal(
    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      startWith(''),
    ),
    { initialValue: '' },
  );

  constructor() {
    effect(() => {
      this.loadData(this.filters(), this.search() ?? '');
    });
  }

  ngOnInit() {
    forkJoin([
      this.locationService.getList(),
      this.equipmentService.getStatuses(),
      this.equipmentTypeService.getList(),
    ]).subscribe(([locations, statuses, equipmentTypes]) => {
      this.locations.set(locations);
      this.statuses.set(statuses);
      this.equipmentTypes.set(equipmentTypes);
    });
  }

  public loadData(filters: Record<string, string> = {}, search = '') {
    this.loading.set(true);
    this.equipmentService.getList(filters, search || undefined).subscribe((res) => {
      this.equipments.set(res);
      this.loading.set(false);
    });
  }

  public filterChanged(key: string, value: any) {
    this.filters.update((current) => {
      const updated = { ...current };
      const isEmpty =
        value === null ||
        value === '' ||
        value === undefined ||
        (Array.isArray(value) && value.length === 0);

      if (isEmpty) {
        delete updated[key];
      } else {
        updated[key] = Array.isArray(value) ? value.join(',') : String(value);
      }
      return updated;
    });
  }

  public onDelete(equipment: Equipment) {
    this.confirmService.show(`Are you sure you what to delete ${equipment.name}?`, () => {
      this.equipmentService.deleteItem(equipment.id!).subscribe(() => {
        this.toastService.showSuccess(`Equipment ${equipment.name} deleted!`);
        this.loadData(this.filters(), this.search() ?? '');
      });
    });
  }
}