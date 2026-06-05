import { inject, Injectable } from '@angular/core';
import { ConfirmationService } from 'primeng/api';

@Injectable({
  providedIn: 'root',
})
export class ConfirmService {
  private confirmationService = inject(ConfirmationService);

  public show(message: string, onDelete: () => void) {
    this.confirmationService.confirm({
      message,
      icon: 'pi pi-info-circle',
      closable: false,
      rejectLabel: 'Cancel',
      rejectButtonProps: {
        label: 'Cancel',
        severity: 'secondary',
        outlined: true
      },
      acceptButtonProps: {
        label: 'Delete',
        severity: 'danger'
      },

      accept: () => {
        onDelete()
      },
    });
  }
}
