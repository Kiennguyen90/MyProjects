import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

export interface ImgData {
  imageUrl: string;
  userId: string;
}

@Component({
  selector: 'app-image-dialog',
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './image-dialog.component.html',
  styleUrl: './image-dialog.component.css'
})
export class ImageDialogComponent {
  constructor(
      public dialogRef: MatDialogRef<ImageDialogComponent>,
      @Inject(MAT_DIALOG_DATA) public data: ImgData
    ) { }
  
    onCancel(): void {
      this.dialogRef.close();
    }
  
    onUpdated(): void {
      this.dialogRef.close(this.data);
    }
}
