import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { EquipmentService } from '../../../core/services/equipment.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Equipment } from '../../../shared/models/common.models';
import { CustomLoadingDirective } from '../../../shared/directives/custom-loading.directive';
import { Tooltip } from 'primeng/tooltip';
import { Divider } from 'primeng/divider';
import { Button } from 'primeng/button';
import { DatePicker } from 'primeng/datepicker';
import { FormsModule } from '@angular/forms';
import {
  CreateReservationRequest,
  ReservationService,
} from '../../../core/services/reservation.service';
import { Reservation } from '../../../shared/models/common.models';
import { EMPTY, switchMap } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { DatePipe } from '@angular/common';
import { ConfirmService } from '../../../core/services/confirm.service';

@Component({
  selector: 'app-equipment-item',
  imports: [CustomLoadingDirective, Tooltip, Divider, Button, DatePicker, FormsModule, DatePipe],
  templateUrl: './equipment-item.component.html',
  styleUrl: './equipment-item.component.scss',
})
export class EquipmentItemComponent implements OnInit {
  private readonly equipmentService = inject(EquipmentService);
  private readonly reservationService = inject(ReservationService);
  public readonly authService = inject(AuthService);
  private readonly confirmService = inject(ConfirmService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  private equipmentId = signal<number | null>(null);
  public equipment = signal<Equipment | null>(null);
  public reservation = signal<Reservation | null>(null);
  public loading = signal(false);

  readonly today = new Date();
  selectedDates: Date[] = [];

  readonly maxDate = computed(() => {
    const maxDays = this.equipment()?.maxLoanDays;
    if (!maxDays) return undefined;
    const max = new Date();
    max.setDate(max.getDate() + maxDays);
    return max;
  });

  get selectedDaysCount(): number {
    const [start, end] = this.selectedDates;
    if (!start || !end) return 0;
    return Math.round((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1;
  }

  ngOnInit() {
    if (this.route.snapshot.paramMap.get('id')) {
      this.equipmentId.set(+this.route.snapshot.paramMap.get('id')!);
    } else {
      this.router.navigate(['/equipment']);
      return;
    }

    this.loadData();
  }

  private loadData(): void {
    this.loading.set(true);
    this.equipment.set(null);
    this.reservation.set(null);
    this.selectedDates = []

    this.equipmentService
      .getById(this.equipmentId()!)
      .pipe(
        switchMap((equipment) => {
          this.equipment.set(equipment);
          if (equipment.statusId !== 2) {
            this.loading.set(false);
            return EMPTY;
          }
          return this.reservationService.getByEquipmentId(this.equipmentId()!);
        }),
      )
      .subscribe((reservation) => {
        this.reservation.set(reservation);
        console.log(this.reservation());
        this.loading.set(false);
      });
  }

  reserve() {
    if (this.equipment()?.statusId !== 1 || !this.selectedDates[1]) return;

    const s = this.selectedDates[0];
    const startDate = new Date(Date.UTC(s.getFullYear(), s.getMonth(), s.getDate(), 0, 0, 0, 1));

    const e = this.selectedDates[1];
    const endDate = new Date(e.getFullYear(), e.getMonth(), e.getDate(), 23, 59, 59, 999);

    const request: CreateReservationRequest = {
      equipmentId: this.equipmentId()!,
      startDate,
      endDate,
    };

    this.loading.set(true);
    this.reservationService.create(request).subscribe((res) => {
      this.loadData();
    });
  }

  return() {
    this.confirmService.show(`Are you sure you want to return ${this.equipment()?.name}?`, () => {
      this.loading.set(true);
      this.reservationService.complete(this.reservation()?.id!).subscribe((res) => {
        this.loadData();
      });
    });
  }
}
