import { Injectable } from '@angular/core';
import { AuthService } from '../fe-services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class HeaderService {

  constructor(private authService : AuthService) { }

  getUserInformation() {
    const userInformation = this.authService.getUserInformation();
    if (userInformation !== null) {
      return JSON.parse(userInformation);
    }
    return undefined;
  }
}
