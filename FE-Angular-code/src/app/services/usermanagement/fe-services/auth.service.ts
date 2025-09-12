import { inject, Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { BaseUserInformationModel } from '../../../interfaces/auth-model';
import { lastValueFrom } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { LocalStorageService } from '../../common/local-storage.service';
import e from 'express';


export interface RefreshAccesstoken {
  errorMessage: string;
  accessToken: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private decodedAccessToken: any;
  private currentTime = Math.floor(Date.now() / 1000);
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  private localStorage = inject(LocalStorageService)
  constructor() { }

  getCurrentUserId(): string | null {
    const userInformation = this.getUserInformation();
    if (userInformation !== null) {
      const user: BaseUserInformationModel = JSON.parse(userInformation);
      return user.userId;
    }
    return null;
  }

  setUserInformation(userInformation: string) {
    this.localStorage.setItem('userInformation', userInformation);
  }

  getUserInformation(): string | null {
    return this.localStorage.getItem('userInformation');
  }

  removeUserInformation() {
    this.localStorage.removeItem('userInformation');
  }

  setAccessToken(token: string) {
    localStorage.setItem('access_token', token);
  }

  getAccessToken(): string | null {
    return this.localStorage.getItem('access_token');
  }

  removeAccessToken() {
    this.localStorage.removeItem('access_token');
  }

  setRefreshToken(token: string) {
    this.localStorage.setItem('refresh_token', token);
  }

  getRefreshToken(): string | null {
    return this.localStorage.getItem('refresh_token');
  }

  removeRefreshToken() {
    localStorage.removeItem('refresh_token');
  }

  decodeAccessToken(): object | null {
    const token = this.getAccessToken();
    if (token) {
      try {
        return jwtDecode<object>(token);
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }

  async isAuthentication(): Promise<boolean> {
    if (this.localStorage.localStorageAvailable) {
      let accessToken = this.getAccessToken();
      let refreshToken = this.getRefreshToken();
      if (refreshToken === null) {
        return false;
      }
      else {
        if (accessToken === null) {
          let isrefreshtokenSuccess = await this.refreshAccessToken(refreshToken);
          if (!isrefreshtokenSuccess) {
            this.removeRefreshToken();
            this.removeUserInformation();
            return false;
          }
          else {
            return isrefreshtokenSuccess;
          }
        }
        else {
          this.decodedAccessToken = jwtDecode(accessToken);
          if (this.decodedAccessToken && this.decodedAccessToken.exp && this.decodedAccessToken.exp > this.currentTime) {
            return true;
          }
          else {
            console.warn('Access token is expired');
            this.removeAccessToken();
            if (refreshToken !== null) {
              let isrefreshtokenSuccess = await this.refreshAccessToken(refreshToken);
              if (!isrefreshtokenSuccess) {
                this.removeRefreshToken();
                this.removeUserInformation();
                return false;
              }
              else {
                return isrefreshtokenSuccess; // Access token refreshed successfully
              }
            }
          }
        }
      }
    }
    return false;
  }

  async refreshAccessToken(refreshtoken: string): Promise<boolean> {
    try {
      let userInformation = this.localStorage.getItem('userInformation');
      let accessToken = this.getAccessToken();

      if (userInformation !== null) {
        const user: BaseUserInformationModel = JSON.parse(userInformation);
        var email = user.email;
        const response = await lastValueFrom(this.http.post<RefreshAccesstoken>(`${this.baseUrl}/user-management/Account/LoginByRefreshtoken`, { Email: email, refreshtoken: refreshtoken }));
        if (response.errorMessage === '') {
          accessToken = response.accessToken;
          this.setAccessToken(accessToken);
          console.log('Refresh token login Succeed');
          return true; // Refresh token login succeeded
        }
        else {
          this.removeRefreshToken();
          this.removeUserInformation();
          console.log('Refresh token login failed:', response.errorMessage);
          return false; // Refresh token login failed
        }
      }
      else {
        console.warn('User information not found, cannot refresh token');
        return false; // User information not found, cannot refresh token
      }
    }
    catch (error) {
      console.error('Refresh token login Error', error);
      return false; // Error occurred during refresh token login
    }
  }
}