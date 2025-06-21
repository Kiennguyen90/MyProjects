import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../header/header.component';
import { Router } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { UserService } from '../../services/usermanagement/user.service';
import { AuthService } from '../../services/usermanagement/auth.service';

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserDialogComponent, UserData } from '../user-dialog/user-dialog.component';
import { MatButtonModule } from '@angular/material/button';


@Component({
  selector: 'app-adminview',
  imports: [HeaderComponent, CommonModule, MatDialogModule, MatButtonModule],
  templateUrl: './cryptoadmin.component.html',
  styleUrl: './cryptoadmin.component.css'
})
export class CryptoadminComponent {
  userModel: UserModel | undefined;
  isLogin: boolean = false;
  authService = inject(AuthService);

  constructor(private router: Router, private userService: UserService, public dialog: MatDialog) {
    this.onLoadUserInfo();
  }

  async onLoadUserInfo() {
    try {
      const userId = this.authService.getCurrentUserId();
      if (userId !== null) {
        this.userModel = await this.userService.getUserById(userId);
        if (this.userModel != undefined) {
          this.isLogin = true;
        }
        else {
          this.isLogin = false;
        }
      }
    }
    catch (error) {
      console.error('Error getting user:', error);
      this.isLogin = false;
    }
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(UserDialogComponent, {
      width: '250px',
      data: { name: '', email: '' }
    });

    dialogRef.afterClosed().subscribe((result: UserData) => {
      if (result) {
        console.log('User Data:', result);
        // You can handle the result here, like saving it to a server
      }
    });
  }
}
