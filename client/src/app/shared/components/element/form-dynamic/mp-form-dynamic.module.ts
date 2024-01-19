import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatMenuModule } from '@angular/material/menu';
import { DxCheckBoxModule, DxDateBoxModule, DxFileUploaderModule, DxTextAreaModule } from 'devextreme-angular';
import { DxNumberBoxModule } from 'devextreme-angular/ui/number-box';
import { DxSelectBoxModule } from 'devextreme-angular/ui/select-box';
import { DxTextBoxModule } from 'devextreme-angular/ui/text-box';
import { SharedModule } from '../../shared.module';
import { BaseButtonModule } from '../mp-button/mp-button.module';
import { MpComboboxModule } from '../mp-combobox/mp-combobox.module';
import { MpGridModule } from '../mp-grid/mp-grid.module';
import { MpFormDynamicComponent } from './mp-form-dynamic.component';
import { MpUploaderModule } from '../uploader/uploader.module';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [MpFormDynamicComponent],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    BaseButtonModule,
    MpComboboxModule,
    MpGridModule,
    MpUploaderModule,
    DxSelectBoxModule,
    DxTextBoxModule,
    DxNumberBoxModule,
    DxDateBoxModule,
    DxCheckBoxModule,
    DxFileUploaderModule,
    DxTextAreaModule,
    MatMenuModule,
    TranslateModule,
  ],
  exports: [MpFormDynamicComponent]
})
export class MpFormDynamicModule { }
