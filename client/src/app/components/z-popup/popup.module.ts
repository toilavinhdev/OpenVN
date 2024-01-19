import { DragDropModule } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { EditorModule } from '@tinymce/tinymce-angular';
import { ViewNoteComponent } from './notebook/view-note/view-note.component';
import { CreateOrUpdateNoteComponent } from './notebook/create-or-update-note/create-or-update-note.component';
import { CreateOrUpdateNoteCategoryComponent } from './notebook/create-or-update-note-category/create-or-update-note-category.component';
import { CloudEnterPasswordComponent } from './cloud/cloud-enter-password/cloud-enter-password.component';
import { DxTextBoxModule } from 'devextreme-angular';
import { CloudSetPasswordComponent } from './cloud/cloud-set-password/cloud-set-password.component';
import { MovingProgessComponent } from './cloud/moving-progess/moving-progess.component';
import { CloudUploaderComponent } from './cloud/cloud-uploader/cloud-uploader.component';
import { MpComboboxModule } from 'src/app/shared/components/micro/combobox/mp-combobox.module';
import { BaseButtonModule } from 'src/app/shared/components/micro/button/button.module';
import { BaseLoadingModule } from 'src/app/shared/components/micro/loading/loading.module';
import { BaseUploaderModule } from 'src/app/shared/components/micro/uploader/uploader.module';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    CreateOrUpdateNoteComponent,
    ViewNoteComponent,
    CreateOrUpdateNoteCategoryComponent,
    CloudUploaderComponent,
    CloudEnterPasswordComponent,
    CloudSetPasswordComponent,
    MovingProgessComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    DragDropModule,
    TranslateModule,
    MpComboboxModule,
    BaseButtonModule,
    BaseLoadingModule,
    MatTooltipModule,
    MatInputModule,
    MatSelectModule,
    MatFormFieldModule,
    EditorModule,
    BaseUploaderModule,
    DxTextBoxModule,
    SharedModule
  ],
  exports: [CreateOrUpdateNoteComponent, ViewNoteComponent, CloudUploaderComponent]
})
export class PopupModule { }
