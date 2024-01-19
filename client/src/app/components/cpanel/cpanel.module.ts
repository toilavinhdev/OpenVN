import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CpanelRoutingModule } from './cpanel-routing.module';
import { CpanelComponent } from './cpanel.component';
import { CpanelDashboardComponent } from './cpanel-dashboard/cpanel-dashboard.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';
import { TranslateModule } from '@ngx-translate/core';
import { CpanelUserComponent } from './cpanel-user/cpanel-user.component';
import { BaseGridModule } from 'src/app/shared/components/element/grid/base-grid.module';
import { CpanelRoleComponent } from './cpanel-role/cpanel-role.component';
import { DxCheckBoxModule } from 'devextreme-angular';
import { BaseButtonModule } from 'src/app/shared/components/micro/button/button.module';


@NgModule({
  declarations: [CpanelComponent, CpanelDashboardComponent, CpanelUserComponent, CpanelRoleComponent],
  imports: [
    CommonModule,
    CpanelRoutingModule,
    SharedModule,
    BaseLoadingModule,
    BaseGridModule,
    BaseButtonModule,
    TranslateModule,
    DxCheckBoxModule,
  ]
})
export class CpanelModule { }
