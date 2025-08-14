import { inject, Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import e from 'express';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private authService = inject(AuthService);
  private userData: any;
  constructor() { }
  
  setUserData(data: any) {
    this.userData = data;
  }
  getUserData(): any {
    if (!this.userData) {
      const userInformation = this.authService.getUserInformation();
      if (userInformation) {
        try {
          this.userData = JSON.parse(userInformation);
        } catch (error) {
          console.error('Error parsing user information:', error);
          this.userData = null;
        }
      }
      else {
        this.userData = null;
      }
      return this.userData;
    }
    return this.userData;
  }
}
