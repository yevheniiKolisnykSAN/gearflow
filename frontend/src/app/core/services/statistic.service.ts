import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { Statistic } from '../../shared/models/common.models';

@Injectable({
  providedIn: 'root',
})
export class StatisticService extends BaseApiService {
  public getStatistics(): Observable<Statistic> {
    return this.get('/statistics');
  }
}
