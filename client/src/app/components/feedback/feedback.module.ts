import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FeedbackRoutingModule } from './feedback-routing.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';
import { DxTextAreaModule, DxTextBoxModule } from 'devextreme-angular';
import { FeedbackComponent } from './feedback.component';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { FeedbackHeaderComponent } from './feedback-header/feedback-header.component';


@NgModule({
  declarations: [FeedbackComponent, FeedbackHeaderComponent],
  imports: [
    CommonModule,
    FeedbackRoutingModule,
    FormsModule,
    ScrollingModule,
    DxTextBoxModule,
    DxTextAreaModule,
    TranslateModule,
    SharedModule,
    BaseLoadingModule,
  ]
})
export class FeedbackModule { }
