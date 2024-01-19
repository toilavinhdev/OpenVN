import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SignInLoggingRoutingModule } from './sign-in-logging-routing.module';
import { SignInLoggingComponent } from './sign-in-logging.component';
import { BaseGridModule } from 'src/app/shared/components/element/grid/base-grid.module';
import { SharedModule } from 'src/app/shared/shared.module';


@NgModule({
  declarations: [SignInLoggingComponent],
  imports: [
    CommonModule,
    SignInLoggingRoutingModule,
    BaseGridModule,
    SharedModule
  ]
})
export class SignInLoggingModule { }
