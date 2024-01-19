import { HttpEventType } from '@angular/common/http';
import { Component, Inject, Injector, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable, forkJoin, throwError } from 'rxjs';
import { catchError, map, takeUntil } from 'rxjs/operators';
import { ServiceResult } from 'src/app/models/base/service-result';
import { Directory } from 'src/app/models/cloud/directory';
import { Message } from 'src/app/models/message';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { MessageBox } from 'src/app/shared/components/element/message-box/message-box.component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { BaseUploaderComponent } from 'src/app/shared/components/micro/uploader/uploader.component';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { CloudFileService } from 'src/app/shared/services/cloud/cloud-file.service';
import { Utility } from 'src/app/shared/utility/utility';

interface UploadEvent {
  id: number,
  ob: Observable<ServiceResult>,
  file: File, status: number,
  response?: ServiceResult,
  message?: string
}

@Component({
  selector: 'app-cloud-uploader',
  templateUrl: './cloud-uploader.component.html',
  styleUrls: ['./cloud-uploader.component.scss']
})
export class CloudUploaderComponent extends BaseComponent {

  successCount = 0;

  failedCount = 0;

  pendingCount = 0;

  events: UploadEvent[] = [];

  files: File[] = [];

  directory: Directory;

  @ViewChild("uploader")
  uploader: BaseUploaderComponent;

  constructor(
    public injector: Injector,
    public cloudFileService: CloudFileService,
    public dialogRef: MatDialogRef<CloudUploaderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super(injector);
    this.directory = data.dir;
  }

  upload2(files) {
    const file = files[0];
    const chunkSize = 100 * 1024 * 1024;
    const totalChunks = Math.ceil(file.size / chunkSize);

    // for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
    //   const start = chunkIndex * chunkSize;
    //   const end = Math.min(start + chunkSize, file.size);
    //   const chunk = file.slice(start, end);
    //   const formData = new FormData();
    // }

    const formData = new FormData();
    formData.append('files', file, file.name);
    this.cloudFileService
      .upload(this.directory.id, formData)
      .pipe(map((event: any) => {
        if (event.type == HttpEventType.UploadProgress) {
          console.customize(Math.round((100 / event.total) * event.loaded));
        } else if (event.type == HttpEventType.Response) {
          console.customize("null");
        }
      }))
      .subscribe(resp => console.customize(resp));
  }

  upload(files) {
    this.files = files;

    const overSize = this.files.find(x => x.size > this.cloudFileService.configuration.maxFileSize);
    if (overSize != null) {
      let message = TranslationService.VALUES['CLOUD']['MAX_FILE_SIZE_MSG']
        .replace('{0}', this.cloudFileService.configuration.maxFileSizeText)
        .replace('{1}', overSize.name);

      MessageBox.information(new Message(this, { content: message }));

      this.uploader.uploadBtn.isFinished = true;
      return;
    }

    this.isLoading = true;
    this.successCount = 0;
    this.failedCount = 0;
    this.pendingCount = this.events.length || files.length;

    if (!this.events.length) {
      (files as Array<File>).forEach((file, index) => {
        const formData = new FormData();
        formData.append('files', file, file.name);

        this.events.push({
          id: index,
          ob: this.cloudFileService.upload(this.directory.id, formData),
          file: file,
          status: 0
        });
      });
    }

    const batchSize = 3;
    const events = [...this.events];
    const remain = events.length;
    this.next(events, batchSize, remain);
  }

  next(events: UploadEvent[], batchSize: number, remain: number) {
    if (!events.length && this.successCount) {
      console.customize(`${this.successCount} files uploaded successfully!`);
      SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['CLOUD']['FILES_UPLOADED_SUCCESSFULLY'].replace('{0}', this.successCount), 5000));
      return;
    }

    const localEvents = events.splice(0, batchSize);
    const obs = localEvents.map(x => x.ob
      .pipe(
        takeUntil(this._onDestroySub),
        catchError(error => {
          x.status = 2;
          x.message = JSON.stringify(error);
          this.pendingCount--;
          this.failedCount++;
          return throwError(error);
        })
      )
    );
    forkJoin(obs)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resps => {
        resps.forEach((resp, idx) => {
          localEvents[idx].response = resp;
          this.pendingCount--;

          if (resp.status == 'success') {
            localEvents[idx].status = 1;
            this.successCount++;
            this.data.ref.shouldRefresh = true;
          } else {
            localEvents[idx].status = 2;
            localEvents[idx].message = resp.error.message;
            this.failedCount++;
          }

        });
        remain -= batchSize;
        console.customize(`uploaded ${localEvents.length} files, remain ${Math.max(0, remain)} files, process will continue after 0.3s...`);
        setTimeout(() => {
          this.next(events, batchSize, remain);
        }, 300);
      }
      );
  }

  retry() {
    this.events = this.events.filter(event => event.status == 2);
    this.events.forEach(event => event.message = '');
    this.upload(this.files);
  }
}
