import { Injectable, inject } from '@angular/core';
import { UserModel } from '../../../interfaces/user-model';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../fe-services/auth.service';
import { lastValueFrom } from 'rxjs';
import { BaseResponseModel } from '../../../interfaces/base-respone-model';
import { BlobservicesService } from '../../common/blob.service';
import { LocalStorageService } from '../../common/local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);
  private blobService = inject(BlobservicesService);
  userComonInformation: UserModel = { id: '', email: '', fullName: '', phoneNumber: '', avatar: '', userRole: '', services: [] };
  getUserSucceed: boolean | undefined;

  constructor(private http: HttpClient, private localStorage: LocalStorageService) {
  }

  async getUserById(id: string): Promise<UserModel | undefined> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.get<UserModel>(`${this.baseUrl}/user-management/User/${id}`, { headers }));
      this.userComonInformation = response;
      this.getUserSucceed = true;
      console.log("get user information succeed");
      return this.userComonInformation;
    } catch (error) {
      console.error("get user information failed", error);
      this.getUserSucceed = false;
      return undefined;
    }
  }

  async updateUser(user: UserModel): Promise<boolean> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.put<UserModel>(`${this.baseUrl}/user-management/User`, user, { headers }));
      this.userComonInformation = response;
      console.log("update user information succeed");
      return true;
    } catch (error) {
      console.error("update user information failed", error);
      return false;
    }
  }

  async uploadAvatar(file: File): Promise<BaseResponseModel> {
    try {
      const formData = new FormData();
      formData.append('avatar', file);

      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);

      // Adjust the URL to match your .NET Core 9 API endpoint
      const response = await lastValueFrom(
        this.http.post<BaseResponseModel>(
          `${this.baseUrl}/user-management/User/avatar`,
          formData
          , { headers } // Uncomment if you use headers
        )
      );
      this.localStorage.removeItem('avatar'+this.userComonInformation.id); // Clear cached avatar
      return response;
    } catch (error) {
      return { isSuccess: false, message: 'Failed to upload avatar' };
    }
  }

  async getAvatar(userId : string | undefined): Promise<string> {
    try {
      let avatarCaching = this.localStorage.getItem('avatar' + userId);
      if (avatarCaching) {
        const blob = await this.blobService.imgBase64ToBlob(avatarCaching, 'image/jpeg');
        const imgsrc = URL.createObjectURL(blob);
        return imgsrc;
      }
      let accessToken = this.authService.getAccessToken();
      if (accessToken === null) {
        return "";
      }
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${accessToken}`);

      const response = await lastValueFrom(
        this.http.get<any>(`${this.baseUrl}/user-management/User/avatar`, { headers })
      );
      if (response) {
        this.localStorage.setItem('avatar'+this.userComonInformation.id, response.fileContents);
        const blob = await this.blobService.imgBase64ToBlob(response.fileContents, 'image/jpeg');
        const imgsrc = URL.createObjectURL(blob);
        return imgsrc;
      }
      return "";
    } catch (error) {
      console.error('Failed to load avatar', error);
      return "error";
    }
  }
}
