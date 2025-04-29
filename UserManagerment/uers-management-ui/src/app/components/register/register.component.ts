import { Component, inject } from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import { HeaderComponent } from '../header/header.component';
import { AccountService } from '../../services/account.service';
import {ActivatedRoute} from '@angular/router';
import { Router } from '@angular/router';
import { RegisterModel } from '../../interfaces/register-model';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, HeaderComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  route: ActivatedRoute = inject(ActivatedRoute);
  accountService = inject(AccountService);
  registerPayload : RegisterModel = {fullName : '', email: '' , password: '', confirmPassword: ''};
  isSucceed: boolean | undefined;


  registerForm = new FormGroup({
    userName: new FormControl(''),
    email: new FormControl(''),
    password: new FormControl(''),
    repeatPassword: new FormControl('')
  });
  
  constructor(private router: Router) {}

  async submitRegister(){
    this.registerPayload.email = this.registerForm.value.email ?? '';
    this.registerPayload.fullName = this.registerForm.value.userName ?? '';
    this.registerPayload.password = this.registerForm.value.password ?? '';
    this.registerPayload.confirmPassword = this.registerForm.value.repeatPassword ?? '';

    await this.accountService.Register(this.registerPayload)
    .then((response) => {this.isSucceed = response});
    if(this.isSucceed){
      this.router.navigate(['/']);
    }
    else{
      console.log("register error")
    }
  }
}
