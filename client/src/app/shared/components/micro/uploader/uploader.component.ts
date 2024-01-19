import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { NgxDropzoneComponent } from 'ngx-dropzone';
import { Message } from 'src/app/models/message';
import { Utility } from 'src/app/shared/utility/utility';
import { BaseButton } from '../button/button.component';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { MessageBox } from '../../element/message-box/message-box.component';
import { DeviceType } from 'src/app/shared/enumerations/device.enum';


@Component({
  selector: 'uploader',
  templateUrl: './uploader.component.html',
  styleUrls: ['./uploader.component.scss']
})
export class BaseUploaderComponent {

  formData = new FormData();

  maxFileSize = 1024 * 1024 * 50;

  @Input()
  allowedFileExtensions = Utility.videoExtensions.map(i => `.${i}`).concat(Utility.imageExtensions.map(i => `.${i}`)).join(",");

  @Input()
  disabled = false;

  @Input()
  multiple = true;

  @Input()
  uploadUrl = '';

  @Input()
  emitAutomatically = false;

  @Input()
  showUploadButton = true;

  @Input()
  files: File[] = [];

  @Output()
  onUpload = new EventEmitter();

  @ViewChild("uploadBtn")
  uploadBtn!: BaseButton;

  @ViewChild("dropzone")
  dropzone!: NgxDropzoneComponent;

  constructor() {
  }

  onSelect(event: any) {
    if (!this.multiple) {
      this.files = [];
      this.files.push(...event.addedFiles);
    } else {
      this.files.push(...event.addedFiles);
    }

    // Tạm xử lý case mobile không hiện ảnh
    if (SharedService.DeviceType === DeviceType.Mobile) {
      MessageBox.information(new Message(this, {content: ""}));
      MessageBox.close();
    }

    if (this.emitAutomatically) {
      this.uploadBtn?.clickExecute(null);
    }
  }

  onRemove(event: any) {
    this.files.splice(this.files.indexOf(event), 1);
  }

  upload() {
    this.onUpload.emit(this.files);
  }
}
