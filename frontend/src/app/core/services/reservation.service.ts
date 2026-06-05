import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { map, Observable } from 'rxjs';
import { Reservation, User } from '../../shared/models/common.models';

@Injectable({
  providedIn: 'root',
})
export class ReservationService extends BaseApiService {
  public create(data: CreateReservationRequest): Observable<Reservation> {
    return this.post('/reservation', data);
  }

  public getByEquipmentId(equipmentId: number): Observable<Reservation> {
    return this.get<Reservation>(`/reservation/active/${equipmentId}`).pipe(
      map((reservation: Reservation) => {
        if (reservation.user) {
          reservation.user = new User(reservation.user);
        }

        return reservation;
      }),
    );
  }

  public complete(id: number): Observable<boolean> {
    return this.put(`/reservation/complete/${id}`, {})
  }
}

export interface CreateReservationRequest {
  equipmentId: number;
  startDate: Date;
  endDate: Date;
}
