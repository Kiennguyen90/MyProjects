import { Injectable } from '@angular/core';
import e from 'express';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  
  localStorageAvailable: boolean = typeof window !== 'undefined' && typeof window.localStorage !== 'undefined';
  localStorage: Storage | null = this.localStorageAvailable ? window.localStorage : null;

  constructor() {
    if (!this.localStorageAvailable) {
      console.warn('Local Storage is not available.');
      return;
    }
    this.localStorage = window.localStorage;
  }

  getItem(key: string): string | null {
    if(this.localStorage === null) {
      console.error('Local Storage is not available:' + key);
      return null;
    }
    else {
      return this.localStorage.getItem(key);
    }
  }

  setItem(key: string, value: string): void {
    if (this.localStorage) {
      this.localStorage.setItem(key, value);
    } else {
      console.error('Local Storage is not available.');
    }
  }

  removeItem(key: string): void {
    if (this.localStorage) {
      this.localStorage.removeItem(key);
    } else {
      debugger
      console.error('Local Storage is not available.');
    }
  }

  clear(): void {
    if (this.localStorage) {
      this.localStorage.clear();
    } else {
      console.error('Local Storage is not available.');
    }
  }
}
