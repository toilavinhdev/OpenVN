import { Component, Injector } from '@angular/core';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { NotificationService } from 'src/app/shared/services/notification/notification.service';
import { BaseComponent } from '../../base-component';


@Component({
  selector: 'app-messenger',
  templateUrl: './messenger.component.html',
  styleUrls: ['./messenger.component.scss']
})
export class MessengerComponent extends BaseComponent {

  constructor(
    public injector: Injector,
    public sharedService: SharedService,
    public transferService: TransferDataService,
    public notificationService: NotificationService
  ) {
    super(injector);
  }

}
