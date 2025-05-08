import { Injectable, inject } from '@angular/core';
import { UserModel } from '../interfaces/user-model';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth.service';
import { lastValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);
  userComonInformation: UserModel = { userId: '', email: '', fullName: '', phoneNumber: '', avatar: '', userRole: '', services: [] };
  getUserSucceed: boolean | undefined;

  constructor(private http: HttpClient) { }

  async getUserById(id: string): Promise<UserModel | undefined> {
    const headers = new HttpHeaders()
      .set('X-Debug-Level', 'minimal')
      .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);

    try {
      const response = await lastValueFrom(this.http.get<UserModel>(`${this.baseUrl}/User/${id}`, { headers }));
      this.userComonInformation = response;
      this.getUserSucceed = true;
      console.log("get user information succeed");
      return this.userComonInformation;
    } catch (error) {
      console.error("get user information failed", error);
      this.getUserSucceed = false;
      return undefined;
    }
  }

  getUserModel() {
    return this.userComonInformation;
  }
}
