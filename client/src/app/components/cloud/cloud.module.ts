import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DirectoryRoutingModule } from './cloud-routing.module';
import { CloudComponent } from './cloud.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { DxPopoverModule } from 'devextreme-angular';
import { BaseButtonModule } from 'src/app/shared/components/micro/button/button.module';

@NgModule({
  declarations: [CloudComponent],
  imports: [
    CommonModule,
    FormsModule,
    DirectoryRoutingModule,
    SharedModule,
    BaseLoadingModule,
    BaseButtonModule,
    TranslateModule,
    DxPopoverModule
  ]
})
export class CloudModule { }
