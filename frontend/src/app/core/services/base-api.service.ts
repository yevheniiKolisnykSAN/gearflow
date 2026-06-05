import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BaseApiService {
  private readonly httpClient = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  public get<T>(url: string, params?: Record<string, string>): Observable<T> {
    const httpParams = params
      ? new HttpParams({ fromObject: params })
      : undefined;
    return this.httpClient.get<T>(this.apiUrl + url, { params: httpParams });
  }

  public post<T>(url: string, data: Record<string, any>, params?: Record<string, string>): Observable<T> {
    const httpParams = params ? new HttpParams({ fromObject: params }) : undefined;
    return this.httpClient.post<T>(this.apiUrl + url, data, { params: httpParams });
  }

  public put<T>(url: string, data: Record<string, any>): Observable<T> {
    return this.httpClient.put<T>(this.apiUrl + url, data);
  }

  public delete<T>(url: string): Observable<T> {
    return this.httpClient.delete<T>(this.apiUrl + url);
  }
}