import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { NotificationModel } from 'src/app/models/notification/notification';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { Routing } from 'src/app/shared/constants/common.constant';
import { NotificationType } from 'src/app/shared/enumerations/notification-type.enum';
import { DateHelper } from 'src/app/shared/helpers/date.helper';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { NotificationService } from 'src/app/shared/services/notification/notification.service';
import { Utility } from 'src/app/shared/utility/utility';
import { BaseComponent } from '../../base-component';
import { SnackBar } from '../../element/snackbar/snackbar.component';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss']
})
export class NotificationComponent extends BaseComponent {

  NotificationType = NotificationType;

  isLoadingNumberOfUnreadNotification = false;

  isLoadingNotification = false;

  notifications: NotificationModel[] = [];

  numberOfNotifications = 0;

  navbarOn = "all";

  selectedNotification = new NotificationModel();

  constructor(
    public injector: Injector,
    public router: Router,
    public sharedService: SharedService,
    public transferService: TransferDataService,
    public notificationService: NotificationService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.transferService.receivedNotificationEvent
    .pipe(takeUntil(this._onDestroySub))
    .subscribe(socketMessage => {
      SnackBar.warning(new SnackBarParameter(this, socketMessage.message, 3500, () => {
        this.router.navigateByUrl(`/${Routing.SIGN_IN_HISTORY.path}`);
      }));
      this.loadNotificationAndNumberUnread();
    });
  }

  initData() {
    super.initData();
    this.loadNumberOfUnreadNotification();
  }

  loadNumberOfUnreadNotification() {
    if (this.isLoadingNumberOfUnreadNotification) {
      return;
    }

    this.isLoadingNumberOfUnreadNotification = true;
    this.notificationService.getNumberOfUnreadNotification()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.isLoadingNumberOfUnreadNotification = false;
        if (resp.status == 'success') {
          this.numberOfNotifications = resp.data;
        }
      },
        _ => this.isLoadingNumberOfUnreadNotification = false
      )
  }

  loadNotifications() {
    if (this.isLoadingNotification) {
      return;
    }

    this.isLoadingNotification = true;
    this.paginationRequest.pageSize = 32;
    this.notificationService.getNotificationPaging(this.paginationRequest, this.navbarOn)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.isLoadingNotification = false;
        if (resp.status == 'success') {
          this.notifications = resp.data;
          this.notifications.forEach(notification => {
            notification.datetime = DateHelper.displayTimeAgo(notification.timestamp);
          })
        }
      },
        _ => this.isLoadingNotification = false
      )
  }

  loadNotificationAndNumberUnread() {
    this.loadNumberOfUnreadNotification();
    this.loadNotifications();
  }

  changeNavigator(value: string) {
    this.navbarOn = value;
    this.loadNotifications();
  }

  markAsRead(id: string, event) {
    this.stopEvent(event);
    this.notificationService.markAsRead(id)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        if (resp.status == 'success') {
          this.loadNotificationAndNumberUnread();
        }
      });
  }

  markAsUnread(id: string, event) {
    this.stopEvent(event);
    this.notificationService.markAsUnread(id)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        if (resp.status == 'success') {
          this.loadNotificationAndNumberUnread();
        }
      });
  }

  markAllAsRead(event) {
    this.stopEvent(event);
    this.notificationService.markAllAsRead()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        if (resp.status == 'success') {
          this.loadNotificationAndNumberUnread();
        }
      });
  }

  displayNotificationNumber() {
    if (this.numberOfNotifications < 100) {
      return this.numberOfNotifications;
    }
    return '99+';
  }

  deleteNotification(event) {
    Utility.featureIsInDevelopment(event);
  }

  redirect(path: string) {
    return this.router.navigateByUrl(`/${path}`);
  }

  stopEvent(event) {
    event.stopPropagation();
    event.preventDefault();
  }
}
