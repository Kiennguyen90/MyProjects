import { Component } from '@angular/core';

@Component({
  selector: 'app-cryptouser',
  imports: [],
  templateUrl: './cryptouser.component.html',
  styleUrl: './cryptouser.component.css'
})
export class CryptouserComponent {
  // Define any properties or methods needed for the component here
  userId: string | null = null;

  constructor() {
    // You can initialize properties or call methods here if needed
  }

  ngOnInit() {
    // This lifecycle hook is called after the component is initialized
    // You can perform any initialization logic here
  }
}
