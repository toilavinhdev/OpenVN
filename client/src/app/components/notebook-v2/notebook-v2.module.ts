import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NotebookV2RoutingModule } from './notebook-v2-routing.module';
import { NotebookV2Component } from './notebook-v2.component';
import { NotebookAddComponent } from './notebook-add/notebook-add.component';
import { NotebookListComponent } from './notebook-list/notebook-list.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { PopupModule } from '../z-popup/popup.module';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { DxListModule, DxPopoverModule, DxScrollViewModule, DxSortableModule, DxSwitchModule, DxTemplateModule, DxTextBoxModule } from 'devextreme-angular';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { EditorModule } from '@tinymce/tinymce-angular';
import { MatMenuModule } from '@angular/material/menu';
import { NotebookKanbanComponent } from './notebook-kanban/notebook-kanban.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';
import { BaseButtonModule } from 'src/app/shared/components/micro/button/button.module';


@NgModule({
  declarations: [NotebookV2Component, NotebookAddComponent, NotebookListComponent, NotebookKanbanComponent],
  imports: [
    CommonModule,
    NotebookV2RoutingModule,
    TranslateModule,
    FormsModule,
    PopupModule,
    BaseLoadingModule,
    BaseButtonModule,
    MatTooltipModule,
    MatInputModule,
    MatFormFieldModule,
    MatMenuModule,
    DxTextBoxModule,
    DxListModule,
    DxPopoverModule,
    DxTemplateModule,
    DxScrollViewModule,
    DxSortableModule,
    DxSwitchModule,
    DragDropModule,
    EditorModule,
    SharedModule,
  ],
  exports: [NotebookV2Component, NotebookAddComponent],
})
export class NotebookV2Module { }
