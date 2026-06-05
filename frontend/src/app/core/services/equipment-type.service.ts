import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { EquipmentType } from '../../shared/models/common.models';

@Injectable({
  providedIn: 'root',
})
export class EquipmentTypeService extends BaseApiService {
  public getList(): Observable<EquipmentType[]> {
    return this.get('/EquipmentType');
  }

  public deleteItem(id: number): Observable<boolean> {
    return this.delete(`/EquipmentType/${id}`);
  }

  public updateItem(equipmentType: EquipmentType): Observable<number> {
    return this.put(`/EquipmentType/${equipmentType.id}`, equipmentType);
  }

  public createItem(name: string) {
    return this.post('/EquipmentType', {name});
  }
}
