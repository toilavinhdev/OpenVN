import { Injectable } from '@angular/core';
import { DeviceType } from '../../enumerations/device.enum';
import { DateHelper } from '../../helpers/date.helper';
import { ConfigService } from '../config/config.service';
import { HttpService } from './http.service';
import { TransferDataService } from './transfer-data.service';
import { HubConnectionService } from './hub-connection.service';
import { HubConnectionState } from '@microsoft/signalr';
import { Utility } from '../../utility/utility';


@Injectable({
  providedIn: 'root'
})
export class SharedService {

  static OnlineUsers = 0;

  static DeviceType = DeviceType.Desktop;

  private _previousScreen = "";

  get previousScreen() {
    return this._previousScreen;
  };

  private _lastVisitedScreen = "";

  get lastVisitedScreen() {
    return this._lastVisitedScreen;
  };

  set lastVisitedScreen(value: string) {
    this._previousScreen = this.lastVisitedScreen;
    this._lastVisitedScreen = value;
    this.transferService.changeScreenEvent.emit(value);
    console.customize(`last visited screen:`, this._lastVisitedScreen);
  }

  get currentLanguage(): string {
    return this.configService?.userConfig?.configValue?.language || 'vi-VN';
  }

  onlineUsersText() {
    switch (this.hubService.connection.state) {
      case HubConnectionState.Connected:
        return SharedService.OnlineUsers + " online";
      case HubConnectionState.Disconnected:
        return "Kết nối bị ngắt";
      case HubConnectionState.Reconnecting:
        return "Đang kết nối lại...";
      default:
        return "Unknown";
    }
  }


  constructor(
    private httpService: HttpService,
    private transferService: TransferDataService,
    private configService: ConfigService,
    private hubService: HubConnectionService
  ) {
  }
}
