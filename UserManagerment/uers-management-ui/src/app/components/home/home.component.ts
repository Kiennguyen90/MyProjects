import { Component, inject } from '@angular/core';
import { NgIf, CommonModule } from '@angular/common';
import { HeaderComponent } from '../header/header.component';
import { RouterLink, Router } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { ServicesModel } from '../../interfaces/services-model';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,  // In case you're building a standalone component.
  imports: [HeaderComponent, RouterLink, NgIf, CommonModule],  // Added NgIf for conditional rendering
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']  // Corrected property name
})
export class HomeComponent {
  userModel: UserModel | undefined;
  servicesModel: ServicesModel = { isCrypto: false, isShopHouse: false };
  isLogin: boolean = false;
  authService = inject(AuthService);

  constructor(private router: Router, private userService: UserService) {
    this.onLoadUserInfo();
  }

  async onLoadUserInfo() {
    try {
      const userId = this.authService.getUserId();
      if (userId !== null) {
        this.userModel = await this.userService.getUserById(userId);
        if (this.userModel != undefined && this.userModel.services.length > 0) {
          if (this.userModel.services.includes("7FF6451C-7D2E-4568-B6D2-D84E27E18319")) {
            this.servicesModel.isCrypto = true;
          }
          if (this.userModel.services.includes("B11CE3B0-3074-421C-A601-B7BF9252C78C")) {
            this.servicesModel.isShopHouse = true;
          }
          this.isLogin = this.userModel !== undefined;
        }
        else {
          this.isLogin = false;
        }
      }
    }
    catch (error) {
      console.error('Error getting user:', error);
      this.isLogin = false;
    }
  }
}
