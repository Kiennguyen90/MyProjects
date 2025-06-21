import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DefaultRoles {
    readonly admin = 
        {
            id: '42CD4109-6174-4FE0-A912-5AA0C1410A6A',
            name: 'admin',
        };
    readonly user = 
        {
            id: '717EA88B-196C-4003-93D0-A4D153B3E131',
            name: 'user',
        };
    readonly member = 
        {
            id: '6137218D-893E-478F-A5E0-6E7319E6E332',
            name: 'member',
        };
    readonly groupadmin = 
        {
            id: '86650F5A-E379-41EB-807A-BC750E9020F2',
            name: 'groupadmin',
        };
}