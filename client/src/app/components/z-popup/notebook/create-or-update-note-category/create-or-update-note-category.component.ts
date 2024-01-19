import { AfterViewInit, Component, ElementRef, Inject, Injector, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { NoteCategory } from 'src/app/models/notebook/note-category';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { BaseButton } from 'src/app/shared/components/micro//button/button.component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { DeviceType } from 'src/app/shared/enumerations/device.enum';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { NoteCategoryService } from 'src/app/shared/services/notebook/note-category.service';

@Component({
  selector: 'app-create-or-update-note-category',
  templateUrl: './create-or-update-note-category.component.html',
  styleUrls: ['./create-or-update-note-category.component.scss']
})

export class CreateOrUpdateNoteCategoryComponent extends BaseComponent implements AfterViewInit {

  category = new NoteCategory();

  clone: NoteCategory = JSON.parse(JSON.stringify(this.category));

  categories: NoteCategory[] = [];

  isFetchingCategory = false;

  isFetchingNote = false;

  @ViewChild("name")
  name!: ElementRef;

  @ViewChild("saveBtn")
  saveBtn: BaseButton;

  constructor(
    public injector: Injector,
    public activatedRoute: ActivatedRoute,
    public router: Router,
    public sharedService: SharedService,
    public noteCategoryService: NoteCategoryService,
    public dialogRef: MatDialogRef<CreateOrUpdateNoteCategoryComponent>,
    public transferService: TransferDataService,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    super.ngOnInit();
  }

  ngAfterViewInit() {
    if (SharedService.DeviceType == DeviceType.Desktop) {
      this.name.nativeElement.focus();
    }
  }

  initData() {
    super.initData();
  }

  validateBeforeSave() {
    const result = {
      isValid: true,
      message: ''
    }
    if (this.category.name == '' || this.category.name.trim() == '') {
      result.isValid = false;
      result.message = TranslationService.VALUES['NOTEBOOK']['CATEGORY_NAME_NOT_EMPTY'];
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

    this.noteCategoryService.saveCategory(this.category).pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.saveBtn.isFinished = true;
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["SAVE_SUCCESS_MSG"], 2000));
            this.transferService.reloadListNotesEvent.emit();
            this.dialogRef.close();
          }
        },
        _ => this.saveBtn.isFinished = true
      );
  }

  onKeyup(event) {
    if (event.key == 'Enter') {
      this.saveBtn.clickExecute(event);
    }
  }
}

