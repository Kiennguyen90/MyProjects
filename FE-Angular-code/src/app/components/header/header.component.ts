import { Component, inject } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { AccountService } from '../../services/usermanagement/be-integration-services/account.service';
import { HeaderService } from '../../services/usermanagement/fe-services/header.service';
import { BaseUserInformationModel } from '../../interfaces/auth-model';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import { UserService } from '../../services/usermanagement/be-integration-services/user.service';
import { LocalStorageService } from '../../services/common/local-storage.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterLink, NgIf, TranslatePipe],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})

export class HeaderComponent {
  userInformation: BaseUserInformationModel | undefined;
  isLogin: boolean = true;
  logoPath: string = '../../../assets/icons/mylogo.png';
  accountService = inject(AccountService);
  userService = inject(UserService);
  avatarPath: string = '../../../assets/icons/mylogo.png';
  private translate = inject(TranslateService);

  constructor(private router: Router, private headerService : HeaderService, private localStorage : LocalStorageService) {
    const savedLanguage = this.localStorage.getItem('language');
    if (savedLanguage) {
      this.translate.use(savedLanguage);
    } else {
      this.translate.use('en'); // Default language
    }
  }

  async ngOnInit(): Promise<void> {
    this.isLogin = await this.headerService.checkUserLogin();
    this.userInformation = this.accountService.getUserInformation();
    debugger
    this.avatarPath = await this.userService.getAvatar(this.userInformation?.userId);
  }
  
  async onLogout() {
    await this.accountService.SignOut().then((response) => {
       this.isLogin = !response
    });
    this.router.navigate(['/login']);
  }

  useLanguage(language: string): void {
      this.translate.use(language);
      this.localStorage.setItem('language', language);
  }
}
