import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';


export interface UpdateUserBalanceData {
  amount: number;
  isDeposit: boolean;
}

@Component({
  selector: 'app-update-user-balance',
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './update-user-balance.component.html',
  styleUrl: './update-user-balance.component.css'
})
export class UpdateUserBalanceComponent {
  constructor(
      public dialogRef: MatDialogRef<UpdateUserBalanceComponent>,
      @Inject(MAT_DIALOG_DATA) public data: UpdateUserBalanceData
    ) { }
  
    onCancel(): void {
      this.dialogRef.close();
    }
  
    onConfirm(): void {
      this.dialogRef.close(this.data);
    }
}
