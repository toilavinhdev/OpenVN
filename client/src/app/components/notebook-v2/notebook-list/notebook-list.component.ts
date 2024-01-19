import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PopupService } from 'src/app/shared/services/base/popup.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { ConfigService } from 'src/app/shared/services/config/config.service';
import { NoteCategoryService } from 'src/app/shared/services/notebook/note-category.service';
import { NoteService } from 'src/app/shared/services/notebook/note.service';
import { TinyEditorService } from 'src/app/shared/services/tiny-editor/tiny-editor.service';
import { BaseNotebookViewComponent } from '../base-notebook-view';

@Component({
  selector: 'app-notebook-list',
  templateUrl: './notebook-list.component.html',
  styleUrls: ['./notebook-list.component.scss']
})
export class NotebookListComponent extends BaseNotebookViewComponent {

  constructor(
    public injector: Injector,
    public noteService: NoteService,
    public noteCategoryService: NoteCategoryService,
    public dialog: MatDialog,
    public transferService: TransferDataService,
    public popupService: PopupService,
    public tinyEditorService: TinyEditorService,
    public sharedService: SharedService,
    public configService: ConfigService
  ) {
    super(injector, noteService, noteCategoryService, dialog, transferService, popupService, tinyEditorService, sharedService, configService);
  }

  drag() {
    this.isDragging = true;
  }

  dragEnded() {
    setTimeout(() => {
      this.isDragging = false;
    }, 100);
  }

  scrollOnMoving(event) {
  }
}
