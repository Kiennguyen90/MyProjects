import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { AccountService } from '../../services/account.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})

export class HeaderComponent {
  isLogin: boolean = true;
  @Input() userModel!: UserModel;

  accountService = inject(AccountService);
  authService = inject(AuthService);

  constructor(private router: Router) {
  }

  async onLogout() {
    await this.accountService.SignOut();
    this.router.navigate(['/login']);
  }
}
