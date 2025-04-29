import { Injectable } from '@angular/core';
import {jwtDecode} from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private accessToken: string | null = null;
  private refreshToken: string | null = null;
  private userId: string | null = null;

  setUserId(userId: string) {
    this.userId = userId;
    localStorage.setItem('user_Id', userId);
  }

  getUserId(): string | null {
    if (!this.userId) {
      this.userId = localStorage.getItem('user_Id');
    }
    return this.userId;
  }

  removeUserId() {
    this.userId = null;
    localStorage.removeItem('user_Id');
  }

  setAccessToken(token: string) {
    this.accessToken = token;
    localStorage.setItem('access_token', token);
  }

  getAccessToken(): string | null {
    if (!this.accessToken) {
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
      return localStorage.getItem('refresh_token');
    }
    return this.refreshToken;
  }

  removeRefreshToken() {
    this.refreshToken = null;
    localStorage.removeItem('refresh_token');
  }

  decodeAccessToken(): any {
    const token = this.getAccessToken();
    if (token) {
      return jwtDecode(token);
    }
    return null;
  }
}
