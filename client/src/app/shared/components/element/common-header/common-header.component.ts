import { Component, Injector, Input, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { App } from 'src/app/models/app/app';
import { Message } from 'src/app/models/message';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { Routing } from 'src/app/shared/constants/common.constant';
import { Event } from 'src/app/shared/constants/event';
import { DateHelper } from 'src/app/shared/helpers/date.helper';
import { AppService } from 'src/app/shared/services/app/app.service';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { PopupService } from 'src/app/shared/services/base/popup.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { ConfigService } from 'src/app/shared/services/config/config.service';
import { NotificationService } from 'src/app/shared/services/notification/notification.service';
import { UserService } from 'src/app/shared/services/user/user.serivce';
import { Utility } from 'src/app/shared/utility/utility';
import { BaseButton } from '../../micro/button/button.component';
import { MessageBox } from '../message-box/message-box.component';
import { SnackBar } from '../snackbar/snackbar.component';
import { UploadAvatarComponent } from '../upload-avatar/upload-avatar.component';

@Component({
  selector: 'app-common-header',
  templateUrl: './common-header.component.html',
  styleUrls: ['./common-header.component.scss']
})
export class CommonHeaderComponent extends BaseComponent {
  Utility = Utility;

  DateHelper = DateHelper;

  SharedService = SharedService;

  searchWidth = Math.max(200, Math.min(window.innerWidth * 0.3, 480));

  isOpened = false;

  apps: App[] = [];

  @Input()
  isShowOnlineUser = true;

  @ViewChild("header")
  instance: CommonHeaderComponent;

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

  clickAppInstance(event) {
    event.stopPropagation();
    event.preventDefault();
  }

  redirect(path: string) {
    return this.router.navigateByUrl(`/${path}`);
  }

  stopEvent(event) {
    event.stopPropagation();
    event.preventDefault();
  }

}
