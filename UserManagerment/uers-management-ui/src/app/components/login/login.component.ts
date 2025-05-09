declare const google: any;
import { Component, inject, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { ActivatedRoute } from '@angular/router';
import { UserModel } from '../../interfaces/user-model';
import { UserService } from '../../services/user.service';
import { RouterLink, Router } from '@angular/router';
import { CommonModule, NgIf, isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgIf, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['../register/register.component.css']  // Corrected property name: styleUrls
})
export class LoginComponent implements OnInit {
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

  constructor( private router: Router, private fb: FormBuilder, @Inject(PLATFORM_ID) private platformId: Object) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadGoogleApi().then(() => {
        google.accounts.id.initialize({
          client_id: "333413287567-mmh71anmggqo2dbg4nqopk38n8s1g9j7.apps.googleusercontent.com",
          callback: this.handleGoogleCallback.bind(this),
        });
  
        google.accounts.id.renderButton(
          document.getElementById("g_id_signin"),
          { theme: "outline", size: "large" }
        );
  
        // google.accounts.id.prompt();
      }).catch(error => {
        console.error("Google API failed to load", error);
      });
    }
  }

  handleGoogleCallback(response: any): void {
    // Implement your logic with response
    console.log('Google login response', response);
    // Process the JWT token, etc.
  }

  private loadGoogleApi(): Promise<void> {
    return new Promise((resolve, reject) => {
      const scriptId = 'google-api-script';
      if (document.getElementById(scriptId)) {
        return resolve();
      }
      
      const script = document.createElement('script');
      script.id = scriptId;
      script.src = 'https://accounts.google.com/gsi/client';
      script.async = true;
      script.defer = true;
      script.onload = () => resolve();
      script.onerror = (error) => reject(error);
      document.body.appendChild(script);
    });
  }

  async submitLogin() {
    this.isPressLogin = true;
    if (this.loginForm.invalid) {
      return;
    }
    
    const { email, password } = this.loginForm.value;
    try {
      const response = await this.accountService.Login(email ?? '', password ?? '');
      this.userId = response;
      if (this.userId !== "error") {
        this.router.navigate(['/']);
      } else {
        console.error("Login error");
      }
    } catch (error) {
      console.error("Submission error:", error);
    }
  }
}