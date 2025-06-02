import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../header/header.component';
import { RouterLink, Router } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-adminview',
  imports: [HeaderComponent, CommonModule],
  templateUrl: './cryptoadmin.component.html',
  styleUrl: './cryptoadmin.component.css'
})
export class CryptoadminComponent {
  userModel: UserModel | undefined;
  isLogin: boolean = false;
  authService = inject(AuthService);

  constructor(private router: Router, private userService: UserService) {
    this.onLoadUserInfo();
  }

  async onLoadUserInfo() {
    try {
      const userId = this.authService.getCurrentUserId();
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
}
