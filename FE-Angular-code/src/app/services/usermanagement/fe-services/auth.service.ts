import { Injectable } from '@angular/core';
import {jwtDecode} from 'jwt-decode';
import { UserInformation } from '../../../interfaces/auth-model';
import { get } from 'http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private accessToken: string | null = null;
  private refreshToken: string | null = null;
  private userInformation: string | null = null;
  private decodedAccessToken: any;
  private decodedRefreshToken: any;
  private currentTime = Math.floor(Date.now() / 1000);

  getCurrentUserId(): string | null {
    const userInformation = this.getUserInformation();
    if (userInformation !== null) {
      const user : UserInformation = JSON.parse(userInformation);
      return user.userId;
    }
    return null;
  }

  setUserInformation(userInformation: string) {
    this.userInformation = userInformation;
    localStorage.setItem('userInformation', userInformation);
  }

  getUserInformation(): string | null {
    if(this.accessToken !== null ) {
      this.decodedAccessToken = jwtDecode(this.accessToken);
      if (this.decodedAccessToken && this.decodedAccessToken.exp && this.decodedAccessToken.exp < this.currentTime) {
        console.warn('Access token is expired');
        this.removeAccessToken();
        this.decodedRefreshToken = jwtDecode(this.getRefreshToken() || '');
        if (this.decodedRefreshToken && this.decodedRefreshToken.exp) {
          if(this.decodedRefreshToken.exp < this.currentTime) {
            console.warn('Refresh token is also expired');
            this.removeRefreshToken();
            this.removeUserInformation();
            return null;
          }
          else {
            console.warn('Refresh token is valid, but access token is expired');
            // Here you would typically call a method to refresh the access token
            // For example: this.refreshAccessToken();
          }
          return null;
        }
        return null;
      }
    }
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
    if (!this.refreshToken) {
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
}