import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { MatPaginatorModule } from '@angular/material/paginator';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { ActionLogsRoutingModule } from './audit-log-routing.module';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';
import { AuditLogComponent } from './audit-log.component';


@NgModule({
  declarations: [AuditLogComponent],
  imports: [
    CommonModule,
    ActionLogsRoutingModule,
    TranslateModule,
    SharedModule,
    MatPaginatorModule,
    BaseLoadingModule
  ]
})
export class AuditLogModule { }
