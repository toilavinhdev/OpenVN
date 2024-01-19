import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SignUpRoutingModule } from './sign-up-routing.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { SignUpComponent } from './sign-up.component';
import { LanguageModule } from 'src/app/shared/components/micro/language/language.module';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { BaseButtonModule } from 'src/app/shared/components/micro/button/button.module';
import { DxCheckBoxModule } from 'devextreme-angular';


@NgModule({
  declarations: [SignUpComponent],
  imports: [
    CommonModule,
    SignUpRoutingModule,
    FormsModule,
    SharedModule,
    BaseButtonModule,
    LanguageModule,
    TranslateModule,
    DxCheckBoxModule
  ]
})
export class SignUpModule { }
