import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SessionTraceLogRoutingModule } from './session-trace-log-routing.module';
import { SessionTraceLogComponent } from './session-trace-log.component';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';
import { MatTooltipModule } from '@angular/material/tooltip';
import { SharedModule } from 'src/app/shared/shared.module';


@NgModule({
  declarations: [SessionTraceLogComponent],
  imports: [
    CommonModule,
    SessionTraceLogRoutingModule,
    ScrollingModule,
    BaseLoadingModule,
    SharedModule,
    MatTooltipModule
  ]
})
export class SessionTraceLogModule { }
