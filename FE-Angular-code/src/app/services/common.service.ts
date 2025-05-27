import { Injectable, inject } from '@angular/core';
import { catchError, lastValueFrom, Observable, of } from 'rxjs';
import { AuthService } from './auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { ServiceModel } from '../interfaces/service-model';

@Injectable({
  providedIn: 'root'
})
export class CommonService {
  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);
  private messageService : string | undefined;
  ListServices: ServiceModel[] | undefined;

  constructor(private http: HttpClient) { }

  async getAllService(): Promise<ServiceModel[] | undefined> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('host', 'http://localhost:4200')
      const response = await lastValueFrom(this.http.get<ServiceModel[]>(`${this.baseUrl}/Service/GetAllServices`,{headers}));
      this.ListServices = response;
      console.log("get ListService succeed");
      return this.ListServices;
    } catch (error) {
      console.error("get ListService failed", error);
      return [];
    }
  }

  private log(message: string) {
  this.messageService = `CommonService: ${message}`;
}
  private handleError<T>(operation = 'operation', result?: T) {
  return (error: any): Observable<T> => {

    // TODO: send the error to remote logging infrastructure
    console.error(error); // log to console instead

    // TODO: better job of transforming error for user consumption
    this.log(`${operation} failed: ${error.message}`);

    // Let the app keep running by returning an empty result.
    return of(result as T);
  };
}
}
