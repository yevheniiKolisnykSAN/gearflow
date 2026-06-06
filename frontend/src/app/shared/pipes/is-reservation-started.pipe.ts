import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'isReservationStarted',
})
export class IsReservationStartedPipe implements PipeTransform {
  transform(date: Date): unknown {
    if (!date) return false;
    return new Date() >= new Date(date);
  }
}
