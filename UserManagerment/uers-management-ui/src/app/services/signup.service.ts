import { Injectable } from '@angular/core';
import { RegisterModel } from '../interfaces/register-model';

@Injectable({
  providedIn: 'root'
})
export class SignupService {

  // async submitRegister(register :RegisterModel): Promise<boolean | undefined>
  // {
  //   const account = await this.getAccountByEmail(email);
  //   if( account != undefined && account.password === password){
  //     await this.userService.setUserModel(email);
  //     return true;
  //   } 
  //   else
  //   {
  //     return false;
  //   }
  // }
  constructor() { }
}
