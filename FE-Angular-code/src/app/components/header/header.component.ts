import { Component, Input, inject } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { AccountService } from '../../services/usermanagement/be-integration-services/account.service';
import { AuthService } from '../../services/usermanagement/fe-services/auth.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterLink, NgIf],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})

export class HeaderComponent {
  @Input() userModel!: UserModel|undefined;
  @Input() isLogin!: boolean|undefined;
  @Input() isRegister!: boolean|undefined;
  
  imagePath: string = '../../../assets/icons/mylogo.png';

  accountService = inject(AccountService);
  authService = inject(AuthService);

  constructor(private router: Router) {
  }
  
  async onLogout() {
    await this.accountService.SignOut().then((response) => {
       this.isLogin = !response
    });
    this.router.navigate(['/login']);
  }
}
