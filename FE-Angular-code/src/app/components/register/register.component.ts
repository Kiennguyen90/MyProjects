import { Component, inject } from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, FormBuilder, Validators} from '@angular/forms';
import { HeaderComponent } from '../header/header.component';
import { AccountService } from '../../services/account.service';
import {ActivatedRoute} from '@angular/router';
import { Router } from '@angular/router';
import { RegisterModel } from '../../interfaces/register-model';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, HeaderComponent, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  route: ActivatedRoute = inject(ActivatedRoute);
  accountService = inject(AccountService);
  registerPayload : RegisterModel = {fullName : '', email: '' , password: '', confirmPassword: ''};
  isSucceed: boolean | undefined;
  isPressRegister: boolean = false;
  isRegister: boolean = true;


  registerForm = new FormGroup({
    userName: new FormControl(''),
    email: new FormControl(''),
    password: new FormControl(''),
    repeatPassword: new FormControl('')
  });
  
  constructor(private router: Router, private fb: FormBuilder) {
    this.registerForm = this.fb.group({
      userName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      repeatPassword: ['', [Validators.required]]
    });
  }

  async submitRegister()
  {
    this.isPressRegister = true;
    this.registerPayload.fullName = this.registerForm.value.userName ?? '';
    this.registerPayload.email = this.registerForm.value.email ?? '';
    this.registerPayload.password = this.registerForm.value.password ?? '';
    this.registerPayload.confirmPassword = this.registerForm.value.repeatPassword ?? '';
    if(this.registerPayload.password !== this.registerPayload.confirmPassword){
      this.registerForm.get('repeatPassword')?.setErrors({marching: true});
      return;
    }
    await this.accountService.Register(this.registerPayload)
    .then((response) => {
      this.isSucceed = response
    });
    if(this.isSucceed){
      this.router.navigate(['/']);
    }
    else{
      console.log("register component error")
    }
  }
}
