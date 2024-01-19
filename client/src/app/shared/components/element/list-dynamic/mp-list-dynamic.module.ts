import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { DxTextBoxModule } from 'devextreme-angular';
import { SharedModule } from '../../shared.module';
import { BaseButtonModule } from '../mp-button/mp-button.module';
import { MpFilterModule } from '../mp-filter/mp-filter.module';
import { MpGridModule } from '../mp-grid/mp-grid.module';
import { PopupChooseExportComponent } from './popup-choose-export/popup-choose-export.component';
import { MpListDynamicComponent } from './mp-list-dynamic.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [MpListDynamicComponent, PopupChooseExportComponent],
  imports: [
    CommonModule,
    SharedModule,
    BaseButtonModule,
    MpGridModule,
    MpFilterModule,
    MatMenuModule,
    MatCardModule,
    MatRadioModule,
    DxTextBoxModule,
    TranslateModule,
  ],
  exports: [MpListDynamicComponent, PopupChooseExportComponent]
})
export class MpListDynamicModule { }
