import { Injectable } from '@angular/core';
import { UserService } from './user.service';
import { AuthModel } from '../interfaces/auth-model';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { RegisterModel } from '../interfaces/register-model';
import { lastValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private baseUrl = environment.apiUrl;
  private accessToken = "";
  private refreshToken = "";
  userId = "";
  userService = inject(UserService);
  authService = inject(AuthService);
  isLoginSucceed = false;
  isLogOutSucceed = false;
  isRegisterSucceed = false;

  constructor(private http: HttpClient) { }

  async Register(registerPayload: RegisterModel): Promise<boolean> {
    try {
      const response = await lastValueFrom(this.http.post<AuthModel>(`${this.baseUrl}/Account/register`, registerPayload));
      if (response) {
        this.userId = response.userId;
        this.accessToken = response.accessToken;
        this.refreshToken = response.refreshToken;
        this.isRegisterSucceed = true;

        this.authService.setAccessToken(this.accessToken);
        this.authService.setRefreshToken(this.refreshToken);
        this.authService.setUserId(this.userId);
        console.log('Account Register Succeed');
        return true;
      }
    } catch (error) {
      console.error('Register error:', error);
    }
    console.log('Register Failed');
    return false;
  }

  async Login(email: string, password: string): Promise<string | ""> {
    try {
      const response = await lastValueFrom(this.http.post<AuthModel>(`${this.baseUrl}/Account/login`, { Email: email, Password: password }));
      if (response) {
        this.userId = response.userId;
        this.accessToken = response.accessToken;
        this.refreshToken = response.refreshToken;
        this.isLoginSucceed = true;

        this.authService.setAccessToken(this.accessToken);
        this.authService.setRefreshToken(this.refreshToken);
        this.authService.setUserId(this.userId);
        console.log('Account Login Succeed');
        return this.userId;
      }
    } catch (error) {
      console.error('Login error:', error);
    }
    return "error";
  }

  async SignOut() : Promise <boolean> {
    try {
      var response = await lastValueFrom (this.http.post<boolean>(`${this.baseUrl}/Account/logout`, {}));
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
      this.authService.removeUserId();
      console.log('Account SignOut Succeed');
      return true;
    }
    return false;
  }
}