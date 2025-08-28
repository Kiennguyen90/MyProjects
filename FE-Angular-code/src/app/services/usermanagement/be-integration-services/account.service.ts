import { Injectable } from '@angular/core';
import { UserService } from './user.service';
import { AuthModel, BaseUserInformationModel } from '../../../interfaces/auth-model';
import { AuthService } from '../fe-services/auth.service';
import { inject } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { RegisterModel } from '../../../interfaces/register-model';
import { lastValueFrom } from 'rxjs';
import { BaseResponseModel } from '../../../interfaces/base-respone-model';
import e from 'express';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private baseUrl = environment.apiUrl;
  private accessToken = "";
  private refreshToken = "";
  userId = "";
  userInformation: BaseUserInformationModel | undefined;
  userService = inject(UserService);
  authService = inject(AuthService);
  isLogOutSucceed = false;
  isRegisterSucceed = false;

  constructor(private http: HttpClient) { }

  async Register(registerPayload: RegisterModel): Promise<boolean> {
    try {
      const response = await lastValueFrom(this.http.post<AuthModel>(`${this.baseUrl}/user-management/Account/register`, registerPayload));
      if (response.error === "") {
        this.userInformation = response.userInformation;
        this.accessToken = response.accessToken;
        this.refreshToken = response.refreshToken;
        this.isRegisterSucceed = true;

        this.authService.setAccessToken(this.accessToken);
        this.authService.setRefreshToken(this.refreshToken);
        this.authService.setUserInformation(JSON.stringify(this.userInformation));
        console.log('Account Register Succeed');
        return true;
      }
      else {
        this.isRegisterSucceed = false;
        console.log('Account Register Failed');
      }
    } catch (error) {
      console.error('Register error:', error);
    }
    console.log('Register Failed');
    return false;
  }

  async Login(email: string, password: string): Promise<AuthModel | undefined> {
    try {
      const response = await lastValueFrom(this.http.post<AuthModel>(`${this.baseUrl}/user-management/Account/login`, { Email: email, Password: password }));
      if (!response) {
        console.error("Login failed");
        return undefined;
      }

      if (response.error === "") {
        this.userInformation = response.userInformation;
        this.accessToken = response.accessToken;
        this.refreshToken = response.refreshToken;

        this.authService.setAccessToken(this.accessToken);
        this.authService.setRefreshToken(this.refreshToken);
        this.authService.setUserInformation(JSON.stringify(this.userInformation));
        return response;
      }
      else {
        return response;
      }
    } 
    catch (error) {
      return undefined;
    }
  }

  async LoginByGoogle(token: string): Promise<AuthModel | undefined> {
    try {
      const data = { token: token };
      const response = await lastValueFrom(this.http.post<AuthModel>(`${this.baseUrl}/user-management/Account/auth/google`, data));
      if (!response) {
        console.error("Login by Google failed");
        return undefined;
      }

      if (response.error === "") {
        this.userInformation = response.userInformation;
        this.accessToken = response.accessToken;
        this.refreshToken = response.refreshToken;

        this.authService.setAccessToken(this.accessToken);
        this.authService.setRefreshToken(this.refreshToken);
        this.authService.setUserInformation(JSON.stringify(this.userInformation));
        return response;
      }
      else {
        return response
      }
    } 
    catch (error) {
      return undefined
    }
  }


  async SignOut(): Promise<boolean> {
    try {
      var response = await lastValueFrom(this.http.post<boolean>(`${this.baseUrl}/user-management/Account/logout`, {}));
      if (response === true) {
        this.isLogOutSucceed = true;
      }
      else {
        this.isLogOutSucceed = false;
      }
      console.log("Logout response: ", response);
    } catch (error) {
      console.error('Logout error:', error);
      this.isLogOutSucceed = false;
    }

    if (this.isLogOutSucceed) {
      this.authService.removeAccessToken();
      this.authService.removeRefreshToken();
      this.authService.removeUserInformation();
      console.log('Account SignOut Succeed');
      return true;
    }
    return false;
  }

  getUserInformation() {
    const userInformation = this.authService.getUserInformation();
    if (userInformation !== null) {
      return JSON.parse(userInformation);
    }
    return undefined;
  }
}