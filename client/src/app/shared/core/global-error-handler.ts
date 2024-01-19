import { ErrorHandler, Injectable, NgZone } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from 'src/app/models/message';
import { SharedService } from '../services/base/shared.service';
import { DeviceType } from '../enumerations/device.enum';
import { MessageBox } from '../components/element/message-box/message-box.component';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(
    private zone: NgZone,
  ) { }

  handleError(error: any) {
    // Check if it's an error from an HTTP response
    // if (!(error instanceof HttpErrorResponse)) {
    //     error = error.rejection; // get the error object
    // }
    if (error) {
      if (!environment.production) {
        this.zone.run(() => {
          const msg = error ? error.message ? error.message : error : '';
          if(SharedService.DeviceType == DeviceType.Mobile) {
            MessageBox.information(new Message(null, { content: `[Fix this error]: ${msg}` }))
          }
        });
      }
      console.error('Error from global error handler', error);
    }
  }
}
