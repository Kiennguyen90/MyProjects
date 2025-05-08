import { Component, inject } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { RouterLink, Router } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,  // In case you're building a standalone component.
  imports: [HeaderComponent, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']  // Corrected property name
})
export class HomeComponent {
  userModel: UserModel | undefined;
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
        this.isLogin = this.userModel !== undefined;
      } else {
        this.isLogin = false;
      }
    } catch (error) {
      console.error('Error getting user:', error);
      this.isLogin = false;
    }
  }
}
