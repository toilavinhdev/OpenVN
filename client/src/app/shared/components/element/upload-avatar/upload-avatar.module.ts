import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UploadAvatarComponent } from './upload-avatar.component';
import { BaseUploaderModule } from '../../micro/uploader/uploader.module';

@NgModule({
  declarations: [UploadAvatarComponent],
  imports: [
    CommonModule,
    BaseUploaderModule
  ],
  exports: [UploadAvatarComponent]
})
export class UploadAvatarModule { }
