import { Component, inject, OnInit, signal } from '@angular/core';
import { Reservation } from '../../../shared/models/common.models';
import { ReservationService } from '../../../core/services/reservation.service';
import { Button } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { DatePipe, formatDate } from '@angular/common';
import { DialogService } from 'primeng/dynamicdialog';
import { CompleteReservationDialogComponent } from './complete-reservation-dialog/complete-reservation-dialog.component';
import { Accordion, AccordionContent, AccordionHeader, AccordionPanel } from 'primeng/accordion';
import { Divider } from 'primeng/divider';
import { forkJoin } from 'rxjs';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CsvExportService } from '../../../core/services/csv-export.service';
import { PdfExportService } from '../../../core/services/pdf-export.service';
import { IsReservationStartedPipe } from '../../../shared/pipes/is-reservation-started.pipe';
import { ConfirmService } from '../../../core/services/confirm.service';

@Component({
  selector: 'app-reservation-list',
  imports: [
    Button,
    FormsModule,
    TableModule,
    DatePipe,
    Accordion,
    AccordionPanel,
    AccordionHeader,
    AccordionContent,
    Divider,
    RouterLink,
    IsReservationStartedPipe,
  ],
  templateUrl: './reservation-list.component.html',
  styleUrl: './reservation-list.component.scss',
})
export class ReservationListComponent implements OnInit {
  private readonly reservationService = inject(ReservationService);
  private readonly dialogService = inject(DialogService);
  readonly authService = inject(AuthService);
  readonly confirmService = inject(ConfirmService);
  private readonly csvExport = inject(CsvExportService);
  private readonly pdfExport = inject(PdfExportService);

  public loading = signal(false);
  public reservations = signal<Reservation[]>([]);
  public pending = signal<Reservation[]>([]);
  public history = signal<Reservation[]>([]);

  ngOnInit() {
    this.loadData();
  }

  public loadData() {
    this.loading.set(true);
    if (this.authService.currentUser()?.isAdmin()) {
      this.reservationService.getAdminReservations().subscribe((res) => {
        this.reservations.set(res.active);
        this.history.set(res.history);
        this.pending.set(res.pending);
        this.loading.set(false);
      });
    } else {
      forkJoin([
        this.reservationService.getAllActive(),
        this.reservationService.getHistory(),
        this.reservationService.getPendingReservationByUserId(this.authService.currentUser()?.id!),
      ]).subscribe(([reservations, history, pending]) => {
        this.reservations.set(reservations);
        this.history.set(history);
        this.pending.set(pending);
        this.loading.set(false);
      });
    }
  }

  exportReservationsCsv(): void {
    this.csvExport.export({
      fileName: 'active-reservations.csv',
      columns: ['Equipment', 'From', 'To'],
      rows: this.reservations().map((r) => [
        r.equipment?.name,
        CsvExportService.formatDate(r.startDate),
        CsvExportService.formatDate(r.endDate),
      ]),
    });
  }

  exportReservationsPdf(): void {
    this.pdfExport.export({
      title: 'Active Reservations',
      fileName: 'active-reservations.pdf',
      columns: ['Equipment', 'From', 'To'],
      rows: this.reservations().map((r) => [
        r.equipment?.name,
        CsvExportService.formatDate(r.startDate),
        CsvExportService.formatDate(r.endDate),
      ]),
    });
  }

  exportHistoryCsv(): void {
    this.csvExport.export({
      fileName: 'reservation-history.csv',
      columns: ['Equipment', 'From', 'To', 'Completed At', 'Defect'],
      rows: this.history().map((r) => [
        r.equipment?.name,
        CsvExportService.formatDate(r.startDate),
        CsvExportService.formatDate(r.endDate),
        CsvExportService.formatDate(r.completedAt),
        (r as any).defect?.comment ?? 'No Defect',
      ]),
    });
  }

  exportHistoryPdf(): void {
    this.pdfExport.export({
      title: 'Reservation History',
      fileName: 'reservation-history.pdf',
      columns: ['Equipment', 'From', 'To', 'Completed At', 'Defect'],
      rows: this.history().map((r) => [
        r.equipment?.name,
        CsvExportService.formatDate(r.startDate),
        CsvExportService.formatDate(r.endDate),
        CsvExportService.formatDate(r.completedAt),
        (r as any).defect?.comment ?? 'No Defect',
      ]),
    });
  }

  public complete(reservation: Reservation) {
    this.loading.set(true);
    const ref = this.dialogService.open(CompleteReservationDialogComponent, {
      closable: true,
      header: 'Complete Reservation',
      data: { reservation },
    });

    ref?.onClose.subscribe((res) => {
      console.log('Closed ReservationDialog', res);
      if (res?.shouldComplete) {
        this.reservationService.complete(reservation.id!, res.defectMsg).subscribe((res) => {
          this.loadData();
        });
      }
    });
  }

  public cancel(reservation: Reservation) {
    this.loading.set(true);
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
