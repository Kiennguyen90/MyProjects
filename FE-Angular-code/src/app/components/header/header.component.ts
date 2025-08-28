import { Component, inject } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { AccountService } from '../../services/usermanagement/be-integration-services/account.service';
import { AuthService } from '../../services/usermanagement/fe-services/auth.service';
import { HeaderService } from '../../services/usermanagement/fe-services/header.service';
import { BaseUserInformationModel } from '../../interfaces/auth-model';
import {TranslatePipe, TranslateDirective, TranslateService} from '@ngx-translate/core';
import { UserService } from '../../services/usermanagement/be-integration-services/user.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterLink, NgIf, TranslatePipe, TranslateDirective],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})

export class HeaderComponent {
  userInformation: BaseUserInformationModel | undefined;
  isLogin: boolean = false;
  logoPath: string = '../../../assets/icons/mylogo.png';
  accountService = inject(AccountService);
  authService = inject(AuthService);
  userService = inject(UserService);
  avatarPath: string = '../../../assets/icons/mylogo.png';
  private translate = inject(TranslateService);

  useLanguage(language: string): void {
      this.translate.use(language);
  }
  constructor(private router: Router, private headerService : HeaderService) {
  }

  async ngOnInit(): Promise<void> {
    this.isLogin = await this.headerService.checkUserLogin();
    this.userInformation = this.accountService.getUserInformation();
    this.avatarPath = await this.userService.getAvatar();
  }
  
  async onLogout() {
    await this.accountService.SignOut().then((response) => {
       this.isLogin = !response
    });
    this.router.navigate(['/login']);
  }
}
