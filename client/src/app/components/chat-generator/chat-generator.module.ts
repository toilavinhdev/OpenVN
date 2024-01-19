import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GenChatRoutingModule as ChatGeneratorRoutingModule } from './chat-generator-routing.module';
import { ChatGeneratorComponent } from './chat-generator.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { BaseUploaderModule } from 'src/app/shared/components/micro/uploader/uploader.module';
import { DxCheckBoxModule, DxColorBoxModule, DxNumberBoxModule, DxSliderModule, DxTextBoxModule } from 'devextreme-angular';
import { BaseButtonModule } from 'src/app/shared/components/micro/button/button.module';
import { GenerateHistoryComponent } from './generate-history/generate-history.component';
import { TranslateModule } from '@ngx-translate/core';
import { MatPaginatorModule } from '@angular/material/paginator';


@NgModule({
  declarations: [ChatGeneratorComponent, GenerateHistoryComponent],
  imports: [
    CommonModule,
    ChatGeneratorRoutingModule,
    SharedModule,
    TranslateModule,
    BaseUploaderModule,
    BaseButtonModule,
    DxNumberBoxModule,
    DxColorBoxModule,
    DxSliderModule,
    DxTextBoxModule,
    DxCheckBoxModule,
    MatPaginatorModule,
  ]
})
export class GenChatModule { }
