import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BaseApiService {
  private readonly httpClient = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  public get<T>(url: string): Observable<T> {
    return this.httpClient.get<T>(this.apiUrl + url)
  }

  public post<T>(url: string, data: Record<string, any>): Observable<T> {
    return this.httpClient.post<T>(this.apiUrl + url, data)
  }
}
