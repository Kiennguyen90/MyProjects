import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./components/home/home.component').then(c => c.HomeComponent),
        title: 'Home page',
    },
    {
        path: 'register',
        loadComponent: () => import('./components/register/register.component').then(c => c.RegisterComponent),
        title: 'Register',
    },
    {
        path: 'login',
        loadComponent: () => import('./components/login/login.component').then(c => c.LoginComponent),
        title: 'Login',
    },
    {
        path: 'cryptoadmin',
        loadComponent: () => import('./components/crypto-components/cryptoadmin/cryptoadmin.component').then(c => c.CryptoadminComponent),
        title: 'CryptoAdmin',
    },
    {
        path: 'cryptouser/:userid',
        loadComponent: () => import('./components/crypto-components/cryptouser/cryptouser.component').then(c => c.CryptouserComponent),
        title: 'CryptoAdmin',
    },
    {
        path: 'registerservice/:serviceid',
        loadComponent: () => import('./components/registerservice/registerservice.component').then(c => c.RegisterserviceComponent),
        title: 'CryptoAdmin',
    },
];
export default routes;
