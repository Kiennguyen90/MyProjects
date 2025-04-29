import { Component, Inject, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet, RouterModule } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { UserService } from '../../services/user.service';
import { AccountService } from '../../services/account.service';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  isLogin: boolean = true;
  userModel: UserModel | undefined;

  accountService = inject(AccountService);
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
        debugger
      }
    }
    catch (error) {
      console.error('Error get user:', error);
    }
  }

  async onLogout() {
    await this.accountService.SignOut();
    this.router.navigate(['/login']);
  }
}
