import {
  HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse, HttpStatusCode
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { ServiceResult } from 'src/app/models/base/service-result';
import { AuthResponse } from '../../models/auth/responses/auth-response';
import { ErrorModel } from '../../models/base/error';
import { Mark } from '../../models/core/mark';
import { NotificationType } from '../../models/core/notify-type.enum';
import { Message } from '../../models/message';
import { SnackBarParameter } from '../../models/snackbar.param';
import { HeaderNames } from '../constants/header-names.constant';
import { AuthStatus } from '../enumerations/auth-status.enum';
import { StringHelper } from '../helpers/string.helper';
import { AuthService } from '../services/auth/auth.service';
import { SharedService } from '../services/base/shared.service';
import { TranslationService } from '../services/base/translation.service';
import { ConfigService } from '../services/config/config.service';
import { RequestErrorMapping } from './request-error-handler/request-mapping-handler';
import { MessageBox } from '../components/element/message-box/message-box.component';
import { SnackBar } from '../components/element/snackbar/snackbar.component';

@Injectable()
export class RequestHandlingInterceptor implements HttpInterceptor {

  private withoutTokens: string[] = [];
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    private authService: AuthService,
    private sharedService: SharedService,
    private configService: ConfigService
    // private translationService: TranslationService,
  ) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    request = this.injectToken(request);

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === HttpStatusCode.TooManyRequests) {
          MessageBox.information(new Message(this, { content: "SPAM DETECTED!" }));
        } else {
          this.fireNotify(Mark.getMark(request.url), null, null);
        }
        return throwError(error.error);
      }),
      switchMap(response => this.checkErrorResponse(request, next, response)),
      switchMap(response => this.checkMessageResponse(response))
    );
  }

  culture() {
    let result = this.configService.userConfig?.configValue?.language || window.navigator.language;
    switch (result) {
      case "vi":
        return "vi-VN";
      case "en":
        return "en-US";
      default:
        return result;
    }
  }

  injectToken(request: HttpRequest<unknown>) {
    if (this.withoutTokens.includes(request.url))
      return request;

    const header = {
      // 'Content-Type': 'application/json; charset=utf-8',
      // 'Accept': 'application/json',
      'Accept': '*/*',
      'Authorization': `Bearer ${this.authService.getAccessToken()}`,
    };

    header['Accept-Language'] = this.culture();
    header[HeaderNames.TENANT_ID] = this.authService.getTenantId();

    return request.clone({
      setHeaders: header
    });
  }

  handleUnauthorized(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // 1 vài request ko refresh token
    if (request.url.includes("sign-out")) {
      return throwError("");
    }

    // Nếu đang refresh thì request khác đợi
    if (this.authService.refreshing) {
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(() => {
          return next.handle(this.injectToken(request));
        })
      );
    }

    return this.authService.refreshToken().pipe(
      switchMap(response => {
        this.authService.refreshing = false;
        // nếu refresh token ko hợp lệ thì throw
        if (response.status != 'success') {
          const currentStatus = this.authService.getAuthStatus();
          if (currentStatus == AuthStatus.SignedOut || currentStatus == AuthStatus.Unknown) {
            this.authService.moveOut(false);
          } else {
            this.authService.signOut((response: AuthResponse) => {
              if (currentStatus === AuthStatus.SignedIn) {
                SnackBar.danger(new SnackBarParameter(null, TranslationService.VALUES['ERROR']['SESSION_EXPRIED'], 2000));
              }
              return this.authService.moveOut(false);
            });
          }
          return throwError(response.error.message);
        }

        this.authService.saveAuthenticate(response.accessToken, response.refreshToken);
        this.refreshTokenSubject.next(response.accessToken);
        return next.handle(this.injectToken(request));
      })
      // catchError((e: HttpErrorResponse) => {
      //   this.authService.refreshing = false;
      //   return throwError(e.error);
      // })
    )

  }

  /**
   * Kiểm tra response lỗi
   * @param request
   * @param next
   * @param response
   * @returns
   */
  checkErrorResponse(request: HttpRequest<unknown>, next: HttpHandler, response: HttpEvent<unknown>) {
    try {
      if (response.type != 0) {
        response = response as HttpResponse<unknown>;
        const result = response.body as ServiceResult;
        if (result && result.status != 'success') {
          switch (result.error?.code) {
            case HttpStatusCode.Unauthorized:
              return this.handleUnauthorized(request, next);

            case HttpStatusCode.Forbidden:
              this.fireNotify(Mark.getMark(response.url), result.error, result.data);
              // SnackBar.openSnackBarDanger(new SnackBarParameter(null, PerrmisionConstant.NOT_PERMISSION, ''));
              break;

            case HttpStatusCode.ServiceUnavailable:
              MessageBox.information(new Message(this, { content: "This service is currently under maintenance. Please try again in a few minutes!" }));
              break;
          }
        }
      }
    } catch (error) {
      console.customize(``, error);
    }
    return of(response);
  }

  /**
   * Fire event notify nếu có trước khi transfer response
   * @param response
   */
  checkMessageResponse(response: HttpEvent<unknown>) {
    try {
      response = response as HttpResponse<unknown>;
      const result = response.body as ServiceResult;

      if (result && result.status == "error" && ![304, 307, 308, 403, 429, 503].includes(result.error?.code)) {
        this.fireNotify(Mark.getMark(response.url), result.error, result.data);
      }
    } catch (error) {
      console.customize(``, error);
    }
    return of(response);
  }

  fireNotify(mark: Mark, error: ErrorModel, body: any) {
    if (mark.allowNotice) {
      if (error && !StringHelper.isNullOrEmpty(error.type)) {
        const handler = RequestErrorMapping.mapping.find(m => m.type == error.type);
        if (handler) {
          handler.func(error, body);
          return;
        }
      }

      const message = !StringHelper.isNullOrEmpty(error?.message) ? error?.message : TranslationService.VALUES['ERROR']['UNKNOWN_MSG'];
      switch (mark.notificationType) {
        case NotificationType.MessageBox:
          MessageBox.information(new Message(null, { content: message }));
          break;
        case NotificationType.SnackBarWarning:
          SnackBar.warning(new SnackBarParameter(null, message));
          break;
        default:
          SnackBar.danger(new SnackBarParameter(null, message));
          break;
      }
    }
  }
}
