import { Component, inject } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { HeaderComponent } from '../../header/header.component';
import { Router } from '@angular/router';
import { UserModel } from '../../../interfaces/user-model';
import { AuthService } from '../../../services/usermanagement/fe-services/auth.service';
import { CryptoadminService } from '../../../services/cryptoservices/be-integration-services/cryptoadmin.service';

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserDialogComponent, AddUserData } from '../dialogs/add-user/add-user.component';
import { MatButtonModule } from '@angular/material/button';
import { UserInformationModel } from '../../../interfaces/crypto/user-model';
import { EditUserData, EdituserDialogComponent } from '../dialogs/edit-user/edit-user.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataService } from '../../../services/usermanagement/fe-services/data.service';


@Component({
  selector: 'app-adminview',
  imports: [HeaderComponent, CommonModule, MatDialogModule, MatButtonModule, DecimalPipe],
  templateUrl: './cryptoadmin.component.html',
  styleUrl: './cryptoadmin.component.css'
})
export class CryptoadminComponent {
  userModel: UserModel | undefined;
  isLogin: boolean = false;
  isPermission: boolean = false;
  cryptoUsers: UserInformationModel[] | undefined;
  currentUserId: string | null = null;
  authService = inject(AuthService);  
  currentActionUser: any | undefined;

  constructor(private router: Router, private dataService: DataService, public dialog: MatDialog, private cryptoadminService: CryptoadminService,private snackBar: MatSnackBar) {
    this.currentUserId = this.authService.getCurrentUserId();
    this.currentActionUser = this.dataService.getUserData();
  }

  async ngOnInit() {
    if (this.currentUserId === null) {
      return;
    }
    else {
      await this.onLoadPage();
      await this.onLoadCryptoUsers();
    }
  }

  async onLoadPage() {
    try {
      this.isLogin = await this.authService.checkUserLogin();
      if (!this.isLogin) {
        this.router.navigate(['/login']);
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
      var getAllUserRespone = await this.cryptoadminService.getAllCryptoUsers();
      if (getAllUserRespone !== undefined) {
        if (getAllUserRespone.message !== "No Permission") {
          this.isPermission = true;
          this.cryptoUsers = getAllUserRespone.listUser;
          return;
        }
      }
      else {
        console.log('No crypto users found');
      }
    } catch (error) {
      console.error('Error loading crypto users:', error);
    }
  }

  async editUserDialog(fullName: string, email: string, phoneNumber: string, userId: string): Promise<void> {
    const editDialogRef = this.dialog.open(EdituserDialogComponent, {
      width: '50%',
      data: { name: fullName, email: email, phoneNumber: phoneNumber, userId: userId }
    });
    // Pre-fill the dialog with existing user data
    editDialogRef.afterClosed().subscribe((result: EditUserData) => {
      if (result) {
        this.cryptoadminService.updateUser(result).then(respone => {
          if (respone.isSuccess) {
            console.log(respone.message);
            this.onLoadCryptoUsers(); // Refresh user info after adding
            this.showActionMessage(respone.message);
          } else {
            console.error(respone.message);
            this.showActionMessage(respone.message);
          }
        }).catch(error => {
          console.error('Error updating user:', error);
          this.showActionMessage('Error updating user: ' + error.message);
        });
      }
    });
  }

  async addUserDialog(): Promise<void> {
    const dialogRef = this.dialog.open(UserDialogComponent, {
      width: '50%',
      data: { name: '', email: '', phoneNumber: '' }
    });
    dialogRef.afterClosed().subscribe((result: AddUserData) => {
      if (result) {
        this.cryptoadminService.addUser(result.email, result.name, result.phoneNumber).then(respone => {
          if (respone.isSuccess) {
            console.log(respone.message);
            this.onLoadCryptoUsers();
            this.showActionMessage(respone.message);
          } else {
            this.showActionMessage(respone.message);
          }
        }).catch(error => {
          console.error('Error adding user:', error);
          this.showActionMessage('Error adding user: ' + error.message);
        });
      }
    });
  }

  selectedUser(email: string): void {
    this.router.navigate(['/cryptoservice/user/' + email]);
  }

  showActionMessage(message : string = ''): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000, // thời gian để snackbar tự động đóng
      horizontalPosition: 'right', // vị trí ngang
      verticalPosition: 'top', // vị trí dọc
      panelClass: ['red-snackbar']
    });
  }
}


