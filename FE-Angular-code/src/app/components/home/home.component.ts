import { Component, inject } from '@angular/core';
import { NgIf, CommonModule, NgFor } from '@angular/common';
import { HeaderComponent } from '../header/header.component';
import { RouterLink, Router } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { ServiceModel } from "../../interfaces/service-model";
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { CommonService } from '../../services/common.service';

@Component({
  selector: 'app-home',
  standalone: true,  // In case you're building a standalone component.
  imports: [HeaderComponent, RouterLink, NgIf, NgFor, CommonModule],  // Added NgIf for conditional rendering
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']  // Corrected property name
})
export class HomeComponent {
  ListServices: ServiceModel[] | undefined;
  userModel: UserModel | undefined;
  isLogin: boolean = false;
  userService: UserService = inject(UserService);
  authService = inject(AuthService);
  commonService: CommonService = inject(CommonService);

  constructor(private router: Router) {
    this.onLoadUserInfo();
  }

  async onLoadUserInfo() {
    try {
      const userId = this.authService.getCurrentUserId();
      if (this.commonService.ListServices === undefined) {
        this.ListServices = await this.commonService.getAllService();
      }
      else {
        this.ListServices = this.commonService.ListServices;
      }
      if (userId !== null) {
        this.userModel = await this.userService.getUserById(userId);
        if (this.userModel != undefined) {
          this.isLogin = true;
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

  onDivClick() {
    if (this.userModel?.userRole === "admin") {
      this.router.navigate(['/cryptoadmin']);
    }
    else if (this.userModel?.userRole === "user") {
      this.router.navigate(['/cryptouser/' + this.userModel.id]);
    }
    else {
      this.router.navigate(['/login']);
    }
  }
}
