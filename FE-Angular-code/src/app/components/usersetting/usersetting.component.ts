import { Component, inject } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { BaseUserInformationModel } from '../../interfaces/auth-model';
import { AccountService } from '../../services/usermanagement/be-integration-services/account.service';
import { AuthService } from '../../services/usermanagement/fe-services/auth.service';
import { HeaderService } from '../../services/usermanagement/fe-services/header.service';
import { NgIf } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ImageDialogComponent } from '../image-dialog/image-dialog.component';
import { UserService } from '../../services/usermanagement/be-integration-services/user.service';

@Component({
  selector: 'app-usersetting',
  imports: [HeaderComponent, NgIf],
  templateUrl: './usersetting.component.html',
  styleUrl: './usersetting.component.css'
})
export class UsersettingComponent {
  userInformation: BaseUserInformationModel | undefined;
  isLogin: boolean = false;
  imagePath: string = '../../../assets/icons/mylogo.png';
  accountService = inject(AccountService);
  authService = inject(AuthService);
  headerService = inject(HeaderService);
  userService = inject(UserService);

  constructor(public dialog: MatDialog) {
  }

  async ngOnInit(): Promise<void> {
    this.isLogin = await this.authService.checkUserLogin();
    this.userInformation = this.accountService.getUserInformation();
    this.imagePath = await this.userService.getAvatar();
  }

  async selectedImg() : Promise<void> {
    const imgDialogRef = this.dialog.open(ImageDialogComponent, {
          width: '50%',
          data: { file: File, userId: this.userInformation?.userId }
        });

    imgDialogRef.afterClosed().subscribe(async (result) => {
      if (result) {
        await this.userService.uploadAvatar(result.file)
          .then((response) => {
            if (response.isSuccess) {
              console.log("User image updated successfully.");
              this.userService.getAvatar().then((imgUrl) => {
                this.imagePath = imgUrl;
              });
            } else {
              console.error("Failed to update user image.");
            }
          });
      }
    });
  }
}
