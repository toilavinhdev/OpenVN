import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../base-component';
import { RequestInformation } from 'src/app/models/base/request-information';
import { Router } from '@angular/router';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { ConfigService } from 'src/app/shared/services/config/config.service';
import { UserService } from 'src/app/shared/services/user/user.serivce';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { PopupService } from 'src/app/shared/services/base/popup.service';
import { AppService } from 'src/app/shared/services/app/app.service';
import { NotificationService } from 'src/app/shared/services/notification/notification.service';
import { takeUntil } from 'rxjs/operators';
import { MessageBox } from '../../element/message-box/message-box.component';
import { Message } from 'src/app/models/message';
import { SnackBar } from '../../element/snackbar/snackbar.component';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { Routing } from 'src/app/shared/constants/common.constant';
import { UploadAvatarComponent } from '../../element/upload-avatar/upload-avatar.component';
import { Event } from 'src/app/shared/constants/event';
import { BaseButton } from '../button/button.component';

@Component({
  selector: 'app-user-account-box',
  templateUrl: './user-account-box.component.html',
  styleUrls: ['./user-account-box.component.scss']
})
export class UserAccountBoxComponent extends BaseComponent{

  fullName = "";

  shortName = "";

  ipInformation = new RequestInformation();

  isLoadingAvatar = false;

  isLoadingInformation = false;

  isLoadingIp = false;

  isNoAvatar = false;

  avatarUrl = "";

  ref: MatDialogRef<UploadAvatarComponent>;

  @ViewChild("signoutBtn")
  signoutBtn!: BaseButton;

  constructor(
    public injector: Injector,
    public router: Router,
    public sharedService: SharedService,
    public translationService: TranslationService,
    public transferService: TransferDataService,
    public authService: AuthService,
    public configService: ConfigService,
    public userService: UserService,
    public dialog: MatDialog,
    public popupService: PopupService,
    public appService: AppService,
    public notificationService: NotificationService
  ) {
    super(injector);
  }

  initData(): void {
    super.initData();
    this.loadAvatar();

    this.transferService.updateAvatarEvent
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.ref?.close();
        this.loadAvatar();
      });
  }

  openMenu() {
    this.loadIp();
    this.loadInformation();
  }

  loadIp() {
    if (this.ipInformation.ip != '' || this.isLoadingIp) {
      return;
    }

    this.isLoadingIp = true;
    this.authService
      .getRequestInformation()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoadingIp = false;
          if (resp.status == 'success') {
            this.ipInformation = resp.data;
          }
        },
        _ => this.isLoadingIp = false
      )
  }

  loadInformation() {
    if (this.userService.user || this.isLoadingInformation) {
      return;
    }

    this.isLoadingInformation = true;
    this.userService.getInformation()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoadingInformation = false;
          if (resp.status == 'success' && resp.data) {
            this.userService.user = resp.data;
            this.fullName = this.userService.user.firstName + " " + this.userService.user.lastName;
          }
        },
        _ => this.isLoadingInformation = false
      )
  }

  loadAvatar() {
    this.isLoadingAvatar = true;
    this.userService.getAvatar()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoadingAvatar = false;
          if (resp.status == 'success' && resp.data) {
            this.isNoAvatar = false;
            this.avatarUrl = resp.data;
          } else {
            this.setDefaultAvatar();
          }
        },
        _ => this.isLoadingAvatar = false
      )
  }

  uploadAvatar(event) {
    event.stopPropagation();
    event.preventDefault();

    const config = this.popupService.maxPingConfig(640, 420);
    config.position = { top: '50px' };
    config.panelClass = 'upload-avatar-popup';

    this.ref = this.dialog.open(UploadAvatarComponent, config);
  }

  setDefaultAvatar() {
    this.isNoAvatar = true;
    this.shortName = this.authService.getAuth()['username'][0];
  }

  removeAvatar(event) {
    event.stopPropagation();
    event.preventDefault();
    MessageBox.confirm(new Message(this, { content: TranslationService.VALUES['USER']['REMOVE_AVATAR_CONFIRM_MSG'] }, () => {
      this.userService.removeAvatar()
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(
          resp => {
            if (resp.status == 'success') {
              SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['USER']['REMOVE_AVATAR_SUCCESS_MSG']));
              this.setDefaultAvatar();
            }
          }
        )
    }));
  }

  signOut() {
    this.tracking(this.commonTrackingEvent(Event.CLICK_SIGN_OUT_BUTTON));
    this.authService.signOut(() => {
      this.signoutBtn.isFinished = true;
      this.authService.moveOut(false);
    });
  }

  feedback(event) {
    this.stopEvent(event);
    this.router.navigateByUrl(`/${Routing.FEEDBACK.path}`);
  }

  stopEvent(event) {
    event.stopPropagation();
    event.preventDefault();
  }
}
