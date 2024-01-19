import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccessDeniedRoutingModule } from './access-denied-routing.module';
import { AccessDeniedComponent } from './access-denied.component';
import { BaseButtonModule } from '../../micro/button/button.module';


@NgModule({
  declarations: [AccessDeniedComponent],
  imports: [
    CommonModule,
    AccessDeniedRoutingModule,
    BaseButtonModule
  ],
  exports: [AccessDeniedComponent]
})
export class AccessDeniedModule { }
