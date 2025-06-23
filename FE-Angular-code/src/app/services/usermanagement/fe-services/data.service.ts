import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private userData: any;
  constructor() { }
  
  setUserData(data: any) {
    this.userData = data;
  }
  getUserData(): any {
    return this.userData;
  }
}
