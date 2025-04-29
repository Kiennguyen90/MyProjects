import { Component, inject } from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import { HeaderComponent } from '../header/header.component';
import { AccountService } from '../../services/account.service';
import {ActivatedRoute} from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, HeaderComponent],
  templateUrl: './login.component.html',
  styleUrl: '../register/register.component.css'
})
export class LoginComponent {

  route: ActivatedRoute = inject(ActivatedRoute);
  accountService = inject(AccountService);
  userService = inject(UserService);
  userModel: UserModel | undefined;
  userId = "";

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  });

  constructor(private router: Router){}

  async submitLogin() {
    await this.accountService.Login (
      this.loginForm.value.email ?? '',
      this.loginForm.value.password ?? ''
    ).then((respone) => {
      this.userId = respone;
    });
    if(this.userId !== "error"){
      this.router.navigate(['/']);
    }
    else
    {
      console.log("error")
    }
  }
}
