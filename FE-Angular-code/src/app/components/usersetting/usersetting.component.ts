import { Component, inject } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { UserInformation } from '../../interfaces/auth-model';
import { AccountService } from '../../services/usermanagement/be-integration-services/account.service';
import { AuthService } from '../../services/usermanagement/fe-services/auth.service';
import { HeaderService } from '../../services/usermanagement/fe-services/header.service';
import { NgIf } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ImageDialogComponent } from '../image-dialog/image-dialog.component';

@Component({
  selector: 'app-usersetting',
  imports: [HeaderComponent, NgIf],
  templateUrl: './usersetting.component.html',
  styleUrl: './usersetting.component.css'
})
export class UsersettingComponent {
  userInformation: UserInformation | undefined;
  isLogin: boolean = false;
  imagePath: string = '../../../assets/icons/mylogo.png';
  accountService = inject(AccountService);
  authService = inject(AuthService);
  headerService = inject(HeaderService);

  constructor(public dialog: MatDialog) {
  }

  async ngOnInit(): Promise<void> {
    this.isLogin = await this.authService.checkUserLogin();
    this.userInformation = this.accountService.getUserInformation();
  }

  async selectedImg() : Promise<void> {
    const imgDialogRef = this.dialog.open(ImageDialogComponent, {
          width: '50%',
          data: { imageUrl: "", userId: this.userInformation?.userId }
        });

    imgDialogRef.afterClosed().subscribe(async (result) => {
      if (result) {
        // await this.accountService.UpdateUserImg(result.imageUrl, result.userId)
        //   .then((response) => {
        //     if (response) {
        //       this.userInformation!.imageUrl = result.imageUrl;
        //       this.headerService.setUserInformation(this.userInformation!);
        //     } else {
        //       console.error("Failed to update user image.");
        //     }
        //   });
      }
    });
  }
}
