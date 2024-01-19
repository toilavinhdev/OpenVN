import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { NotificationService } from 'src/app/shared/services/notification/notification.service';

@Component({
  selector: 'app-online-user',
  templateUrl: './online-user.component.html',
  styleUrls: ['./online-user.component.scss']
})
export class OnlineUserComponent implements OnInit {

  tooltip = "loading...";

  constructor(
    public sharedService: SharedService,
    public notificationService: NotificationService
  ) { }

  ngOnInit(): void {
  }

  viewDetail() {
    this.notificationService.getOnlineDetail()
      .subscribe(resp => {
        if (resp.status == 'success') {
          this.tooltip = `${resp.data.identity} ${resp.data.identity > 1 ? 'users' : 'user'}, ${resp.data.anonymous} khách`;
        }
      })
  }
}
