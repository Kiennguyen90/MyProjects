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

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UpdateUserBalanceData, UpdateUserBalanceComponent } from '../dialogs/update-user-balance/update-user-balance.component';
import { TradingTokenComponent } from '../dialogs/trading-token/trading-token.component';

@Component({
  selector: 'app-cryptouser',
  imports: [HeaderComponent, RouterLink, NgIf, NgFor, NgClass, FormsModule, DecimalPipe, DatePipe],
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

  constructor(private cryptouserService: CryptouserService, private route: ActivatedRoute, public dialog: MatDialog, private snackBar: MatSnackBar) {
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

  async onUpdateUserBalance(isDeposit: boolean) {
    if (this.selectedUserInformation) {
      console.log(`onUpdateUserBalance clicked for user: ${this.selectedUserInformation.email}`);
      // Implement deposit logic here
      if (this.userBalanceAmount !== undefined && this.userBalanceAmount > 0) {
        var response = await this.cryptouserService.updateUserBalanceServiceAsync(this.selectedUserInformation.userId, this.userBalanceAmount, isDeposit);
        if (response.isSuccess) {
          console.log('onUpdateUserBalance successful:', response.message);
          this.userBalanceAmount = 0; // Reset deposit amount after successful deposit
          // Optionally, refresh user information or update UI
          await this.onLoadUserInfo();
          this.showActionMessage(response.message);
        }
      } else {
        this.showActionMessage('Invalid onUpdateUserBalance amount');
      }

    } else {
      this.showActionMessage('No user information available for onUpdateUserBalance');
    }
  }

  async onTradeTokenAsync(tradingToken: TransactionTokenRequestModel) {
    if (this.selectedUserInformation) {
      console.log(`Trade token clicked for user: ${this.selectedUserInformation.email}`);
      var response = await this.cryptouserService.tokenExchangeServiceAsyn(tradingToken);
      if (response.isSuccess) {
        console.log('Trade token successful:', response.message);
        this.tokenAmount = 0;
        this.amountVnd = 0;
        this.amountUsdt = 0;
        this.showActionMessage(response.message);
        await this.onLoadUserInfo();
      }
      else {
        console.error('Trade token failed:', response.message);
      }
    }
    else {
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
      this.amountVnd = this.amountUsdt * 26400;
      this.amountVnd = parseFloat(this.amountVnd.toFixed(2));
    } else {
      this.amountVnd = undefined;
    }
  }

  onUpdateUserBalanceDialog(isDeposit: boolean): void {
    if (this.selectedUserInformation) {
      const dialogRef = this.dialog.open(UpdateUserBalanceComponent, {
        width: '50%',
        data: { amount: this.userBalanceAmount || 0, isDeposit: isDeposit }
      });
      dialogRef.afterClosed().subscribe((result: UpdateUserBalanceData | undefined) => {
        if (result) {
          console.log('Dialog closed with result:', result);
          this.userBalanceAmount = result.amount;
          this.onUpdateUserBalance(result.isDeposit);
        } else {
          console.log('Dialog closed without result');
        }
      });
    }
    else {
      console.error('No user information available for update balance dialog');
    }
  }

  onTradingTokenDialog(isBuy: boolean): void {
    if (this.selectedUserInformation) {
      let tokenId = this.cryptoTokenList?.cryptoTokens.find(token => token.symbol === this.selectedToken)?.id;
      if (!tokenId) {
        console.error('Selected token not found in crypto tokens list');
        return;
      }
      let tokenName = this.cryptoTokenList?.cryptoTokens.find(token => token.symbol === this.selectedToken)?.name;
      if (!tokenName) {
        console.error('Selected token not found in crypto tokens list');
        return;
      }
      var tradingToken: TransactionTokenRequestModel | undefined = undefined;
      if (this.tokenAmount !== undefined && this.amountVnd !== undefined && this.tokenAmount > 0 && this.amountVnd > 0) {
        tradingToken = {
          userId: this.selectedUserInformation.userId,
          tokenId: tokenId,
          tokenName: tokenName,
          tokenAmount: this.tokenAmount,
          amountVnd: this.amountVnd,
          amountUsdt: this.amountUsdt || 0,
          isBuy: isBuy
        };
      }
      else {
        tradingToken = {
          userId: this.selectedUserInformation.userId,
          tokenId: tokenId,
          tokenName: tokenName,
          tokenAmount: 0,
          amountVnd: 0,
          amountUsdt: 0,
          isBuy: isBuy
        };
      }

      const dialogRef = this.dialog.open(TradingTokenComponent, {
        width: '50%',
        data: tradingToken
      });
      dialogRef.afterClosed().subscribe((result: TransactionTokenRequestModel | undefined) => {
        if (result) {
          console.log('Dialog closed with result:', result);
          this.onTradeTokenAsync(result);
        } else {
          console.log('Dialog closed without result');
        }
      });
    }
    else {
      console.error('No user information available for update balance dialog');
    }
  }

  showActionMessage(message: string = ''): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000, // thời gian để snackbar tự động đóng
      horizontalPosition: 'right', // vị trí ngang
      verticalPosition: 'top', // vị trí dọc
      panelClass: ['red-snackbar']
    });
  }
}
