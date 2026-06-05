import { Component, inject, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Button } from 'primeng/button';
import { Card } from 'primeng/card';
import { FormInputComponent } from '../../../shared/components/form-input/form-input.component';
import { ActivatedRoute, Router } from '@angular/router';
import { DropdownComponent } from '../../../shared/components/dropdown/dropdown.component';
import { markForm } from '../../../shared/helpers/helpers';
import { ValidatorType, validatorWithMessage } from '../../../shared/helpers/validators';
import { LocationService } from '../../../core/services/location.service';
import {
  Equipment,
  EquipmentStatus,
  EquipmentType,
  Location,
} from '../../../shared/models/common.models';
import { EquipmentService } from '../../../core/services/equipment.service';
import { EquipmentTypeService } from '../../../core/services/equipment-type.service';
import { forkJoin } from 'rxjs';
import { CustomLoadingDirective } from '../../../shared/directives/custom-loading.directive';

@Component({
  selector: 'app-equipment-form',
  imports: [
    Button,
    Card,
    FormInputComponent,
    ReactiveFormsModule,
    DropdownComponent,
    CustomLoadingDirective,
  ],
  templateUrl: './equipment-form.component.html',
  styleUrl: './equipment-form.component.scss',
})
export class EquipmentFormComponent implements OnInit {
  private readonly locationService = inject(LocationService);
  private readonly equipmentService = inject(EquipmentService);
  private readonly equipmentTypeService = inject(EquipmentTypeService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  public locations = signal<Location[]>([]);
  public statuses = signal<EquipmentStatus[]>([]);
  public equipmentTypes = signal<EquipmentType[]>([]);
  public isEditMode = signal(false)
  private equipmentId = signal<number | null>(null)
  public loading = signal(false);

  equipmentForm = new FormGroup({
    name: new FormControl('', [validatorWithMessage(ValidatorType.Required)]),
    specification: new FormControl('', [validatorWithMessage(ValidatorType.Required)]),
    maxLoanDays: new FormControl<number | null>(7, [validatorWithMessage(ValidatorType.Required)]),
    statusId: new FormControl<number | null>(null, [validatorWithMessage(ValidatorType.Required)]),
    typeId: new FormControl<number | null>(null, [validatorWithMessage(ValidatorType.Required)]),
    locationId: new FormControl<number | null>(null, [
      validatorWithMessage(ValidatorType.Required),
    ]),
  });

  ngOnInit() {
    this.loading.set(true);

    this.equipmentId.set(this.route.snapshot.params['id'])
    this.isEditMode.set(!!this.equipmentId())

    forkJoin([
      this.locationService.getList(),
      this.equipmentService.getStatuses(),
      this.equipmentTypeService.getList(),
    ]).subscribe(([locations, statuses, equipmentTypes]) => {
      this.locations.set(locations);
      this.statuses.set(statuses);
      this.equipmentTypes.set(equipmentTypes);

      this.loading.set(false);
    });

    if (this.isEditMode()) {
      this.equipmentService.getById(this.equipmentId()!).subscribe((equipment) => {
        console.log('equipment', equipment);
        this.equipmentForm.patchValue(equipment);
      })
    }
  }

  submit() {
    this.equipmentForm.markAsDirty();
    this.equipmentForm.markAsTouched();
    markForm(this.equipmentForm);

    if (!this.equipmentForm.valid) return;
    this.loading.set(true);

    const request$ = this.isEditMode()
      ? this.equipmentService.update(this.equipmentId()!, this.equipmentForm.value as Equipment)
      : this.equipmentService.create(this.equipmentForm.value as Equipment);

    request$.subscribe(() => {
      this.loading.set(false);
      this.router.navigate(['/equipment']);
    });
  }
}
