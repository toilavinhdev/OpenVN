import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatPaginatorModule } from '@angular/material/paginator';
import { DxCheckBoxModule } from 'devextreme-angular';
import { SharedModule } from 'src/app/shared/shared.module';
import { BaseLoadingModule } from '../../micro/loading/loading.module';
import { GridComponent } from './base-grid.component';

@NgModule({
  declarations: [GridComponent],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    BaseLoadingModule,
    DxCheckBoxModule,
    MatPaginatorModule,
  ],
  exports: [GridComponent]
})
export class BaseGridModule { }
