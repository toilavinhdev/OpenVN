import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TokenReceiverRoutingModule } from './token-receiver-routing.module';
import { TokenReceiverComponent } from './token-receiver.component';


@NgModule({
  declarations: [TokenReceiverComponent],
  imports: [
    CommonModule,
    TokenReceiverRoutingModule
  ]
})
export class TokenReceiverModule { }
