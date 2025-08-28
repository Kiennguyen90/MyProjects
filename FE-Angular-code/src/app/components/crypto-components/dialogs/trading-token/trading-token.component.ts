import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TransactionTokenRequestModel } from '../../../../interfaces/crypto/trading-token-model';

@Component({
  selector: 'app-trading-token',
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './trading-token.component.html',
  styleUrl: './trading-token.component.css'
})
export class TradingTokenComponent {
  constructor(
        public dialogRef: MatDialogRef<TradingTokenComponent>,
        @Inject(MAT_DIALOG_DATA) public data: TransactionTokenRequestModel
      ) { }
    
      onCancel(): void {
        this.dialogRef.close();
      }
    
      onConfirm(): void {
        this.dialogRef.close(this.data);
      }
}
