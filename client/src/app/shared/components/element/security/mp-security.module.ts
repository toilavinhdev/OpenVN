import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { DxTextBoxModule } from 'devextreme-angular';
import { BaseButtonModule } from '../mp-button/mp-button.module';
import { MpSecurityComponent } from './mp-security.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [MpSecurityComponent],
  imports: [
    CommonModule,
    DxTextBoxModule,
    BaseButtonModule,
    TranslateModule,
  ],
  exports: [MpSecurityComponent]
})
export class MpSecurityModule { }
