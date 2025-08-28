import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BlobservicesService {

  constructor() { }
  
  async imgBase64ToBlob(base64: string, mimeType: string): Promise<Blob> {
    // Decode base64 to binary string
    const binary = atob(base64);

    // Create an array of unsigned 8-bit integers
    const array = new Uint8Array(binary.length);

    // Loop through binary string to fill the array
    for (let i = 0; i < binary.length; i++) {
      array[i] = binary.charCodeAt(i);
    }

    // Create Blob
    return new Blob([array], { type: mimeType });
  }
}
