import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { Equipment, EquipmentStatus } from '../../shared/models/common.models';

@Injectable({
  providedIn: 'root',
})
export class EquipmentService extends BaseApiService {

  public create(equipment: Equipment): Observable<number> {
    return this.post('/equipment', equipment);
  }

  public update(id: number, equipment: Equipment): Observable<number> {
    return this.put(`/equipment/${id}`, equipment);
  }

  public getList(filters: Record<string, string> = {}, search?: string): Observable<Equipment[]> {
    const params: Record<string, string> = { ...filters };
    if (search) params['search'] = search;
    return this.get<Equipment[]>('/equipment', Object.keys(params).length ? params : undefined);
  }

  public getById(id: number): Observable<Equipment> {
    return this.get(`/equipment/${id}`);
  }

  public deleteItem(id: number): Observable<boolean> {
    return this.delete('/equipment/' + id);
  }

  public getStatuses(): Observable<EquipmentStatus[]> {
    return this.get('/EquipmentStatus');
  }
}
