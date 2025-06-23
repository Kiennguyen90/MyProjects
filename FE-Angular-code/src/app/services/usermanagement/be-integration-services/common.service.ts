import { Injectable, inject } from '@angular/core';
import { catchError, lastValueFrom, throwError } from 'rxjs';
import { AuthService } from '../fe-services/auth.service';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ServiceModel } from '../../../interfaces/service-model';

@Injectable({
  providedIn: 'root'
})
export class CommonService {
  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);
  private messageService: string | undefined;
  ListServices: ServiceModel[] | undefined;
  SeletedService: ServiceModel | undefined;

  constructor(private http: HttpClient) { }

  async getAllService(): Promise<ServiceModel[] | undefined> {
    try {
      const response = await lastValueFrom(this.http.get<ServiceModel[]>(`${this.baseUrl}/user-management/Service/GetAllServices`)
        .pipe(
          catchError((error: HttpErrorResponse) => {
            if (error.error instanceof TypeError) {
              // Client-side or network error
              console.error('An error occurred:', error.error.message);
            } else {
              // Backend error
              console.error(`Backend returned code ${error.status}, body was: ${error.error}`);
            }
            // Return an observable with user-friendly error message
            return throwError(() => new Error('Something bad happened; please try again later.'));
          })
        ));
      this.ListServices = response;
      console.log("get ListService succeed");
      return this.ListServices;
    } catch (error) {
      console.error("get ListService failed", error);
      return [];
    }
  }

  async getServiceById(serviceId: string): Promise<ServiceModel | undefined> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.get<ServiceModel>(`${this.baseUrl}/user-management/Service/${serviceId}`, { headers })
        .pipe(
          catchError((error: HttpErrorResponse) => {
            if (error.error instanceof TypeError) {
              // Client-side or network error
              console.error('An error occurred:', error.error.message);
            } else {
              // Backend error
              console.error(`Backend returned code ${error.status}, body was: ${error.error}`);
            }
            // Return an observable with user-friendly error message
            return throwError(() => new Error('Something bad happened; please try again later.'));
          })
        ));
      this.SeletedService = response;
      console.log("get Service succeed");
      return this.SeletedService;
    } catch (error) {
      console.error("get Service failed", error);
      return undefined;
    }
  }

  async registerService(serviceId: string, typeId: number): Promise<boolean> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.post<boolean>(`${this.baseUrl}/user-management/Service/registerService`, { serviceId, typeId }, { headers }));
      if (response) {
        console.log("register service succeed");
        return true;
      } else {
        console.error("register service failed");
        return false;
      }
    }
    catch (error) {
      console.error("register service failed", error);
      return false;
    }
  }

  getMessageService(): string | undefined {
    return this.messageService;
  }

  setMessageService(message: string): void {
    this.messageService = message;
  }
}



