import { AfterViewInit, Component, ElementRef, HostListener, Inject, Injector, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { FormMode } from 'src/app/shared/enumerations/common.enum';
import { DeviceType } from 'src/app/shared/enumerations/device.enum';
import { Message } from 'src/app/models/message';
import { Note } from 'src/app/models/notebook/note';
import { NoteCategory } from 'src/app/models/notebook/note-category';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { PopupService } from 'src/app/shared/services/base/popup.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { NoteCategoryService } from 'src/app/shared/services/notebook/note-category.service';
import { NoteService } from 'src/app/shared/services/notebook/note.service';
import { CreateOrUpdateNoteCategoryComponent } from '../create-or-update-note-category/create-or-update-note-category.component';
import { ViewNoteComponent } from '../view-note/view-note.component';
import { TinyEditorService } from 'src/app/shared/services/tiny-editor/tiny-editor.service';
import { ChangeNoteType } from 'src/app/shared/constants/change-note-type.constant';
import { BaseButton } from 'src/app/shared/components/micro/button/button.component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { MessageBox } from 'src/app/shared/components/element/message-box/message-box.component';

@Component({
  selector: 'app-create-or-update-note',
  templateUrl: './create-or-update-note.component.html',
  styleUrls: ['./create-or-update-note.component.scss']
})
export class CreateOrUpdateNoteComponent extends BaseComponent implements AfterViewInit {

  FormMode = FormMode;

  note = new Note();

  clone: Note = JSON.parse(JSON.stringify(this.note));

  categories: NoteCategory[] = [];

  isFetchingCategory = false;

  isFetchingNote = false;

  categoryRef: MatDialogRef<CreateOrUpdateNoteCategoryComponent>;

  viewRef: MatDialogRef<ViewNoteComponent>;

  config = this.tinyEditorService.getConfig(window.innerHeight * 0.5, () => this.isLoadingEditor = false);

  isLoadingEditor = false;

  @ViewChild("title")
  title!: ElementRef;

  @ViewChild("saveBtn")
  saveBtn: BaseButton;

  @ViewChild("tinymce")
  tinymce!: any;

  @HostListener('window:keyup.esc') onKeyUp() {
    this.confirmClose();
  }

  // @HostListener("window:beforeunload", ["$event"]) unloadHandler(event: Event) {
  //   event.returnValue = false;
  // }

  constructor(
    public injector: Injector,
    public activatedRoute: ActivatedRoute,
    public router: Router,
    public sharedService: SharedService,
    public noteService: NoteService,
    public noteCategoryService: NoteCategoryService,
    public dialogRef: MatDialogRef<CreateOrUpdateNoteComponent>,
    public transferService: TransferDataService,
    public popupService: PopupService,
    public dialog: MatDialog,
    public tinyEditorService: TinyEditorService,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.dialogRef.disableClose = true;
    this.dialogRef.backdropClick().subscribe(_ => {
      this.confirmClose();
    });
  }

  ngAfterViewInit() {
    if (SharedService.DeviceType == DeviceType.Desktop) {
      this.title.nativeElement.focus();
    }
  }

  initData() {
    super.initData();
    this.loadData();
  }

  loadData() {
    if (this.data.formMode == FormMode.Update) {
      this.isLoading = true;
      forkJoin(this.noteService.getNote(this.data.note.id), this.noteCategoryService.getCategories())
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(
          resp => {
            this.isLoading = false;
            this.isLoadingEditor = true;
            const resp1 = resp[0];
            const resp2 = resp[1];

            if (resp1.status == 'success') {
              this.note = resp1.data;
              this.clone = JSON.parse(JSON.stringify(this.note));
            }
            if (resp2.status == 'success') {
              this.categories = resp2.data;
            }
          },
          _ => this.isLoading = false
        )
    } else {
      this.isLoadingEditor = true;
      if (this.data.categoryId) {
        this.note.categoryId = this.data.categoryId;
        this.clone = JSON.parse(JSON.stringify(this.note));

        this.noteCategoryService.getCategories()
          .pipe(takeUntil(this._onDestroySub))
          .subscribe(
            resp => {
              this.isFetchingCategory = false;
              if (resp.status == 'success') {
                this.categories = resp.data;
              }
            },
            () => this.isFetchingCategory = false
          );
      }
    }
  }

  getMasterId() {
    return this.activatedRoute.snapshot.params['id'] + "";
  }

  getData(id: string) {
    this.isFetchingNote = true;
    this.isFetchingCategory = true;

    forkJoin(
      this.noteCategoryService.getCategories(),
      this.noteService.getNote(id)
    )
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isFetchingNote = false;
          this.isFetchingCategory = false;

          const resp1 = resp[0];
          const resp2 = resp[1];

          if (resp1.status == 'success') {
            this.categories = resp1.data;
          }
          if (resp2.status == 'success') {
            this.note = resp2.data;
          }
        },
        () => {
          this.isFetchingNote = false;
          this.isFetchingCategory = false;
        }
      );
  }

  loadCategories() {
    if (this.isFetchingCategory) {
      return;
    }

    this.isFetchingCategory = true;
    this.noteCategoryService.getCategories()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isFetchingCategory = false;
          if (resp.status == 'success') {
            this.categories = resp.data;
          }
        },
        () => this.isFetchingCategory = false
      );
  }

  validateBeforeSave() {
    const result = {
      isValid: true,
      message: ''
    }
    if (this.note.title == '' || this.note.title.trim() == '') {
      result.isValid = false;
      result.message = TranslationService.VALUES['NOTEBOOK']['TITLE_NOT_EMPTY'];
    } else if (this.note.content == '' || this.note.content.trim() == '') {
      result.isValid = false;
      result.message = TranslationService.VALUES['NOTEBOOK']['CONTENT_NOT_EMPTY'];
    } else if (this.note.categoryId == '' || this.note.categoryId == '0') {
      result.isValid = false;
      result.message = TranslationService.VALUES['NOTEBOOK']['CATEGORY_NOT_EMPTY'];
    }
    return result;
  }

  save() {
    if (this.isLoading || this.isFetchingNote || this.isFetchingCategory) {
      return;
    }

    const validate = this.validateBeforeSave();
    if (!validate.isValid) {
      this.saveBtn.isFinished = true;
      SnackBar.warning(new SnackBarParameter(this, validate.message));
      return;
    }

    const type = ChangeNoteType.Title + ChangeNoteType.Content + ChangeNoteType.CategoryId + ChangeNoteType.IsPublic + ChangeNoteType.IsPinned;
    const api = this.data.formMode == FormMode.Add ? this.noteService.saveNote(this.note) : this.noteService.updateNote(this.note, type);
    api.pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.saveBtn.isFinished = true;
          if (resp.status == 'success') {
            if (this.data.formMode == FormMode.Update) {
              SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["UPDATE_SUCCESS_MSG"], 2000));
              const config = this.popupService.maxPingConfig();
              config['panelClass'] = 'fucking-view';
              config['data'] = {
                id: this.note.id
              }
              this.viewRef = this.dialog.open(ViewNoteComponent, config);
              this.viewRef.updatePosition({ top: '60px' });
            } else {
              SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["SAVE_SUCCESS_MSG"], 2000));
            }
            this.transferService.reloadListNotesEvent.emit();
            setTimeout(() => {
              this.dialogRef.close();
            }, 50);
          }
        },
        _ => this.saveBtn.isFinished = true
      );
  }

  confirmClose() {
    if (JSON.stringify(this.note) != JSON.stringify(this.clone)) {
      MessageBox.confirm(new Message(this, { content: TranslationService.VALUES['NOTEBOOK']['CANCEL_WARNING'] }, () => {
        this.dialogRef.close(false);
      }));
    }
    else {
      this.dialogRef.close(false);
    }
  }

  openCreateCategory(event) {
    const config = this.popupService.maxPingConfig();
    config['panelClass'] = 'fucking-category-create';
    config['minHeight'] = '100px';
    config['height'] = '160px';
    this.categoryRef = this.dialog.open(CreateOrUpdateNoteCategoryComponent, config);
    this.categoryRef.updatePosition({ top: '80px' });
  }
}
