import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NotebookViewModeRoutingModule } from './notebook-view-mode-routing.module';
import { NotebookViewModeComponent } from './notebook-view-mode.component';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { MatTooltipModule } from '@angular/material/tooltip';
import { EditorModule } from '@tinymce/tinymce-angular';
import { MatMenuModule } from '@angular/material/menu';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';
import { BaseButtonModule } from 'src/app/shared/components/micro/button/button.module';


@NgModule({
  declarations: [NotebookViewModeComponent],
  imports: [
    CommonModule,
    NotebookViewModeRoutingModule,
    TranslateModule,
    FormsModule,
    BaseLoadingModule,
    BaseButtonModule,
    MatTooltipModule,
    MatMenuModule,
    EditorModule,
  ]
})
export class NotebookViewModeModule { }
