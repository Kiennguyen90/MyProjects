import { Component, inject } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { AccountService } from '../../services/usermanagement/be-integration-services/account.service';
import { AuthService } from '../../services/usermanagement/fe-services/auth.service';
import { HeaderService } from '../../services/usermanagement/fe-services/header.service';
import { UserInformation } from '../../interfaces/auth-model';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterLink, NgIf],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})

export class HeaderComponent {
  userInformation: UserInformation | undefined;
  isLogin: boolean = false;
  imagePath: string = '../../../assets/icons/mylogo.png';
  accountService = inject(AccountService);
  authService = inject(AuthService);
  

  constructor(private router: Router, private headerService : HeaderService) {
  }

  async ngOnInit(): Promise<void> {
    this.isLogin = await this.headerService.checkUserLogin();
    this.userInformation = this.accountService.getUserInformation();
  }
  
  async onLogout() {
    await this.accountService.SignOut().then((response) => {
       this.isLogin = !response
    });
    this.router.navigate(['/login']);
  }
}
