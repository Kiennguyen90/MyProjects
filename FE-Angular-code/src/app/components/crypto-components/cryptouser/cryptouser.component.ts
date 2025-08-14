import { Component, inject, NgModule } from '@angular/core';
import { HeaderComponent } from '../../header/header.component';
import { AuthService } from '../../../services/usermanagement/fe-services/auth.service';
import { UserInformationModel } from '../../../interfaces/crypto/user-model';
import { CryptouserService } from '../../../services/cryptoservices/be-integration-services/cryptouser.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgFor, NgIf, DecimalPipe, DatePipe, NgClass } from '@angular/common';
import { ListCryptoTokensModel } from '../../../interfaces/crypto/tokens-model';
import { FormsModule } from '@angular/forms';
import { TransactionTokenRequestModel, UserTokensModel } from '../../../interfaces/crypto/trading-token-model';

@Component({
  selector: 'app-cryptouser',
  imports: [HeaderComponent, RouterLink, NgIf, NgFor, NgClass, FormsModule, DecimalPipe, DatePipe ],
  templateUrl: './cryptouser.component.html',
  styleUrl: './cryptouser.component.css',
})
export class CryptouserComponent {
  currentActionUserId: string | null = null;
  isGroupAdmin: boolean = false;
  email: string | null = null;
  cryptoTokenList: ListCryptoTokensModel | undefined;
  selectedUserInformation: UserInformationModel | undefined;
  authService = inject(AuthService);
  userBalanceAmount: number | undefined;
  selectedToken: string = 'BTC'; // Default selected option
  tokenAmount: number | undefined;
  amountVnd: number | undefined;
  amountUsdt: number | undefined;
  userTokens: UserTokensModel | undefined; // User's tokens
  transactionType: string = 'Buy'; // Default transaction type
  balanceChangeType: string = 'Deposit'; // Default balance change type
  totalPredictedValue: number = 0; // Initialize total predicted value

  constructor(private cryptouserService: CryptouserService, private route: ActivatedRoute) {
    this.currentActionUserId = this.authService.getCurrentUserId();
  }

  async ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.email = params.get('email');
    });
    await this.onLoadUserInfo();
  }

  async onLoadUserInfo() {
    try {
      if (this.currentActionUserId === null) {
        return;
      }
      if (this.email !== null) {
        this.selectedUserInformation = await this.cryptouserService.getUserInformationServiceAsync(this.email);
      }
      let adminId = this.selectedUserInformation?.groupAdminId;
      if (adminId !== undefined && adminId === this.currentActionUserId) {
        this.isGroupAdmin = true;
      }
      this.cryptoTokenList = await this.cryptouserService.getCryptoTokensServiceAsync();
      if (this.cryptoTokenList === undefined) {
        this.cryptoTokenList = { cryptoTokens: [], message: 'No tokens available', isSuccess: false };
      }
      console.log("Crypto tokens loaded:", this.cryptoTokenList);
      await this.onLoadUserTokensAsync();
      console.log("User information loaded:", this.selectedUserInformation);
    } catch (error) {
      console.error('Error loading user information:', error);
    }
  }

  async onLoadUserTokensAsync() {
    if (this.email) {
      console.log(`Loading tokens for user: ${this.email}`);
      try {
        this.userTokens = await this.cryptouserService.getUserTokenByEmailServiceAsync(this.email);
        if (this.userTokens && this.userTokens.tokens) {
          this.userTokens.tokens.forEach(token => {
            this.totalPredictedValue += token.totalValue;
            });
        } else {
          console.error('No tokens found for user:', this.email);
        }
      } catch (error) {
        console.error('Error loading user tokens:', error);
      }
    } else {
      console.error('No user information available');
    }
  }

  onTokenSelectionChange(event: any): void {
    this.selectedToken = event.target.value;
  }

  onTransactionSelectionChange(event: any): void {
    this.transactionType = event.target.value;
  }
  onBalanceSelectionChange(event: any): void {
    this.balanceChangeType = event.target.value;
  }

  async onUpdateUserBalance(type : string) {
    if (this.selectedUserInformation) {
      console.log(`onUpdateUserBalance clicked for user: ${this.selectedUserInformation.email}`);
      // Implement deposit logic here
      if (this.userBalanceAmount !== undefined && this.userBalanceAmount > 0) {
        console.log(`onUpdateUserBalance amount: ${this.userBalanceAmount}`);
        // Call the service to handle deposit logic
        debugger
        var response = await this.cryptouserService.updateUserBalanceServiceAsync(this.selectedUserInformation.userId, this.userBalanceAmount, type === 'Deposit');
        if (response.isSuccess) {
          console.log('onUpdateUserBalance successful:', response.message);
          this.userBalanceAmount = 0; // Reset deposit amount after successful deposit
          // Optionally, refresh user information or update UI
          await this.onLoadUserInfo();
        }
      } else {
        console.error('Invalid onUpdateUserBalance amount');
      }

    } else {
      console.error('No user information available for onUpdateUserBalance');
    }
  }

  async onTradeTokenAsync(isBuy : boolean) {
    if (this.selectedUserInformation) {
      console.log(`Trade token clicked for user: ${this.selectedUserInformation.email}`);
      if (this.tokenAmount !== undefined && this.amountVnd !== undefined && this.tokenAmount > 0 && this.amountVnd > 0) {
        console.log(`Trading token: ${this.selectedToken}, Amount: ${this.tokenAmount}, Total: ${this.amountVnd}`);
        let tokenId = this.cryptoTokenList?.cryptoTokens.find(token => token.symbol === this.selectedToken)?.id;
        if (!tokenId) {
          console.error('Selected token not found in crypto tokens list');
          return;
        }
        var tradingToken: TransactionTokenRequestModel = {
          userId: this.selectedUserInformation.userId,
          tokenId: tokenId,
          tokenAmount: this.tokenAmount,
          amountVnd: this.amountVnd,
          amountUsdt: this.amountUsdt || 0, // Use amountUsdt if available, otherwise default to 0
          isBuy: isBuy // 'buy' or 'sell'
        };
        var response = await this.cryptouserService.tokenExchangeServiceAsyn(tradingToken);
        if (response.isSuccess) {
          console.log('Trade token successful:', response.message);
          this.tokenAmount = 0; // Reset token amount after successful buy
          this.amountVnd = 0; // Reset amount after successful buy
          await this.onLoadUserInfo();
        }
        else {
          console.error('Trade token failed:', response.message);
        }
      } else {
        console.error('Invalid token amount or total amount');
      }
    } else {
      console.error('No user information available for trading token');
    }
  }

  onAmountVndChange() {
    if (this.amountVnd !== undefined && this.amountVnd > 0) {
      // Assuming a conversion rate of 1 USDT = 1 USD for simplicity
      this.amountUsdt = this.amountVnd / 26400; // Example conversion rate, adjust as needed
      this.amountUsdt = parseFloat(this.amountUsdt.toFixed(4)); // Round to 2 decimal places
    } else {
      this.amountUsdt = undefined; // Reset if amount is invalid
    }
  }

  onAmountUsdtChange() {
    if (this.amountUsdt !== undefined && this.amountUsdt > 0) {
      // Assuming a conversion rate of 1 USDT = 1 USD for simplicity
      this.amountVnd = this.amountUsdt * 26400; // Example conversion rate, adjust as needed
      this.amountVnd = parseFloat(this.amountVnd.toFixed(2)); // Round to 2 decimal places
    } else {
      this.amountVnd = undefined; // Reset if amount is invalid
    }
  }
}
