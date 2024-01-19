import { Component, Injector, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { Note } from 'src/app/models/notebook/note';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { NoteCategoryService } from 'src/app/shared/services/notebook/note-category.service';
import { NoteService } from 'src/app/shared/services/notebook/note.service';
import { TinyEditorService } from 'src/app/shared/services/tiny-editor/tiny-editor.service';
import { ButtonColor } from 'src/app/shared/constants/button.constant';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { ChangeNoteType } from 'src/app/shared/constants/change-note-type.constant';
import { BaseButton } from 'src/app/shared/components/micro/button/button.component';

@Component({
  selector: 'app-notebook-view-mode',
  templateUrl: './notebook-view-mode.component.html',
  styleUrls: ['./notebook-view-mode.component.scss']
})
export class NotebookViewModeComponent extends BaseComponent {

  note: Note = null;

  isLoadingEditor = false;

  config = this.tinyEditorService.autoResizeConfig(() => this.isLoadingEditor = false);

  masterId = "0";

  ownerId = "";

  isLoading = true;

  isView = true;

  @ViewChild("saveBtn")
  saveBtn: BaseButton;

  constructor(
    public injector: Injector,
    public noteService: NoteService,
    public noteCategoryService: NoteCategoryService,
    public tinyEditorService: TinyEditorService,
    public activatedRoute: ActivatedRoute,
    public router: Router,
    public authService: AuthService,
    public transferService: TransferDataService
  ) {
    super(injector);
  }

  initData(): void {
    super.initData();

    if (!this.authService.isSignedIn()) {
      this.transferService.resolvedEvent.emit();
    }

    this.getIds();
    this.loadData();
  }

  getIds() {
    this.masterId = this.activatedRoute.snapshot.params['id'];
    this.ownerId = this.authService.getUserId();
  }

  loadData() {
    this.isLoading = true;
    this.isView = true;
    this.noteService.getNoteWithoutWarning(this.masterId)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        if (resp.status == 'success') {
          if (resp.data) {
            this.isLoadingEditor = true;
          }
          this.note = resp.data;
        }
        this.isLoading = false;
      },
        _ => this.isLoading = false
      )
  }

  back() {
    this.router.navigate([`/${this.Routing.NOTEBOOK.path}`]);
  }

  changeMode(isView: boolean) {
    this.isLoadingEditor = true;
    this.isView = isView;
    if (this.isView) {
      this.config = this.tinyEditorService.autoResizeConfig(() => this.isLoadingEditor = false);
    } else {
      this.config = this.tinyEditorService.getConfig(window.innerHeight - 96, () => this.isLoadingEditor = false);
    }
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
    if (this.isLoading) {
      return;
    }

    const validate = this.validateBeforeSave();
    if (!validate.isValid) {
      this.saveBtn.isFinished = true;
      SnackBar.warning(new SnackBarParameter(this, validate.message));
      return;
    }

    const type = ChangeNoteType.Title + ChangeNoteType.Content + ChangeNoteType.CategoryId + ChangeNoteType.IsPublic + ChangeNoteType.IsPinned;
    this.noteService.updateNote(this.note, type).pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.saveBtn.isFinished = true;
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["UPDATE_SUCCESS_MSG"], 2000));
            this.loadData();
          }
        },
        _ => this.saveBtn.isFinished = true
      );
  }
}
