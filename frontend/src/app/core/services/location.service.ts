import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { Location } from '../../shared/models/common.models';

@Injectable({
  providedIn: 'root',
})
export class LocationService extends BaseApiService {
  public getList(): Observable<Location[]> {
    return this.get('/location');
  }

  public deleteItem(id: number): Observable<boolean> {
    return this.delete(`/location/${id}`);
  }

  public updateItem(location: Location): Observable<number> {
    return this.put(`/location/${location.id}`, location);
  }

  public createItem(name: string) {
    return this.post('/location', {name});
  }
}
