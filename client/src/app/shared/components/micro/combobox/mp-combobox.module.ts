import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DxSelectBoxModule } from 'devextreme-angular';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MpComboboxComponent } from './mp-combobox.component';

@NgModule({
  declarations: [MpComboboxComponent],
  imports: [
    CommonModule,
    DxSelectBoxModule,
    MatProgressSpinnerModule,
    MatIconModule,
  ],
  exports: [MpComboboxComponent]
})
export class MpComboboxModule { }
