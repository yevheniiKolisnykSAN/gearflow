import { Injectable } from '@angular/core';

export interface CsvExportOptions {
  fileName?: string;
  columns: string[];
  rows: (string | number | null | undefined)[][];
}

@Injectable({ providedIn: 'root' })
export class CsvExportService {

  export(options: CsvExportOptions): void {
    const header = options.columns.join(',');
    const body = options.rows
      .map((row) => row.map((cell) => `"${String(cell ?? '').replace(/"/g, '""')}"`).join(','))
      .join('\n');

    const csv = `${header}\n${body}`;
    const blob = new Blob(['﻿' + csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = options.fileName ?? 'export.csv';
    link.click();
    URL.revokeObjectURL(url);
  }

  static formatDate(date: Date | string | undefined | null): string {
    if (!date) return '';
    const d = new Date(date);
    return `${String(d.getDate()).padStart(2, '0')}-${String(d.getMonth() + 1).padStart(2, '0')}-${d.getFullYear()}`;
  }
}