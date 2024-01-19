import { Component, OnInit } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { SnackBar } from '../snackbar/snackbar.component';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { UserService } from 'src/app/shared/services/user/user.serivce';
import { TranslationService } from 'src/app/shared/services/base/translation.service';

@Component({
  selector: 'app-upload-avatar',
  templateUrl: './upload-avatar.component.html',
  styleUrls: ['./upload-avatar.component.scss']
})
export class UploadAvatarComponent implements OnInit {

  imageExtensions = ".apng,.avif,.gif,.jpg,.jpeg,.jfif,.pjpeg,.pjp,.png,.svg,.webp";

  constructor(
    public userSerivce: UserService,
    public transferService: TransferDataService
  ) { }

  ngOnInit(): void {
  }

  upload(event) {
    const formData = new FormData();
    formData.append('avatar', event[0], event[0].name);

    this.userSerivce.setAvatar(formData)
      .subscribe(resp => {
        if (resp.status == 'success') {
          this.transferService.updateAvatarEvent.emit();
          SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['USER']['UPDATE_AVATAR_SUCCESS_MSG']))
        }
      })
  }
}
