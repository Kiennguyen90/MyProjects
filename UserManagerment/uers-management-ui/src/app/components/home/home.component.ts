import { Component, inject } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { RouterLink, RouterOutlet, Router } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  imports: [HeaderComponent, RouterOutlet, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  userModel: UserModel | undefined;
  isLogin: boolean = true;
  authService = inject(AuthService);

  constructor(private router: Router, private userService: UserService) {
    this.onLoadUserInfo();
  }

  async onLoadUserInfo() {
    try {
      if (this.authService.getUserId() !== null) {
        let userId: string = this.authService.getUserId() ?? "";
        await this.userService.getUserById(userId).then((response) => {
          this.userModel = response
        });
        if (this.userModel !== undefined) {
          this.isLogin = true;
        } else {
          this.isLogin = false;
        }
      }
    }
    catch (error) {
      console.error('Error get user:', error);
    }
  }
}
