import { Component, Inject, Injector, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ButtonColor } from 'src/app/shared/constants/button.constant';
import { ErrorMessageConstant } from 'src/app/shared/constants/common.constant';
import { Message } from 'src/app/models/message';
import { BaseService } from 'src/app/shared/services/base/base.service';
import { BaseComponent } from '../../base-component';
import { ExportType } from 'src/app/shared/enumerations/common.enum';
import { ExcelService } from 'src/app/shared/services/base/excel.service';
import { MpListDynamicComponent } from '../mp-list-dynamic.component';
import { takeUntil } from 'rxjs/operators';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { BaseButton } from '../../button/button.component';

@Component({
  selector: 'app-popup-choose-export',
  templateUrl: './popup-choose-export.component.html',
  styleUrls: ['./popup-choose-export.component.scss']
})
export class PopupChooseExportComponent extends BaseComponent {
  ButtonColor = ButtonColor;

  ExportType = ExportType;

  exportTypeSelected = ExportType.OnScreen;

  infoExportOnScreen = "The system will export all records in the list. Once done, click \"Download\" to download the export file."

  infoExportAll = "The system will export all data. Once done, click \"Download\" to download the export file."

  isShowMessage = false;

  isDone = false;

  message = "Exporting, please wait...";

  downloadSuccess = false;

  key = "";

  serviceName = "";

  @ViewChild("exportBtn")
  exportBtn!: BaseButton;

  constructor(
    public injector: Injector,
    public excelService: ExcelService,
    @Inject(MAT_DIALOG_DATA) public data: Message,
  ) {
    super(injector);
  }

  ngOnInit(): void {
      super.ngOnInit();
      this.serviceName = this.data.data['service'];
  }

  /**
   * Cập nhật loại xuất khẩu đc chọn
   */
  updateExportTypeSelected(e: any) {
    this.exportTypeSelected = parseInt(e.value);
  }


  export() {
    this.isShowMessage = true;
    if (this.exportTypeSelected == ExportType.All) {
      this.paginationRequest.pageSize = 1000;
    }

    const module = (this.data.sender as MpListDynamicComponent).options.controller;
    this.excelService.serviceName = this.serviceName;
    this.excelService.getExportKey(module, this.paginationRequest)
    .pipe(takeUntil(this._onDestroySub))
    .subscribe(
      response => {
        if (response.status == "success") {
          this.isDone = true;
          this.message = "All ready, download now...";
          this.key = response.data;
        } else {
          this.message = TranslationService.VALUES['ERROR']['UNKNOWN_MSG'];
          this.exportBtn.isFinished = true;
        }
      },
      error => {
        this.exportBtn.isFinished = true;
        this.isShowMessage = false;
      }
    )
  }

  download() {
    const module = (this.data.sender as MpListDynamicComponent).options.controller;
    window.location.href = `${this.excelService.getBaseHost()}/${this.serviceName}/excel/${module}/export?key=${this.key}`;
    this.downloadSuccess = true;

    setTimeout(() => {
      this.data.callback();
    }, 1000);
  }
}
