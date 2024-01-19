import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DxNumberBoxModule, DxSelectBoxModule, DxTextBoxModule } from 'devextreme-angular';
import { BaseButtonModule } from '../mp-button/mp-button.module';
import { MpFilterComponent } from './mp-filter.component';



@NgModule({
  declarations: [MpFilterComponent],
  imports: [
    CommonModule,
    BaseButtonModule,
    DxSelectBoxModule,
    DxTextBoxModule,
    DxNumberBoxModule,
  ],
  exports: [MpFilterComponent]
})
export class MpFilterModule { }
