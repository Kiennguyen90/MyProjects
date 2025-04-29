import { Injectable } from '@angular/core';
import { UserService } from './user.service';
import { AuthModel } from '../interfaces/auth-model';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RegisterModel } from '../interfaces/register-model';

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

  async Register(registerPayload: RegisterModel): Promise<boolean | undefined> {
    this.http.post<AuthModel>(this.baseUrl + '/Account/register', registerPayload)
      .subscribe(response => {
        this.userId = response.userId,
        this.accessToken = response.accessToken,
        this.refreshToken = response.refreshToken,
        this.isRegisterSucceed = true
      });
    if (this.isRegisterSucceed) {
      this.authService.setAccessToken(this.accessToken);
      this.authService.setRefreshToken(this.refreshToken);
      this.authService.setUserId(this.userId);
      console.log('Account Register Succeed');
      return true;
    }
    else {
      console.log('Register Failed');
      return false;
    }
  }

  async Login(email: string, password: string): Promise <string | ""> {
    const body = { Email: email, Password: password };
    this.http.post<AuthModel>(this.baseUrl + '/Account/login', body)
      .subscribe(response => {
        this.userId = response.userId,
        this.accessToken = response.accessToken,
        this.refreshToken = response.refreshToken,
        this.isLoginSucceed = true
      });
    if (this.isLoginSucceed) {
      this.authService.setAccessToken(this.accessToken);
      this.authService.setRefreshToken(this.refreshToken);
      this.authService.setUserId(this.userId);
      console.log('Account Login Succeed');
      return this.userId;
    }
    else {
      return "error";
    }
  }

  async SignOut(): Promise<boolean | undefined> {
    this.http.post<any>(this.baseUrl + '/Account/logout', {})
      .subscribe(response => {
        this.isLogOutSucceed = true
      });
    if (this.isLogOutSucceed) {
      this.authService.removeAccessToken();
      this.authService.removeRefreshToken();
      this.authService.removeUserId()
      console.log('Account SignOut Succeed:');
      return true;
    }
    else {
      return false;
    }
  }
}
