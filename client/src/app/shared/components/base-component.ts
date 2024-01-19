import { Directive, Injector, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subject } from "rxjs";
import { PaginationRequest } from "../../models/base/pagination-request";
import { Tracking } from "../../models/core/tracking";
import { ButtonColor, ButtonType, IconButtonType } from "../constants/button.constant";
import { Routing } from "../constants/common.constant";
import { Event } from "../constants/event";
import { DeviceType } from "../enumerations/device.enum";
import { ActionExponent } from "../enumerations/permission.enum";
import { TrackingService } from "../services/base/tracking.service";
import { TranslationService } from "../services/base/translation.service";
import { Utility } from "../utility/utility";
import { SharedService } from "../services/base/shared.service";

@Directive()
export class BaseComponent implements OnInit, OnDestroy {

  SharedService = SharedService;

  ButtonType = ButtonType;

  ButtonColor = ButtonColor;

  IconButtonType = IconButtonType;

  ActionExponent = ActionExponent;

  DeviceType = DeviceType;

  Routing = Routing;

  Utility = Utility;

  isLoading: boolean = false;

  paginationRequest = new PaginationRequest();

  public _onDestroySub: Subject<void> = new Subject<void>();
  public trackingService: TrackingService;
  public sharedService: SharedService;
  public activatedRoute: ActivatedRoute;
  public timerId: any;

  constructor(
    public injector: Injector
  ) { }

  ngOnInit() {
    this.initServices();
    this.initData();
    this.tracking();

    setTimeout(() => {
      const key = this.activatedRoute.snapshot.data['key'];
      Utility.changeTitle(TranslationService.VALUES['ROUTER'][key] || "Open VN");
    }, 100);
  }

  // unsubscribe khi destroy
  ngOnDestroy() {
    if (this._onDestroySub) {
      this._onDestroySub.next();
      this._onDestroySub.complete();
      this._onDestroySub.unsubscribe();
    }
  }

  initData() {
    return;
  }

  initServices() {
    this.trackingService = this.injector.get(TrackingService);
    this.sharedService = this.injector.get(SharedService);
    this.activatedRoute = this.injector.get(ActivatedRoute);
  }

  tracking(event?: Tracking, callback?: Function) {
    clearTimeout(this.timerId);

    this.trackingService.events.push(event || this.commonTrackingEvent(Event.CHANGE_PAGE));
    this.timerId = setTimeout(() => {
      this.trackingService.distinctEvents();
      for (let i = 0; i < this.trackingService.events.length; i++) {
        const event = this.trackingService.tracking(this.trackingService.events[i]);

        event.subscribe(_ => {
          if (callback) {
            callback(_);
          }
        });
      }
      this.trackingService.events = [];
    }, 256);
  }

  protected commonTrackingEvent(eventId: string) {
    const event = new Tracking();
    event.eventId = eventId;
    event.origin = window.location.origin;
    event.previousScreen = this.sharedService.previousScreen;
    event.currentScreen = this.sharedService.lastVisitedScreen;
    event.screenWidth = window.outerWidth;
    event.screenHeight = window.outerHeight;
    event.screenInnerWidth = window.innerWidth;
    event.screenInnerHeight = window.innerHeight;
    event.language = window.navigator.language;
    return event;
  };
}
