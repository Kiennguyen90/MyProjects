
import { lastValueFrom } from 'rxjs';
import { UserModel } from '../../../interfaces/user-model';
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../usermanagement/fe-services/auth.service';
import { environment } from '../../../../environments/environment';
import { CryptoUserModel } from '../../../interfaces/crypto/cryptouser-model';
import { AddUserResponseModel } from '../../../interfaces/crypto/adduser-respone-model';
import e from 'express';

@Injectable({
  providedIn: 'root'
})
export class CryptoadminService {

  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);
  constructor(private http: HttpClient) { }

  // async getUserById(id: string): Promise<UserModel | undefined> {
  //   try {
  //     const headers = new HttpHeaders()
  //       .set('X-Debug-Level', 'minimal')
  //       .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);

  //     const response = await lastValueFrom(this.http.get<UserModel>(`${this.baseUrl}/User/${id}`, { headers }));
  //     console.log("get user information succeed");
  //   } catch (error) {
  //     console.error("get user information failed", error);
  //     return undefined;
  //   }
  // }

  // async updateUser(user: UserModel): Promise<UserModel | undefined> {
  //   try {
  //     const headers = new HttpHeaders()
  //       .set('X-Debug-Level', 'minimal')
  //       .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
  //     const response = await lastValueFrom(this.http.put<UserModel>(`${this.baseUrl}/User/${user.id}`, user, { headers }))
  //   }
  //   catch (error) {
  //     console.error("update user information failed", error);
  //     return undefined;
  //   };
  // }

  async addUser(email: string, userName: string) : Promise<AddUserResponseModel> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.post<AddUserResponseModel>(`${this.baseUrl}/crypto-service/User/AddUser`, { email: email, userName: userName }, { headers }))
      return response;
    }
    catch (error) {
      console.error("add user failed", error);
      return error as AddUserResponseModel;
    };
  }

  async getAllCryptoUsers(): Promise<CryptoUserModel[] | undefined> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);

      const response = await lastValueFrom(this.http.get<CryptoUserModel[]>(`${this.baseUrl}/crypto-service/Group/GetAllUsers`,{ headers }));
      console.log("get all crypto users succeed");
      return response;
    }
    catch (error) {
      console.error("get all userssssss failed", error);
      return undefined;
    }
  }
}
