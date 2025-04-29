import { Component, Input } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { RouterLink, RouterOutlet } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';

@Component({
  selector: 'app-home',
  imports: [HeaderComponent, RouterOutlet, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  userModel: UserModel | undefined;
}
