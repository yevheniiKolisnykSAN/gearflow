import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { map, Observable } from 'rxjs';
import { AdminReservations, Reservation, User } from '../../shared/models/common.models';

@Injectable({
  providedIn: 'root',
})
export class ReservationService extends BaseApiService {
  public getAllActive(): Observable<Reservation[]> {
    return this.get<Reservation[]>('/reservation');
  }

  public create(data: CreateReservationRequest): Observable<Reservation> {
    return this.post<Reservation>('/reservation', data);
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

  public getActiveListByEquipmentId(id: number): Observable<Reservation[]> {
    return this.get<Reservation[]>('/reservation/active-by-equipment/' + id).pipe(
      map((reservations: Reservation[]) =>
        reservations.map((r) => {
          if (r.user) r.user = new User(r.user);
          return r;
        }),
      ),
    );
  }

  public complete(id: number, defectComment: string): Observable<boolean> {
    return this.put<boolean>(`/reservation/complete/${id}`, { defectComment });
  }

  public cancel(id: number): Observable<boolean> {
    return this.put<boolean>(`/reservation/cancel/${id}`, {});
  }

  public getHistory(): Observable<Reservation[]> {
    return this.get<Reservation[]>('/reservation/history');
  }

  public getReservedDates(id: number): Observable<Date[]> {
    return this.get<Date[]>(`/reservation/dates/${id}`);
  }

  public getPendingReservationList(): Observable<Reservation[]> {
    return this.get<Reservation[]>('/reservation/pending').pipe(
      map((reservations: Reservation[]) => {
        return reservations.map((r) => {
          if (r.user) {
            r.user = new User(r.user);
          }
          return r;
        });
      }),
    );
  }

  public getPendingReservationByUserId(id: number): Observable<Reservation[]> {
    return this.get<Reservation[]>(`/reservation/pending/${id}`).pipe(
      map((reservations: Reservation[]) => {
        return reservations.map((r) => {
          if (r.user) {
            r.user = new User(r.user);
          }
          return r;
        });
      }),
    );
  }

  public confirmReservation(id: number, defectComment: string | null): Observable<boolean> {
    return this.put<boolean>(`/reservation/confirm/${id}`, { defectComment });
  }

  public getAdminReservations(): Observable<AdminReservations>{
    return this.get<AdminReservations>('/reservation/admin');
  }
}

export interface CreateReservationRequest {
  equipmentId: number;
  startDate: Date;
  endDate: Date;
}
