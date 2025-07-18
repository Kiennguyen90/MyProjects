import { inject, Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { UserInformation } from '../../../interfaces/auth-model';
import { lastValueFrom } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';


export interface RefreshAccesstoken {
  errorMessage: string;
  accessToken: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private accessToken: string | null = null;
  private refreshToken: string | null = null;
  private userInformation: string | null = null;
  private decodedAccessToken: any;
  private currentTime = Math.floor(Date.now() / 1000);
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  getCurrentUserId(): string | null {
    const userInformation = this.getUserInformation();
    if (userInformation !== null) {
      const user: UserInformation = JSON.parse(userInformation);
      return user.userId;
    }
    return null;
  }

  setUserInformation(userInformation: string) {
    this.userInformation = userInformation;
    localStorage.setItem('userInformation', userInformation);
  }

  getUserInformation(): string | null {
    if (!this.userInformation && typeof window !== 'undefined' && window.localStorage) {
      this.userInformation = localStorage.getItem('userInformation');
    }
    return this.userInformation;
  }

  removeUserInformation() {
    this.userInformation = null;
    localStorage.removeItem('userInformation');
  }

  setAccessToken(token: string) {
    this.accessToken = token;
    localStorage.setItem('access_token', token);
  }

  getAccessToken(): string | null {
    if (!this.accessToken && typeof window !== 'undefined' && window.localStorage) {
      this.accessToken = localStorage.getItem('access_token');
    }
    return this.accessToken;
  }

  removeAccessToken() {
    this.accessToken = null;
    localStorage.removeItem('access_token');
  }

  setRefreshToken(token: string) {
    this.refreshToken = token;
    localStorage.setItem('refresh_token', token);
  }

  getRefreshToken(): string | null {
    if (!this.refreshToken && typeof window !== 'undefined' && window.localStorage) {
      this.refreshToken = localStorage.getItem('refresh_token');
    }
    return this.refreshToken;
  }

  removeRefreshToken() {
    this.refreshToken = null;
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

  async checkUserLogin(): Promise<boolean> {
    this.accessToken = this.getAccessToken();
    this.refreshToken = this.getRefreshToken();
    if (this.accessToken !== null) {
      this.decodedAccessToken = jwtDecode(this.accessToken);
      if (this.decodedAccessToken && this.decodedAccessToken.exp && this.decodedAccessToken.exp < this.currentTime) {
        console.warn('Access token is expired');
        this.removeAccessToken();
        if (this.refreshToken !== null) {
          let isrefreshtokenSuccess = await this.refreshAccessToken(this.refreshToken);
          if (!isrefreshtokenSuccess) {
            this.removeRefreshToken();
            this.removeUserInformation();
            return false; // Refresh token failed, user is not logged in
          }
          else {
            return isrefreshtokenSuccess; // Access token refreshed successfully
          }
        }
      }
      else {
        console.log('Access token is valid');
        return true; // Access token is valid, user is logged in
      }
    }
    else {
      if (this.refreshToken !== null) {
        let isrefreshtokenSuccess = await this.refreshAccessToken(this.refreshToken);
        if (!isrefreshtokenSuccess) {
          this.removeRefreshToken();
          this.removeUserInformation();
          return false; // Refresh token failed, user is not logged in
        }
        else {
          return isrefreshtokenSuccess; // Access token refreshed successfully
        }
      }
    }

    return this.accessToken !== null;
  }

  async refreshAccessToken(refreshtoken: string): Promise<boolean> {
    try {
      let userInformation = localStorage.getItem('userInformation');
      if (userInformation !== null) {
        const user: UserInformation = JSON.parse(userInformation);
        var email = user.email;
        const response = await lastValueFrom(this.http.post<RefreshAccesstoken>(`${this.baseUrl}/user-management/Account/LoginByRefreshtoken`, { Email: email, refreshtoken: refreshtoken }));
        if (response.errorMessage === '') {
          this.accessToken = response.accessToken;
          this.setAccessToken(this.accessToken);
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