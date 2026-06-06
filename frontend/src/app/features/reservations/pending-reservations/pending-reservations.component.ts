import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ReservationService } from '../../../core/services/reservation.service';
import { Reservation } from '../../../shared/models/common.models';
import { DatePipe } from '@angular/common';
import { Textarea } from 'primeng/textarea';
import { Button } from 'primeng/button';
import { ConfirmService } from '../../../core/services/confirm.service';
import { CsvExportService } from '../../../core/services/csv-export.service';
import { PdfExportService } from '../../../core/services/pdf-export.service';

@Component({
  selector: 'app-pending-reservations',
  imports: [FormsModule, TableModule, DatePipe, Textarea, Button],
  templateUrl: './pending-reservations.component.html',
  styleUrl: './pending-reservations.component.scss',
})
export class PendingReservationsComponent implements OnInit {
  private readonly reservationService = inject(ReservationService);
  private readonly confirmService = inject(ConfirmService);
  private readonly csvExport = inject(CsvExportService);
  private readonly pdfExport = inject(PdfExportService);

  public reservations = signal<Reservation[]>([]);
  public loading = signal(false);

  ngOnInit() {
    this.loadData();
  }

  public loadData() {
    this.loading.set(true);
    this.reservationService.getPendingReservationList().subscribe((reservations) => {
      this.reservations.set(reservations);
      this.loading.set(false);
    });
  }

  private readonly exportColumns = ['Equipment', 'Made By', 'Sent At', 'Defect Comment'];

  private getExportRows() {
    return this.reservations().map((r) => [
      r.equipment?.name, r.user?.fullName,
      CsvExportService.formatDate(r.pendingAt),
      (r as any).defect?.comment ?? '',
    ]);
  }

  exportCsv(): void {
    this.csvExport.export({ fileName: 'pending-reservations.csv', columns: this.exportColumns, rows: this.getExportRows() });
  }

  exportPdf(): void {
    this.pdfExport.export({ title: 'Pending Reservations', fileName: 'pending-reservations.pdf', columns: this.exportColumns, rows: this.getExportRows() });
  }

  public confirm(reservation: Reservation, value?: string) {
    let defectComment = !!value ? value : null;
    this.confirmService.show(
      `Are you sure want to confirm this pending reservation for (${reservation.equipment?.name})?`,
      () => {
        this.reservationService
          .confirmReservation(reservation.id!, defectComment)
          .subscribe((res) => {
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
