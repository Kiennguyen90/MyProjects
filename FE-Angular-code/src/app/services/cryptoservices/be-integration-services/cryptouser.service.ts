import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CryptoUserModel } from '../../../interfaces/crypto/cryptouser-model';
import { lastValueFrom } from 'rxjs';
import { AuthService } from '../../usermanagement/fe-services/auth.service';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CryptouserService {
  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);

  constructor(private http: HttpClient) {

  }

  async getUserInformation(email: string): Promise<CryptoUserModel | undefined> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);

      const response = await lastValueFrom(this.http.get<CryptoUserModel>(`${this.baseUrl}/crypto-service/User/${email}`, { headers }));
      console.log("get crypto users succeed");
      return response;
    }
    catch (error) {
      console.error("get userssssss failed", error);
      return undefined;
    }
  }
}
