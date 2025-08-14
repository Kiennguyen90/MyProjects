
import { lastValueFrom } from 'rxjs';
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../usermanagement/fe-services/auth.service';
import { environment } from '../../../../environments/environment';
import { ListUserResponeModel } from '../../../interfaces/crypto/user-model';
import { BaseResponseModel } from '../../../interfaces/base-respone-model';
import { EditUserData } from '../../../components/crypto-components/dialogs/edit-user/edit-user.component';

@Injectable({
  providedIn: 'root'
})
export class CryptoadminService {

  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);
  constructor(private http: HttpClient) { }

  async addUser(email: string, userName: string, phoneNumber: string): Promise<BaseResponseModel> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.post<BaseResponseModel>(`${this.baseUrl}/crypto-service/User`, { email: email, userName: userName, phoneNumber: phoneNumber }, { headers }))
      return response;
    }
    catch (error) {
      console.error("add user failed", error);
      return error as BaseResponseModel;
    };
  }

  async updateUser(data : EditUserData) : Promise<BaseResponseModel> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.put<BaseResponseModel>(`${this.baseUrl}/crypto-service/User/${data.userId}`, { email: data.email, userName: data.name, phoneNumber: data.phoneNumber }, { headers }))
      return response;
    }
    catch (error) {
      console.error("add user failed", error);
      return error as BaseResponseModel;
    };
  }

  async getAllCryptoUsers(): Promise<ListUserResponeModel | undefined> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);

      const response = await lastValueFrom(this.http.get<ListUserResponeModel>(`${this.baseUrl}/crypto-service/Group/GetAllUsers`, { headers }));
      console.log("get all crypto users succeed");
      return response;
    }
    catch (error) {
      console.error("get all userssssss failed", error);
      return undefined;
    }
  }
}
