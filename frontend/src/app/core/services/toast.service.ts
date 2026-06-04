import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private messageService = inject(MessageService)

  public showError(message: string): void {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: message })
  }

  public showSuccess(message: string): void {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: message })
  }
}
