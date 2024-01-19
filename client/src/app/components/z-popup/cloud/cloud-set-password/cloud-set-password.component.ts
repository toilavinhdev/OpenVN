import { Component, INJECTOR, Inject, Injector, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { takeUntil } from 'rxjs/operators';
import { CloudComponent } from 'src/app/components/cloud/cloud.component';
import { Directory } from 'src/app/models/cloud/directory';
import { Tracking } from 'src/app/models/core/tracking';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { BaseButton } from 'src/app/shared/components/micro/button/button.component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { AuthCloudService } from 'src/app/shared/services/cloud/auth-cloud.service';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-cloud-set-password',
  templateUrl: './cloud-set-password.component.html',
  styleUrls: ['./cloud-set-password.component.scss']
})
export class CloudSetPasswordComponent extends BaseComponent {

  Utility = Utility;

  directory: Directory;

  password = "";

  className = "show";

  mode = "password";

  @ViewChild("nextBtn")
  nextBtn: BaseButton;

  constructor(
    public injector: Injector,
    public authCloudService: AuthCloudService,
    public dialogRef: MatDialogRef<CloudSetPasswordComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super(injector);
    this.directory = data.dir;
  }

  tracking(event?: Tracking, callback?: Function): void {
  }

  toggle() {
    if (this.className == 'show') {
      this.className = 'hide';
      this.mode = 'text';
    } else {
      this.className = 'show';
      this.mode = 'password';
    }
  }

  setPassword() {
    if (this.password == '') {
      this.nextBtn.isFinished = true;
      return;
    }
    this.authCloudService
      .setPassword(this.directory.id, this.password)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["CLOUD"]["DIRECTORY"]["SET_PASSWORD_SUCCESS_MSG"]));
            this.dialogRef.close();
            // (this.data.ref as DirectoryComponent).loadData();
            this.directory.isLocked = true;
          }
        },
        _ => this.nextBtn.isFinished = true
      );
  }

  execute(event) {
    this.nextBtn.clickExecute(event);
  }
}
