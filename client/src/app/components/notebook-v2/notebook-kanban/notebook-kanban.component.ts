import { Component, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ServiceResult } from 'src/app/models/base/service-result';
import { Note } from 'src/app/models/notebook/note';
import { NoteCategory } from 'src/app/models/notebook/note-category';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { ChangeNoteType } from 'src/app/shared/constants/change-note-type.constant';
import { PopupService } from 'src/app/shared/services/base/popup.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { ConfigService } from 'src/app/shared/services/config/config.service';
import { NoteCategoryService } from 'src/app/shared/services/notebook/note-category.service';
import { NoteService } from 'src/app/shared/services/notebook/note.service';
import { TinyEditorService } from 'src/app/shared/services/tiny-editor/tiny-editor.service';
import { BaseNotebookViewComponent } from '../base-notebook-view';

@Component({
  selector: 'app-notebook-kanban',
  templateUrl: './notebook-kanban.component.html',
  styleUrls: ['./notebook-kanban.component.scss']
})
export class NotebookKanbanComponent extends BaseNotebookViewComponent {

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


  onListReorder(event) {
    console.customize(event);
    // const list = this.lists.splice(e.fromIndex, 1)[0];
    // this.lists.splice(e.toIndex, 0, list);

    // const status = this.statuses.splice(e.fromIndex, 1)[0];
    // this.statuses.splice(e.toIndex, 0, status);
  }

  onDragStart(event) {
    this.isDragging = true;
  }

  onDrop(event) {
    let api: Observable<ServiceResult>;
    const fromIndex = event.fromIndex + 1;
    const toIndex = (event.toIndex > event.toData['notes'].length ? event.toData['notes'].length : event.toIndex) + 1;
    const element = event.fromData['notes'][fromIndex - 1];

    if ((fromIndex == toIndex && event.fromData.id == event.toData.id) || (event.fromData.id == event.toData.id && event.toData.id == 0 )) {
      return;
    }

    event.fromData['notes'].splice(fromIndex - 1, 1);
    event.toData['notes'].splice(toIndex, 0, element);

    if (event.fromData.id == '0') {
      element.isPinned = false;
      element.categoryId = event.toData.id;
      element.order = toIndex;
      api = this.noteService.updateNote(element, ChangeNoteType.IsPinned + ChangeNoteType.CategoryId + ChangeNoteType.Order);

    } else if (event.toData.id == '0') {
      element.isPinned = true;
      api = this.noteService.updateNote(element, ChangeNoteType.IsPinned);

    } else {
      element.categoryId = event.toData.id;
      element.order = toIndex;
      api = this.noteService.updateNote(element, ChangeNoteType.CategoryId + ChangeNoteType.Order);
    }

    api.pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        if (resp.status == 'success') {
          SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["UPDATE_SUCCESS_MSG"], 800));
          this.loadData();
        }
      })
  }
}

class NoteWithLoading extends Note {
  public isLoading = false;
  public isHover = false;
}

class CategoryWithNotes extends NoteCategory {
  public notes: NoteWithLoading[] = [];
  public isEditMode = false;
  public isEditing = false;
  public isDeleting = false;
}
