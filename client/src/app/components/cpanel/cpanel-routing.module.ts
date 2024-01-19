import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CpanelComponent } from './cpanel.component';
import { CpanelDashboardComponent } from './cpanel-dashboard/cpanel-dashboard.component';
import { CpanelUserComponent } from './cpanel-user/cpanel-user.component';
import { CpanelRoleComponent } from './cpanel-role/cpanel-role.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard'
  },
  {
    path: '',
    component: CpanelComponent,
    children: [
      {
        path : 'dashboard',
        component: CpanelDashboardComponent,
      },
      {
        path : 'user',
        component: CpanelUserComponent,
      },
      {
        path : 'role',
        component: CpanelRoleComponent,
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CpanelRoutingModule { }
