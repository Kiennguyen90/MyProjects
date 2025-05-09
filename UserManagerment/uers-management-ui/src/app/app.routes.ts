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
];
export default routes;
