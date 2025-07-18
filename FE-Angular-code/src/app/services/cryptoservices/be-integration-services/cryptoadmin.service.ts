
import { lastValueFrom } from 'rxjs';
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../usermanagement/fe-services/auth.service';
import { environment } from '../../../../environments/environment';
import { ListUserResponeModel } from '../../../interfaces/crypto/cryptouser-model';
import { AddUserResponseModel } from '../../../interfaces/crypto/adduser-respone-model';
import e from 'express';
import { EditUserData } from '../../../components/crypto-components/edituser-dialog/edituser-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class CryptoadminService {

  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);
  constructor(private http: HttpClient) { }

  async addUser(email: string, userName: string, phoneNumber: string): Promise<AddUserResponseModel> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.post<AddUserResponseModel>(`${this.baseUrl}/crypto-service/User`, { email: email, userName: userName, phoneNumber: phoneNumber }, { headers }))
      return response;
    }
    catch (error) {
      console.error("add user failed", error);
      return error as AddUserResponseModel;
    };
  }

  async updateUser(data : EditUserData) : Promise<AddUserResponseModel> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.put<AddUserResponseModel>(`${this.baseUrl}/crypto-service/User/${data.userId}`, { email: data.email, userName: data.name, phoneNumber: data.phoneNumber }, { headers }))
      return response;
    }
    catch (error) {
      console.error("add user failed", error);
      return error as AddUserResponseModel;
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
