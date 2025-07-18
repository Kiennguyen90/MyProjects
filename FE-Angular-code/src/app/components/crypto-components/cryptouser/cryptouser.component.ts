import { Component, inject } from '@angular/core';
import { HeaderComponent } from '../../header/header.component';
import { AuthService } from '../../../services/usermanagement/fe-services/auth.service';
import { CryptoUserModel } from '../../../interfaces/crypto/cryptouser-model';
import { CryptouserService } from '../../../services/cryptoservices/be-integration-services/cryptouser.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-cryptouser',
  imports: [HeaderComponent, RouterLink, NgIf],
  templateUrl: './cryptouser.component.html',
  styleUrl: './cryptouser.component.css'
})
export class CryptouserComponent {
  currentActionUserId: string | null = null;
  isGroupAdmin: boolean = false;
  email: string | null = null;
  cryptouserInformation: CryptoUserModel | undefined; // Replace 'any' with the appropriate type if available
  authService = inject(AuthService);

  constructor(private cryptouserService: CryptouserService, private route: ActivatedRoute) {
    this.currentActionUserId = this.authService.getCurrentUserId();
  }

  async ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.email = params.get('email');
    });
    await this.onLoadUserInfo();
  }

  async onLoadUserInfo() {
    try {
      if (this.currentActionUserId === null) {
        return;
      }
      if (this.email !== null) {
        this.cryptouserInformation = await this.cryptouserService.getUserInformation(this.email);
      }
      let adminId = this.cryptouserInformation?.groupAdminId;
      if (adminId !== undefined && adminId === this.currentActionUserId) {
          this.isGroupAdmin = true;
        }
    } catch (error) {
      console.error('Error loading user information:', error);
    }
  }
}
