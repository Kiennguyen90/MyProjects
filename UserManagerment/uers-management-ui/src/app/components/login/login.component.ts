import { Component, inject } from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, FormBuilder, Validators} from '@angular/forms';
import { HeaderComponent } from '../header/header.component';
import { AccountService } from '../../services/account.service';
import {ActivatedRoute} from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, HeaderComponent, NgIf],
  templateUrl: './login.component.html',
  styleUrl: '../register/register.component.css'
})
export class LoginComponent {

  route: ActivatedRoute = inject(ActivatedRoute);
  accountService = inject(AccountService);
  userService = inject(UserService);
  userModel: UserModel | undefined;
  userId = "";
  isPressLogin: boolean = false;

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  });

  constructor(private router: Router, private fb: FormBuilder){
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  async submitLogin() {
    this.isPressLogin = true;
    if (this.loginForm.invalid) {
      return;
    }
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
