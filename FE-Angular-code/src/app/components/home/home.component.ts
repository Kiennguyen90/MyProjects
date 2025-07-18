import { Component, inject } from '@angular/core';
import { NgIf, CommonModule, NgFor } from '@angular/common';
import { HeaderComponent } from '../header/header.component';
import { Router } from '@angular/router';
import { UserModel, userServiceModel } from '../../interfaces/user-model';
import { ServiceModel } from "../../interfaces/service-model";
import { UserService } from '../../services/usermanagement/be-integration-services/user.service';
import { AuthService } from '../../services/usermanagement/fe-services/auth.service';
import { CommonService } from '../../services/usermanagement/be-integration-services/common.service';
import { DataService } from '../../services/usermanagement/fe-services/data.service';
import { DefaultServices } from '../../../assets/staticfiles/default-services';
import { DefaultRoles } from '../../../assets/staticfiles/default-roles';

@Component({
  selector: 'app-home',
  standalone: true,  // In case you're building a standalone component.
  imports: [HeaderComponent, NgIf, NgFor, CommonModule],  // Added NgIf for conditional rendering
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']  // Corrected property name
})
export class HomeComponent {
  ListServices: ServiceModel[] | undefined;
  userModel: UserModel | undefined;
  isLogin: boolean = false;
  userService: UserService = inject(UserService);
  authService = inject(AuthService);
  commonService: CommonService = inject(CommonService);
  isCryptoService: boolean = false;
  isShoppingService: boolean = false;
  defaultServices: DefaultServices = inject(DefaultServices);
  DefaultRoles: DefaultRoles = inject(DefaultRoles);
  currentActionUserId: string | null = null;

  constructor(private router: Router, private dataService: DataService) {
  }

  async ngOnInit() {
    await this.onLoadPage();
    this.isLogin = await this.authService.checkUserLogin();
    this.currentActionUserId = this.authService.getCurrentUserId();
    if (this.currentActionUserId === null) {
      return;
    }
    this.onLoadUserInfo();
  }

  async onLoadPage() {
    if (this.commonService.ListServices === undefined) {
        this.ListServices = await this.commonService.getAllService();
      }
      else {
        this.ListServices = this.commonService.ListServices;
      }
  }
  
  async onLoadUserInfo() {
    try {
      if (this.currentActionUserId !== null && this.isLogin) {
        this.userModel = await this.userService.getUserById(this.currentActionUserId);
        if (this.userModel != undefined) {
          this.checkService();
          this.dataService.setUserData(this.userModel);
        }
      }
    }
    catch (error) {
      console.error('Error getting user:', error);
    }
  }

  onServiceClick(isActive: boolean, serviceId: string) {
    if (this.userModel === undefined) {
      this.router.navigate(['/login']);
      return;
    }
    if (this.userModel.services === undefined || this.userModel.services === null || this.userModel.services.length === 0 || !isActive) {
      this.router.navigate(['/registerservice/' + serviceId]);
      return;
    }
    else 
    {
      if (serviceId === this.defaultServices.crypto.id) {
        if (this.userModel.services.some(s => s.roleId == this.DefaultRoles.groupadmin.id)) {
          this.router.navigate(['/cryptoadmin']);
        }
        else {
          this.router.navigate(['/cryptouser/'+ this.userModel.email]);
        }
      }
      else if (serviceId === this.defaultServices.shophouse.id) {
        this.router.navigate(['/shoppingservice']);
      }
    }
  }

  checkService() {
    if (this.userModel === undefined || this.userModel.services === undefined || this.userModel.services === null) {
      return;
    }
    else {
      var userServices = this.userModel.services as userServiceModel[];
      const serviceIds = userServices.map(s => s.serviceId);
      console.log("serviceIds: ", serviceIds[0]);
      if (serviceIds.includes(this.defaultServices.crypto.id)) {
        this.isCryptoService = true;
      }
      if (serviceIds.includes(this.defaultServices.shophouse.id)) {
        this.isShoppingService = true;
      }
    }
  }
}
