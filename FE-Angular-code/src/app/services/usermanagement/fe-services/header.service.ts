import { Injectable } from '@angular/core';
import { AuthService } from '../fe-services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class HeaderService {

  constructor(private authService : AuthService) { }

  async checkUserLogin(): Promise<boolean> {
    return await this.authService.checkUserLogin();
  }
}
