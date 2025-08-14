import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

export interface EditUserData {
  name: string;
  email: string;
  phoneNumber: string;
  userId: string;
}

@Component({
  selector: 'app-edituser-dialog',
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './edit-user.component.html',
  styleUrl: './edit-user.component.css'
})
export class EdituserDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<EdituserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: EditUserData
  ) { }

  onCancel(): void {
    this.dialogRef.close();
  }

  onUpdated(): void {
    this.dialogRef.close(this.data);
  }
}
