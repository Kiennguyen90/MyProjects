import { Component, inject } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { UserModel } from '../../interfaces/user-model';
import { ActivatedRoute, Router } from '@angular/router';
import { DataService } from '../../services/usermanagement/fe-services/data.service';
import { ServiceModel } from '../../interfaces/service-model';
import { CommonService } from '../../services/usermanagement/be-integration-services/common.service';
import { AuthService } from '../../services/usermanagement/fe-services/auth.service';
import { UserService } from '../../services/usermanagement/be-integration-services/user.service';
import { NgClass, NgFor } from '@angular/common';

@Component({
  selector: 'app-registerservice',
  imports: [HeaderComponent, NgClass, NgFor],
  templateUrl: './registerservice.component.html',
  styleUrl: './registerservice.component.css',
  standalone: true
})
export class RegisterserviceComponent {
  commonService: CommonService = inject(CommonService);
  userService = inject(UserService);
  userModel: UserModel | undefined;
  isLogin: boolean = true;
  serviceId: string | undefined;
  selectedService: ServiceModel | undefined;
  selectedTypeId: number = 0;

  constructor(private router: Router, private route: ActivatedRoute, private dataService: DataService, private authService: AuthService) {
    this.isLogin = true;
  }

  ngOnInit() {
    const userId = this.authService.getCurrentUserId();
    this.serviceId = this.route.snapshot.paramMap.get('serviceid') || undefined;
    if (userId !== null && this.serviceId !== undefined) {
      this.onLoadUserInfo(userId);
      this.onLoadService(this.serviceId || '');
    }
    else {
      this.isLogin = false;
      this.router.navigate(['/login']);
    }
  }

  async onLoadUserInfo(userId: string) {
    try {
      this.userModel = await this.userService.getUserById(userId);
      if (this.userModel !== undefined) {
        this.isLogin = true;
        this.dataService.setUserData(this.userModel);
      } else {
        this.isLogin = false;
        this.router.navigate(['/login']);
      }
    } catch (error) {
      console.error('Error getting user:', error);
      this.isLogin = false;
    }
  }

  async onLoadService(serviceId: string) {
    this.selectedService = await this.commonService.getServiceById(serviceId);
  }

  async onRegisterService() {
    if (this.selectedService === undefined || this.userModel === undefined) {
      return;
    }
    await this.commonService.registerService(this.selectedService.id, this.selectedTypeId).then((response) => {
      if (response) {
        this.router.navigate(['/']);
      } else {
        console.error('Failed to register service');
      }
    }).catch((error) => {
      console.error('Error registering service:', error);
    });
  }

  onPlanSelected(serviceSelected: number) {
    this.selectedTypeId = serviceSelected;
    console.log("Selected plan:", this.selectedTypeId);
  }
}
