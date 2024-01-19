import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgOtpInputModule } from  'ng-otp-input';
import { SharedModule } from 'src/app/shared/shared.module';
import { MpOtpBoxComponent } from './mp-otp-box.component';
import { BaseButtonModule } from '../mp-button/mp-button.module';

@NgModule({
  declarations: [MpOtpBoxComponent],
  imports: [
    CommonModule,
    NgOtpInputModule,
    SharedModule,
    BaseButtonModule,
  ],
  exports: [MpOtpBoxComponent]
})
export class MpOtpBoxModule { }
