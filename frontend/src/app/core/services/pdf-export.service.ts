import { Injectable } from '@angular/core';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

type RawRow = (string | number | null | undefined)[];

export interface PdfExportOptions {
  title: string;
  subtitle?: string;
  fileName?: string;
  columns: string[];
  rows: RawRow[];
}

export interface PdfSection {
  heading: string;
  columns: string[];
  rows: RawRow[];
}

@Injectable({ providedIn: 'root' })
export class PdfExportService {

  export(options: PdfExportOptions): void {
    const doc = this.createDoc(options.title, options.subtitle);

    autoTable(doc, {
      head: [options.columns],
      body: options.rows as any,
      startY: options.subtitle ? 28 : 22,
      styles: { fontSize: 9 },
      headStyles: { fillColor: [99, 102, 241] },
      alternateRowStyles: { fillColor: [245, 245, 255] },
    });

    doc.save(options.fileName ?? `${options.title.toLowerCase().replace(/\s+/g, '-')}.pdf`);
  }

  exportMultiSection(title: string, sections: PdfSection[], fileName?: string): void {
    const doc = this.createDoc(title);
    let currentY = 22;

    for (const section of sections) {
      doc.setFontSize(11);
      doc.setTextColor(60, 60, 60);
      doc.text(section.heading, 14, currentY);
      currentY += 4;

      autoTable(doc, {
        head: [section.columns],
        body: section.rows as any,
        startY: currentY,
        styles: { fontSize: 9 },
        headStyles: { fillColor: [99, 102, 241] },
        alternateRowStyles: { fillColor: [245, 245, 255] },
        didDrawPage: (data) => {
          currentY = data.cursor?.y ?? currentY;
        },
      });

      currentY = (doc as any).lastAutoTable.finalY + 12;
    }

    doc.save(fileName ?? `${title.toLowerCase().replace(/\s+/g, '-')}.pdf`);
  }

  private createDoc(title: string, subtitle?: string): jsPDF {
    const doc = new jsPDF();

    doc.setFontSize(16);
    doc.setTextColor(40, 40, 40);
    doc.text(title, 14, 14);

    if (subtitle) {
      doc.setFontSize(10);
      doc.setTextColor(120, 120, 120);
      doc.text(subtitle, 14, 22);
    }

    return doc;
  }
}