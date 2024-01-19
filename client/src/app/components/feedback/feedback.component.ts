import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { HubConnectionState } from '@microsoft/signalr';
import { takeUntil } from 'rxjs/operators';
import { Feedback } from 'src/app/models/feedback/feedback';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { DeviceType } from 'src/app/shared/enumerations/device.enum';
import { DateHelper } from 'src/app/shared/helpers/date.helper';
import { HubConnectionService } from 'src/app/shared/services/base/hub-connection.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { FeedbackService } from 'src/app/shared/services/feedback/feedback.service';

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.scss'],
  // changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FeedbackComponent extends BaseComponent {

  feedbacks: Feedback[] = [];

  feedback = new Feedback();

  isSending = false;

  isShift = false;

  timer: any;

  someoneTyping = false;

  hasNewFeedback = false;

  @ViewChild("chatInstance")
  chatInstance: any;

  @ViewChild("virtualScroll")
  virtualScroll: CdkVirtualScrollViewport;

  constructor(
    public injector: Injector,
    public feedbackService: FeedbackService,
    public transferService: TransferDataService,
    public hubService: HubConnectionService,
    // public cdr: ChangeDetectorRef
  ) {
    super(injector);
  }

  initData() {
    super.initData();
    this.init();

    if (SharedService.DeviceType == DeviceType.Desktop) {
      setTimeout(() => {
        SnackBar.success(new SnackBarParameter(this, "Tôi trân quý mọi góp ý của các bạn 💖💖💖", 2000));
      }, 1500);
    }
  }

  init() {
    this.registerNewFeedback();
    this.registerSomeOnTyping();
    this.loadData();
    this.softRefreshData();
  }

  loadData() {
    if (this.isLoading) {
      return;
    }

    this.isLoading = true;
    this.feedbackService
      .get()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoading = false;
          if (resp.status == 'success') {
            this.feedbacks = resp.data;
            this.feedbacks.forEach(x => x.timeFormat = DateHelper.displayTimeAgo(x.createdDate));
            // this.cdr.detectChanges();
            setTimeout(() => {
              this.chatInstance.nativeElement.focus();
              this.scrollToBottom();
              // this.cdr.detectChanges();
            }, 200);
          }
        },
        _ => {
          this.isLoading = false;
          // this.cdr.detectChanges();
        }
      )
  }

  softRefreshData() {
    setInterval(() => {
      this.feedbacks.forEach(x => x.timeFormat = DateHelper.displayTimeAgo(x.createdDate));
    }, 6000);
  }

  scrollToBottom() {
    const element = this.virtualScroll.elementRef.nativeElement;
    element.scrollTo(0, element.scrollHeight);
  }

  typing(event) {
    if (event.key == 'Enter') {
      this.send();
    } else {
      if (this.feedback.content.length) {
        this.hubService.sendTyping();
      }
    }
  }

  send() {
    if (this.isSending || this.feedback.content.trim() == '') {
      this.chatInstance.nativeElement.focus();
      return;
    }

    this.isSending = true;
    this.feedbackService.send(this.feedback)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isSending = false;
          if (resp.status == 'success') {
            this.feedback = new Feedback();
            this.chatInstance.nativeElement.focus();
            // this.cdr.detectChanges();
          }
        },
        _ => this.isSending = false
      );
  }

  registerNewFeedback() {
    this.transferService.receivedNewFeedbackEvent
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        const newFb = JSON.parse(resp.message);
        newFb.timeFormat = DateHelper.displayTimeAgo(new Date());
        this.feedbacks = this.feedbacks.concat([newFb]);
        this.hasNewFeedback = true;

        setTimeout(() => {
          this.hasNewFeedback = false;
        }, 4000);
      })
  }

  registerSomeOnTyping() {
    this.transferService.receivedSomeOnTypeingEvent
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(_ => {
        this.someoneTyping = true;
        clearTimeout(this.timer);

        this.timer = setTimeout(() => {
          this.someoneTyping = false;
        }, 3000);
      })
  }

  viewLastFeedback() {
    this.scrollToBottom();
    this.hasNewFeedback = false;
  }
}
