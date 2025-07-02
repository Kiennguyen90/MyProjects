import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../../header/header.component';
import { Router } from '@angular/router';
import { UserModel } from '../../../interfaces/user-model';
import { UserService } from '../../../services/usermanagement/be-integration-services/user.service';
import { AuthService } from '../../../services/usermanagement/fe-services/auth.service';
import { CryptoadminService } from '../../../services/cryptoservices/be-integration-services/cryptoadmin.service';

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserDialogComponent, AddUserData } from '../user-dialog/user-dialog.component';
import { MatButtonModule } from '@angular/material/button';
import { CryptoUserModel } from '../../../interfaces/crypto/cryptouser-model';
import { EdituserDialogComponent } from '../edituser-dialog/edituser-dialog.component';


@Component({
  selector: 'app-adminview',
  imports: [HeaderComponent, CommonModule, MatDialogModule, MatButtonModule],
  templateUrl: './cryptoadmin.component.html',
  styleUrl: './cryptoadmin.component.css'
})
export class CryptoadminComponent {
  userModel: UserModel | undefined;
  isLogin: boolean = false;
  cryptoUsers: CryptoUserModel[] | undefined;
  currentUserId: string | null = null;
  authService = inject(AuthService);

  constructor(private router: Router, private userService: UserService, public dialog: MatDialog, private cryptoadminService: CryptoadminService) {
        this.currentUserId = this.authService.getCurrentUserId();
  }

  async ngOnInit() {
    if (this.currentUserId === null) {
      return;
    }
    else 
    {
      await this.onLoadUserInfo();
      await this.onLoadCryptoUsers();
    }
  }

  async onLoadUserInfo() {
    try {
      const userId = this.currentUserId;
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

  async onLoadCryptoUsers() {
    try {
      if (this.cryptoUsers === undefined) {
        this.cryptoUsers = [];
      }
      this.cryptoUsers = await this.cryptoadminService.getAllCryptoUsers();
    } catch (error) {
      console.error('Error loading crypto users:', error);
    }
  }

  async editUser(userId : string): Promise<void> {
    const editDialogRef = this.dialog.open(EdituserDialogComponent, {
      width: '500px',
      data: { name: '', email: '', phoneNumber: '', userId: userId }
    });
    // Pre-fill the dialog with existing user data
    editDialogRef.afterClosed().subscribe((result: AddUserData) => {
      if (result) {
        // this.cryptoadminService.updateUser(result).then(success => {
        //   if (success) {
        //     console.log('User updated successfully');
        //     this.onLoadCryptoUsers(); // Refresh user list after update
        //   } else {
        //     console.error('Failed to update user');
        //   }
        // }).catch(error => {
        //   console.error('Error updating user:', error);
        // });
      }
    });
  }

   addUserDialog(): void {
    const dialogRef = this.dialog.open(UserDialogComponent, {
      width: '50%',
      data: { name: '', email: '' }
    });

    dialogRef.afterClosed().subscribe((result: AddUserData) => {
      if (result) {
        this.cryptoadminService.addUser(result.email, result.name).then(respone => {
          if (respone.isSuccess) {
            console.log(respone.message);
            // this.onLoadCryptoUsers(); // Refresh user info after adding
          } else {
            console.error(respone.message);
          }
        }).catch(error => {
          console.error('Error adding user:', error);
        });
      }
    });
  }
}
