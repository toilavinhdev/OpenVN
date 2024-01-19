import { Injectable } from '@angular/core';
import { CommonConstant } from 'src/app/shared/constants/common.constant';
import { HttpService } from 'src/app/shared/services/base/http.service';
import { environment } from 'src/environments/environment';
import { ServiceResult } from '../../../models/base/service-result';
import { SharedService } from './shared.service';
import { Tracking } from '../../../models/core/tracking';

@Injectable({
  providedIn: 'root'
})
export class TrackingService {

  public tracking_base_host = `${environment.tracking_base_host}`;
  public events: Tracking[] = [];

  constructor(
    public httpService: HttpService,
    public sharedService: SharedService
  ) {
  }

  tracking(payload: Tracking) {
    const url = `${this.tracking_base_host}/tracking-dynamic?${CommonConstant.DISALLOW_NOTICE}`;
    return this.httpService.post<ServiceResult>(url, payload);
  }

  distinctEvents() {
    for (let i = 0; i < this.events.length; i++) {
      let event = this.events[i];
      let matched = false;

      for (let j = i + 1; j < this.events.length; j++) {
        let ev = this.events[j];
        if (ev.eventId == event.eventId && ev.data == event.data && ev.currentScreen == event.currentScreen) {
          matched = true;
          break;
        }
      }

      if (matched) {
        this.events.splice(i--, 1);
      }
    }
  }
}
