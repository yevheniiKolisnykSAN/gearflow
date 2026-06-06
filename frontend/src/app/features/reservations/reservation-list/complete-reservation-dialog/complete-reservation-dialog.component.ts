import { Component, inject } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FormInputComponent } from '../../../../shared/components/form-input/form-input.component';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Button } from 'primeng/button';
import { Divider } from 'primeng/divider';

@Component({
  selector: 'app-complete-reservation-dialog',
  imports: [FormInputComponent, FormsModule, ReactiveFormsModule, Button, Divider],
  templateUrl: './complete-reservation-dialog.component.html',
  styleUrl: './complete-reservation-dialog.component.scss',
})
export class CompleteReservationDialogComponent {
  public readonly dynamicDialogConfig = inject(DynamicDialogConfig);
  public readonly dynamicDialogRef = inject(DynamicDialogRef);

  public control = new FormControl(null);

  onComplete() {
    this.dynamicDialogRef.close({ shouldComplete: true, defectMsg: this.control.value });
  }
}
