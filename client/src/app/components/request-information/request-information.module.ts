import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RequestInformationRoutingModule } from './request-information-routing.module';
import { RequestInformationComponent } from './request-information.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';


@NgModule({
  declarations: [RequestInformationComponent],
  imports: [
    CommonModule,
    RequestInformationRoutingModule,
    SharedModule,
    TranslateModule,
    BaseLoadingModule
  ]
})
export class RequestInformationModule { }
