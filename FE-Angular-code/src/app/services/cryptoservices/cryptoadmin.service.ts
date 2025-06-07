
import { lastValueFrom } from 'rxjs';
import { UserModel } from '../../interfaces/user-model';
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../usermanagement/auth.service';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CryptoadminService {

  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);
  constructor(private http: HttpClient) { }

  async getUserById(id: string): Promise<UserModel | undefined> {
      try {
        const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);

        const response = await lastValueFrom(this.http.get<UserModel>(`${this.baseUrl}/User/${id}`, { headers }));
        console.log("get user information succeed");
      } catch (error) {
        console.error("get user information failed", error);
        return undefined;
      }
    }
}
