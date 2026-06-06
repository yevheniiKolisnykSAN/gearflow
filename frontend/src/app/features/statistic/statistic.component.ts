import { Component, inject, OnInit, signal } from '@angular/core';
import { StatisticService } from '../../core/services/statistic.service';
import { PdfExportService } from '../../core/services/pdf-export.service';
import { UIChart } from 'primeng/chart';
import { Button } from 'primeng/button';
import { Statistic } from '../../shared/models/common.models';
import { DecimalPipe } from '@angular/common';

const CHART_COLORS = ['#6366f1', '#22c55e', '#f59e0b', '#ef4444', '#3b82f6', '#8b5cf6', '#ec4899', '#14b8a6'];

@Component({
  selector: 'app-statistic',
  imports: [UIChart, DecimalPipe, Button],
  templateUrl: './statistic.component.html',
  styleUrl: './statistic.component.scss',
})
export class StatisticComponent implements OnInit {
  private readonly statisticService = inject(StatisticService);
  private readonly pdfExport = inject(PdfExportService);

  statistics = signal<Statistic | null>(null);
  barChartData = signal<any>(null);
  doughnutChartData = signal<any>(null);

  readonly barChartOptions = {
    responsive: true,
    plugins: {
      legend: { display: false },
    },
    scales: {
      y: { beginAtZero: true, ticks: { stepSize: 1 } },
    },
  };

  readonly doughnutChartOptions = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom', labels: { padding: 16 } },
    },
  };

  ngOnInit() {
    this.statisticService.getStatistics().subscribe((data) => {
      this.statistics.set(data);

      this.barChartData.set({
        labels: data.byMonth.map((m) => m.month),
        datasets: [
          {
            label: 'Reservations',
            data: data.byMonth.map((m) => m.count),
            backgroundColor: '#6366f1',
            borderRadius: 6,
          },
        ],
      });

      this.doughnutChartData.set({
        labels: data.byType.map((t) => t.typeName),
        datasets: [
          {
            data: data.byType.map((t) => t.count),
            backgroundColor: CHART_COLORS,
          },
        ],
      });
    });
  }

  getBarWidth(count: number): number {
    const max = Math.max(...(this.statistics()?.topEquipment.map((e) => e.count) ?? [1]));
    return max === 0 ? 0 : Math.round((count / max) * 100);
  }

  exportPdf(): void {
    const stats = this.statistics();
    if (!stats) return;

    this.pdfExport.exportMultiSection('Statistics Report', [
      {
        heading: 'Summary',
        columns: ['Metric', 'Value'],
        rows: [
          ['Total Reservations', stats.totalReservations],
          ['Active Reservations', stats.activeReservations],
          ['Avg Duration (days)', stats.avgDurationDays.toFixed(1)],
          ['Total Defects', stats.totalDefects],
        ],
      },
      {
        heading: 'Reservations by Month',
        columns: ['Month', 'Count'],
        rows: stats.byMonth.map((m) => [m.month, m.count]),
      },
      {
        heading: 'By Equipment Type',
        columns: ['Type', 'Count'],
        rows: stats.byType.map((t) => [t.typeName, t.count]),
      },
      {
        heading: 'Top Equipment',
        columns: ['#', 'Equipment', 'Reservations'],
        rows: stats.topEquipment.map((e, i) => [i + 1, e.name, e.count]),
      },
    ], 'statistics-report.pdf');
  }
}