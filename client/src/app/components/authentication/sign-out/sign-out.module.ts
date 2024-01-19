import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SignOutRoutingModule } from './sign-out-routing.module';
import { SignOutComponent as SignOutComponent } from './sign-out.component';
import { TranslateModule } from '@ngx-translate/core';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';


@NgModule({
  declarations: [SignOutComponent],
  imports: [
    CommonModule,
    SignOutRoutingModule,
    TranslateModule,
    BaseLoadingModule
  ],
  exports: [SignOutComponent],
})
export class SignOutModule { }
