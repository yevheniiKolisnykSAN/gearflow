import { Component, inject, OnInit, signal } from '@angular/core';
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
import { EMPTY, forkJoin, switchMap } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { DatePipe, formatDate } from '@angular/common';
import { ConfirmService } from '../../../core/services/confirm.service';
import { Accordion, AccordionContent, AccordionHeader, AccordionPanel } from 'primeng/accordion';
import { TableModule } from 'primeng/table';
import { CompleteReservationDialogComponent } from '../../reservations/reservation-list/complete-reservation-dialog/complete-reservation-dialog.component';
import { DialogService, DynamicDialogStyle } from 'primeng/dynamicdialog';
import { DialogStyle } from 'primeng/dialog';
import { IsReservationStartedPipe } from '../../../shared/pipes/is-reservation-started.pipe';

@Component({
  selector: 'app-equipment-item',
  imports: [
    CustomLoadingDirective,
    Tooltip,
    Divider,
    Button,
    DatePicker,
    FormsModule,
    DatePipe,
    Accordion,
    AccordionPanel,
    AccordionHeader,
    AccordionContent,
    TableModule,
    IsReservationStartedPipe,
  ],
  templateUrl: './equipment-item.component.html',
  styleUrl: './equipment-item.component.scss',
})
export class EquipmentItemComponent implements OnInit {
  private readonly equipmentService = inject(EquipmentService);
  private readonly reservationService = inject(ReservationService);
  public readonly authService = inject(AuthService);
  private readonly dialogService = inject(DialogService);
  private readonly confirmService = inject(ConfirmService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  private equipmentId = signal<number | null>(null);
  public equipment = signal<Equipment | null>(null);
  public reservation = signal<Reservation | null>(null);
  public activeReservations = signal<Reservation[]>([]);
  public loading = signal(false);

  readonly today = new Date();
  selectedDates: Date[] = [];
  readonly disabledDates = signal<Date[]>([]);
  readonly dynamicMaxDate = signal<Date | undefined>(undefined);

  get selectedDaysCount(): number {
    const [start, end] = this.selectedDates;
    if (!start || !end) return 0;
    return Math.round((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1;
  }

  onDateSelect(): void {
    const [start, end] = this.selectedDates;
    if (start && !end) {
      this.calcDynamicMax(start);
    } else {
      this.dynamicMaxDate.set(undefined);
    }
  }

  private calcDynamicMax(start: Date): void {
    const maxLoanDays = this.equipment()?.maxLoanDays ?? 365;

    const maxFromLoan = new Date(start);
    maxFromLoan.setDate(maxFromLoan.getDate() + maxLoanDays - 1);

    const startDay = new Date(start.getFullYear(), start.getMonth(), start.getDate());

    const firstDisabled = this.disabledDates()
      .map((d) => new Date(d.getFullYear(), d.getMonth(), d.getDate()))
      .filter((d) => d.getTime() > startDay.getTime())
      .sort((a, b) => a.getTime() - b.getTime())[0];

    if (firstDisabled) {
      const dayBeforeDisabled = new Date(firstDisabled);
      dayBeforeDisabled.setDate(dayBeforeDisabled.getDate() - 1);
      this.dynamicMaxDate.set(maxFromLoan < dayBeforeDisabled ? maxFromLoan : dayBeforeDisabled);
    } else {
      this.dynamicMaxDate.set(maxFromLoan);
    }
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
    this.selectedDates = [];

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

    forkJoin([
      this.reservationService.getReservedDates(this.equipmentId()!),
      this.reservationService.getActiveListByEquipmentId(this.equipmentId()!),
    ]).subscribe(([reservedDate, activeList]) => {
      this.disabledDates.set(reservedDate.map((d) => new Date(d)));
      this.activeReservations.set(activeList);
    });
  }

  reserve() {
    if (!this.selectedDates[1]) return;

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

  public complete(reservation: Reservation) {
    const ref = this.dialogService.open(CompleteReservationDialogComponent, {
      closable: true,
      header: 'Complete Reservation',
      data: { reservation },
    });

    ref?.onClose.subscribe((res) => {
      if (res?.shouldComplete) {
        this.reservationService.complete(reservation.id!, res.defectMsg).subscribe((res) => {
          this.loadData();
        });
      }
    });
  }

  public cancel(reservation: Reservation) {
    this.confirmService.show(
      `Are you sure you want to cancel your reservation from:
    ${formatDate(reservation.startDate, 'dd-MM-yyyy', 'en-US')} to:
    ${formatDate(reservation.endDate, 'dd-MM-yyyy', 'en-US')}
     for ${reservation.equipment?.name}?`,
      () => {
        this.reservationService.cancel(reservation.id!).subscribe((res) => {
          this.loadData();
        });
      },
      {
        severity: 'success',
        label: 'Confirm',
      },
    );
  }
}
