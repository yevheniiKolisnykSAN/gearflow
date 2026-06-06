import { inject, Injectable } from '@angular/core';
import { ConfirmationService } from 'primeng/api';

@Injectable({
  providedIn: 'root',
})
export class ConfirmService {
  private confirmationService = inject(ConfirmationService);

  public show(message: string, onDelete: () => void, btnConfig?: ConfirmButtonConfig) {
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
        label: btnConfig?.label ?? 'Delete',
        severity: btnConfig?.severity ?? 'danger'
      },

      accept: () => {
        onDelete()
      },
    });
  }
}

interface ConfirmButtonConfig {
  severity: 'success' | 'error' | 'danger';
  label: string;
}
