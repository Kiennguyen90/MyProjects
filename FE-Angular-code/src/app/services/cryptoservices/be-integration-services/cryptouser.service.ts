import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserInformationModel } from '../../../interfaces/crypto/user-model';
import { lastValueFrom } from 'rxjs';
import { AuthService } from '../../usermanagement/fe-services/auth.service';
import { environment } from '../../../../environments/environment';
import { ListCryptoTokensModel } from '../../../interfaces/crypto/tokens-model';
import { BaseResponseModel } from '../../../interfaces/base-respone-model';
import { TransactionTokenRequestModel, UserTokensModel } from '../../../interfaces/crypto/trading-token-model';

@Injectable({
  providedIn: 'root'
})
export class CryptouserService {
  private baseUrl = environment.apiUrl;
  private authService = inject(AuthService);

  constructor(private http: HttpClient) {

  }

  async getUserInformationServiceAsync(email: string): Promise<UserInformationModel | undefined> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);

      const response = await lastValueFrom(this.http.get<UserInformationModel>(`${this.baseUrl}/crypto-service/User/${email}`, { headers }));
      console.log("get crypto users succeed");
      return response;
    }
    catch (error) {
      console.error("get userssssss failed", error);
      return undefined;
    }
  }
  async getCryptoTokensServiceAsync(): Promise<ListCryptoTokensModel> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);

      const response = await lastValueFrom(this.http.get<ListCryptoTokensModel>(`${this.baseUrl}/crypto-service/crypto/tokens`, { headers }));
      console.log("get crypto tokens succeed");
      return response;
    }
    catch (error) {
      console.error("get crypto tokens failed", error);
      return { cryptoTokens: [], message: 'Failed to fetch tokens', isSuccess: false };
    }
  }

  async updateUserBalanceServiceAsync(userId: string, amount: number, isDeposit : boolean): Promise<BaseResponseModel> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const body = { userId, amount, isDeposit };
      const response = await lastValueFrom(this.http.post<BaseResponseModel>(`${this.baseUrl}/crypto-service/user/UpdateUserBalace`, body, { headers }));
      console.log("Deposit crypto token succeed");
      return response;
    } catch (error) {
      console.error("Deposit crypto token failed", error);
      return { isSuccess: false, message: 'Failed to update user balance' };
    }
  }

  async tokenExchangeServiceAsyn(tradingToken : TransactionTokenRequestModel): Promise<BaseResponseModel> {
    try {
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.post<BaseResponseModel>(`${this.baseUrl}/crypto-service/crypto/ExchangeToken`, tradingToken, { headers }));
      console.log("Update user token succeed");
      return response;
    } catch (error) {
      console.error("Update user token failed", error);
      return { isSuccess: false, message: 'Failed to update user token' };
    }
  }

  async getUserTokenByEmailServiceAsync(email: string): Promise<UserTokensModel| undefined> {
    try {
      if (!email) {
        console.error('Email is required to fetch user tokens');
        return { isSuccess: false, message: 'Email is required to fetch user tokens', tokens: [] };
      }
      const headers = new HttpHeaders()
        .set('X-Debug-Level', 'minimal')
        .set('Authorization', `Bearer ${this.authService.getAccessToken()}`);
      const response = await lastValueFrom(this.http.get<any>(`${this.baseUrl}/crypto-service/crypto/UserTokens/${email}`, { headers }));
      if (!response || !response.isSuccess) {
        console.error("Failed to fetch user tokens", response?.message);
        return { isSuccess: false, message: response?.message || 'Failed to fetch user tokens', tokens: [] };
      }
      console.log(response.message);
      return response as UserTokensModel;
    } catch (error) {
      console.error("Update user token failed", error);
      return { isSuccess: false, message: 'Failed to fetch user tokens', tokens: [] };
    }
  }
}
