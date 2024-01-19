import { Component, Inject, Injector } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { takeUntil } from 'rxjs/operators';
import { Tracking } from 'src/app/models/core/tracking';
import { Message } from 'src/app/models/message';
import { Note } from 'src/app/models/notebook/note';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { MessageBox } from 'src/app/shared/components/element/message-box/message-box.component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { ChangeNoteType } from 'src/app/shared/constants/change-note-type.constant';
import { Event } from 'src/app/shared/constants/event';
import { FormMode } from 'src/app/shared/enumerations/common.enum';
import { PopupService } from 'src/app/shared/services/base/popup.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { NoteService } from 'src/app/shared/services/notebook/note.service';
import { TinyEditorService } from 'src/app/shared/services/tiny-editor/tiny-editor.service';
import { CreateOrUpdateNoteComponent } from '../create-or-update-note/create-or-update-note.component';

@Component({
  selector: 'app-view-note',
  templateUrl: './view-note.component.html',
  styleUrls: ['./view-note.component.scss']
})
export class ViewNoteComponent extends BaseComponent {

  note = new Note();
  dialogRef: MatDialogRef<CreateOrUpdateNoteComponent>;
  isLoadingEditor = false;
  config = this.tinyEditorService.getConfig(window.innerHeight * 0.5, () => this.isLoadingEditor = false);

  constructor(
    public injector: Injector,
    public noteService: NoteService,
    public dialog: MatDialog,
    public viewRef: MatDialogRef<ViewNoteComponent>,
    public transferService: TransferDataService,
    public popupService: PopupService,
    public tinyEditorService: TinyEditorService,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super(injector);
  }

  initData() {
    super.initData();
    this.loadData();
  }

  tracking(event?: Tracking, callback?: Function): void {
    event = this.commonTrackingEvent(Event.VIEW_NOTE);
    event.data = this.data.id;
    super.tracking(event, callback);
  }

  loadData() {
    this.isLoading = true;
    this.noteService.getNote(this.data.id)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          if (resp.status == 'success') {
            this.note = resp.data;
            this.isLoadingEditor = true;
            this.calculateHeight(this.note.content);
            this.isLoading = false;
          } else {
            this.isLoading = false;
          }
        },
        _ => this.isLoading = false
      )
  }

  calculateHeight(content: string) {
    const element = document.getElementById("calculator");
    element.innerHTML = content;
    element.style.fontSize = "13px";

    const height = element.clientHeight;
    let newHeight = 0;

    if (height <= 18) {
      newHeight = height + 60;
    } else if (height <= 80) {
      newHeight = height + 120;
    } else {
      newHeight = height + 180;
    }

    this.config['height'] = newHeight > window.innerHeight * 0.5 ? window.innerHeight * 0.5 : newHeight;
    element.innerHTML = '';
  }

  edit(note: Note) {
    const config = this.popupService.maxPingConfig();
    config['panelClass'] = 'fucking-create';
    config['data'] = {
      note: note,
      formMode: FormMode.Update
    };

    this.dialogRef = this.dialog.open(CreateOrUpdateNoteComponent, config);
    this.dialogRef.updatePosition({ top: '60px' });
    setTimeout(() => {
      this.viewRef.close(false);
    }, 50);
  }

  remove(note: Note) {
    MessageBox.confirmDelete(new Message(this, { content: TranslationService.VALUES["COMMON"]["WARNING_DELETE_SUCCESS_MSG"] }, () => {
      this.noteService
        .deleteNote([note.id])
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(resp => {
          this.isLoading = false;
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["DELETE_SUCCESS_MSG"], 800));
            this.transferService.reloadListNotesEvent.emit();
            this.viewRef.close();
          }
        })
    }));
  }

  changeMode(note: Note, value: boolean) {
    if (value) {
      MessageBox.confirm(new Message(this, { content: TranslationService.VALUES["NOTEBOOK"]["WARNING_PUBLIC_NOTE"] }, () => {
        this.saveChangeMode(note, value);
      }));
    } else {
      this.saveChangeMode(note, value);
    }
  }

  saveChangeMode(note: Note, value: boolean) {
    this.isLoading = true;
    this.note.isPublic = value;
    this.noteService.updateNote(this.note, ChangeNoteType.IsPublic)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["UPDATE_SUCCESS_MSG"], 800));
            this.loadData();
          }
        },
        _ => this.isLoading = false
      )
  }

  close() {
    this.viewRef.close();
  }
}
