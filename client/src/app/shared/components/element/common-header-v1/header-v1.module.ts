import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderV1Component } from './header-v1.component';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { BaseButtonModule } from '../../micro/button/button.module';
import { BaseLoadingModule } from '../../micro/loading/loading.module';

@NgModule({
  declarations: [HeaderV1Component],
  imports: [
    CommonModule,
    HttpClientModule,
    RouterModule,
    BaseButtonModule,
    BaseLoadingModule,
    MatMenuModule,
    MatTooltipModule,
    TranslateModule,
  ],
  exports: [HeaderV1Component]
})
export class BaseHeaderModule { }
