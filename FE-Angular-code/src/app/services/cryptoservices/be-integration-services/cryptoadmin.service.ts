
import { lastValueFrom } from 'rxjs';
import { UserModel } from '../../../interfaces/user-model';
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../usermanagement/fe-services/auth.service';
import { environment } from '../../../../environments/environment';

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

  async updateUser(user: UserModel): Promise<UserModel | undefined> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.put<UserModel>(`${this.baseUrl}/User/${user.id}`, user, { headers }))
    }
    catch (error) {
      console.error("update user information failed", error);
      return undefined;
    };
  }

  async addUser(user: UserModel) : Promise<boolean> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.post<boolean>(`${this.baseUrl}/User/${user.id}`, user, { headers }))
      return response;
    }
    catch (error) {
      console.error("add user information failed", error);
      return false;
    };
  }
}
