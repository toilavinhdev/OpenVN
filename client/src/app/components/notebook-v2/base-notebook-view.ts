import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { forkJoin } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { FormMode } from 'src/app/shared/enumerations/common.enum';
import { Note } from 'src/app/models/notebook/note';
import { NoteCategory } from 'src/app/models/notebook/note-category';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { NoteCategoryService } from 'src/app/shared/services/notebook/note-category.service';
import { NoteService } from 'src/app/shared/services/notebook/note.service';
import { MessageBox } from 'src/app/shared/components/element/message-box/message-box.component';
import { Message } from 'src/app/models/message';
import { Utility } from 'src/app/shared/utility/utility';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { PopupService } from 'src/app/shared/services/base/popup.service';
import { TinyEditorService } from 'src/app/shared/services/tiny-editor/tiny-editor.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { DeviceType } from 'src/app/shared/enumerations/device.enum';
import { DxPopoverComponent } from 'devextreme-angular';
import { ViewNoteComponent } from '../z-popup/notebook/view-note/view-note.component';
import { ConfigService } from 'src/app/shared/services/config/config.service';
import { NotebookType } from 'src/app/shared/enumerations/notebook-type.enum';
import { ChangeNoteType } from 'src/app/shared/constants/change-note-type.constant';
import { CreateOrUpdateNoteComponent } from '../z-popup/notebook/create-or-update-note/create-or-update-note.component';

@Component({
  selector: 'first-check',
  template: '',
  styles: []
})
export class BaseNotebookViewComponent extends BaseComponent {
  DeviceType = DeviceType;

  NotebookType = NotebookType;

  categories: CategoryWithNotes[] = [];

  category = new CategoryWithNotes();

  notes: NoteWithLoading[] = [];

  isDragging = false;

  isAddingCategory = false;

  config = this.tinyEditorService.getConfig(window.innerHeight * 0.5, () => null);

  colors = ['#fff', '#f28b82', '#fbbc04', '#fff475', '#ccff90', '#a7ffeb', '#cbf0f8', '#aecbfa', '#d7aefb', '#fdcfe8', '#e8eaed'];

  isPopoverVisible = false;

  targetId = "";

  dialogRef: MatDialogRef<ViewNoteComponent>;

  editRef: MatDialogRef<CreateOrUpdateNoteComponent>;

  @Input()
  query = "";

  @Output("addNoteEvent")
  addNoteEvent = new EventEmitter<CategoryWithNotes>();

  @ViewChild("backgroundPopover")
  backgroundPopover: DxPopoverComponent;

  @ViewChild("categoryInput")
  categoryInput: ElementRef;

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
    super(injector);
  }

  initData(): void {
    this.loadData();
    this.transferService.reloadListNotesEvent
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(_ => this.loadData());
  }

  loadData() {
    this.isLoading = true;
    forkJoin(this.noteService.getNotes(this.query), this.noteCategoryService.getCategories())
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          const resp1 = resp[0];
          const resp2 = resp[1];
          this.isLoading = false;
          if (resp1.status == 'success') {
            this.notes = [];
            this.notes = resp1.data;
            this.notes.forEach(note => note.originBackgroundColor = note.backgroundColor);
          }
          if (resp2.status == 'success') {
            this, this.categories = [];
            this.categories = resp2.data;

            const pinnedCategory = new CategoryWithNotes();
            pinnedCategory.id = "0";
            pinnedCategory.name = "Pinned";
            pinnedCategory.notes = this.notes.filter(n => n.isPinned);

            this.categories.forEach(cate => {
              cate.isExpand = true;
              cate.notes = this.notes.filter(n => n.categoryId == cate.id && !n.isPinned);
            });
            this.categories = [pinnedCategory].concat(this.categories);
          }
        },
        () => this.isLoading = false
      )
  }

  view(note: NoteWithLoading, event) {
    if (this.isDragging || note.isLoading) {
      return;
    }

    const config = this.popupService.maxPingConfig();
    config['panelClass'] = 'fucking-view';
    config['data'] = {
      id: note.id
    }

    this.dialogRef = this.dialog.open(ViewNoteComponent, config);
    this.dialogRef.updatePosition({ top: '60px' });
  }

  edit(note: NoteWithLoading, event) {
    event.stopPropagation();
    if (this.isDragging || note.isLoading) {
      return;
    }

    const config = this.popupService.maxPingConfig();
    config['panelClass'] = 'fucking-create';
    config['data'] = {
      note: note,
      formMode: FormMode.Update
    };
    this.editRef = this.dialog.open(CreateOrUpdateNoteComponent, config);
    this.editRef.updatePosition({ top: '60px' });
  }

  changePin(note: NoteWithLoading, isPin: boolean, event) {
    event.stopPropagation();
    if (note.isLoading) {
      return;
    }

    note.isLoading = true;
    note.isPinned = isPin;
    this.noteService
      .updateNote(note, ChangeNoteType.IsPinned)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          note.isLoading = false;
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["NOTEBOOK"][isPin ? "PINNED_SUCCESS_MSG" : "UNPINNED_SUCCESS_MSG"], 2000));
            this.loadData();
          }
        },
        _ => note.isLoading = false
      )
  }

  changeBackground(note: NoteWithLoading, color: string, event) {
    this.isPopoverVisible = false;
    this.targetId = "";

    event.stopPropagation();
    if (note.isLoading) {
      return;
    }

    note.isLoading = true;
    note.backgroundColor = color;
    this.noteService
      .updateNote(note, ChangeNoteType.BackgroundColor)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          note.isLoading = false;
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["UPDATE_SUCCESS_MSG"], 800));
            this.loadData();
          }
        },
        _ => note.isLoading = false
      )
  }

  remove(note: NoteWithLoading, event) {
    event.stopPropagation();
    if (note.isLoading) {
      return;
    }

    MessageBox.confirmDelete(new Message(this, { content: TranslationService.VALUES["COMMON"]["WARNING_DELETE_SUCCESS_MSG"] }, () => {
      note.isLoading = true;
      this.noteService
        .deleteNote([note.id])
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(
          resp => {
            note.isLoading = false;
            if (resp.status == 'success') {
              SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["DELETE_SUCCESS_MSG"], 2000));
              this.loadData();
            }
          },
          _ => note.isLoading = false
        )
    }));
  }

  setNotification(note: NoteWithLoading, event) {
    event.stopPropagation();
    Utility.featureIsInDevelopment(event);
  }

  setBackground(note: NoteWithLoading, event) {
    event.stopPropagation();
    this.isPopoverVisible = true;
    this.targetId = note.id;
  }

  editMode(category: CategoryWithNotes) {
    category.isEditMode = true;
    setTimeout(() => {
      this.categoryInput.nativeElement.focus();
    }, 20);
  }

  updateCategory(category: CategoryWithNotes) {
    if (category.isEditing) {
      return;
    }

    category.isEditing = true;
    this.noteCategoryService.updateCategory(category)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          category.isEditing = false;
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['COMMON']['UPDATE_SUCCESS_MSG']));
            this.loadData();
          }
        },
        _ => category.isEditing = false
      )
  }

  onKeyup(category: CategoryWithNotes, event) {
    if (event.key == 'Enter') {
      this.updateCategory(category);
    }
  }

  removeCategory(category: CategoryWithNotes) {
    if (category.isDeleting) {
      return;
    }

    MessageBox.confirmDelete(new Message(this, { content: TranslationService.VALUES['COMMON']['WARNING_DELETE_SUCCESS_MSG'] }, () => {
      category.isDeleting = true;
      this.noteCategoryService.deleteCategories([category.id])
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(
          resp => {
            category.isDeleting = false;
            if (resp.status == 'success') {
              SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['COMMON']['DELETE_SUCCESS_MSG']));
              this.loadData();
            }
          },
          _ => category.isDeleting = false)
    }));
  }

  openPopup(category: CategoryWithNotes) {
    this.addNoteEvent.emit(category);
  }

  startAddingCategory() {
    this.isAddingCategory = true;
    setTimeout(() => {
      this.categoryInput.nativeElement.focus();
    }, 50);
  }

  addCategory() {
    this.isAddingCategory = true;
    if (this.addCategory.name.trim() == '') {
      SnackBar.danger(new SnackBarParameter(this, TranslationService.VALUES['NOTEBOOK']['CATEGORY_NAME_NOT_EMPTY'], 1000));
      return;
    }
    this.noteCategoryService.saveCategory(this.category)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        if (resp.status == 'success') {
          SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['COMMON']['SAVE_SUCCESS_MSG'], 1000));
          this.category = new CategoryWithNotes();
          this.loadData();
        }
      });
  }

  onEnter(event) {
    if (event.key == 'Enter') {
      this.addCategory();
    }
  }
}


export class NoteWithLoading extends Note {
  public isLoading = false;
  public isHover = false;
}

export class CategoryWithNotes extends NoteCategory {
  public notes: NoteWithLoading[] = [];
  public isExpand = true;
  public isEditMode = false;
  public isEditing = false;
  public isDeleting = false;
}
