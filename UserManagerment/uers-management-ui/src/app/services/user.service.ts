import { Injectable, inject } from '@angular/core';
import { UserModel } from '../interfaces/user-model';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth.service';

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
    const baseHeaders = new HttpHeaders().set('X-Debug-Level', 'minimal');
    await this.http.get<UserModel>(this.baseUrl + '/User/' + id, { headers: baseHeaders.set('Authorization', `Bearer ${this.authService.getAccessToken()}`) })
      .subscribe(response => {
        this.userComonInformation = response;
        debugger
          this.getUserSucceed = true;
      });
    if (this.getUserSucceed) {
      console.log("get user information succeed")
      return this.userComonInformation;
    }
    else {
      console.log("get user information failed")
      return undefined;
    }
  }

  getUserModel() {
    return this.userComonInformation;
  }
}
