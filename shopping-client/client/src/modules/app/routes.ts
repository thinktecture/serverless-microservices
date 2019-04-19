import {Routes} from '@angular/router';
import {HomeComponent} from './components/home/home';
import {LoginComponent} from './components/login/login';
import {IsAuthenticated} from './guards/isAuthenticated';
import {OrderListComponent} from './components/list/orderList';

export const ROUTES: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: '/home'
  },
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'orders',
    canActivate: [IsAuthenticated],
    children: [
      {
        path: 'list',
        component: OrderListComponent
      }
    ]
  }
];
