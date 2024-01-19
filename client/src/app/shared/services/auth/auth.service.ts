import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonConstant, Routing } from 'src/app/shared/constants/common.constant';
import { StringHelper } from 'src/app/shared/helpers/string.helper';
import { HttpService } from 'src/app/shared/services/base/http.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { environment } from 'src/environments/environment';
import { LocalStorageKey } from '../../constants/localstorage.key';
import { AuthStatus } from '../../enumerations/auth-status.enum';
import { RefreshTokenModel } from '../../../models/auth/requests/refresh-token-model';
import { UserCred } from '../../../models/auth/requests/user-cred';
import { AuthResponse } from '../../../models/auth/responses/auth-response';
import { ServiceResult } from '../../../models/base/service-result';
import { LocalHelper } from '../../helpers/local.helper';
import { PaginationRequest } from '../../../models/base/pagination-request';
import { BaseMessageResponse } from '../../../models/base/base-message-response';
import { ErrorModel } from 'src/app/models/base/error';
import { of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Utility } from '../../utility/utility';
import { CreateAccount } from 'src/app/models/auth/requests/create-account';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public auth_base_host = environment.base_host + "/" + environment.api_version;
  public serviceName = 'opensync';
  public controller = 'auth';

  /**
   * True nếu đang refresh token, otherwise false
   */
  public refreshing = false;
  public signOutCallback?: Function;

  constructor(
    public httpService: HttpService,
    public transfer: TransferDataService,
    public router: Router,
    public activatedRoute: ActivatedRoute,
  ) {
  }

  moveOut(hasNext?: boolean) {
    localStorage.removeItem('auth');
    let url = Routing.SIGN_IN.path;
    if (hasNext == undefined || hasNext == true) {
      console.customize("has next", hasNext)
      url += '?next=' + window.location.href;
    }
    return this.router.navigateByUrl(`/${url}`);
  }

  isSignedIn() {
    return this.getAuthStatus() === AuthStatus.SignedIn;
  }

  getAuth() {
    return LocalHelper.getAndParse('auth');
  }

  getTenantId() {
    return this.getAuth()[LocalStorageKey.TENANT_ID] || '';
  }

  getUserId() {
    return this.getAuth()[LocalStorageKey.USER_ID] || '';
  }

  getAccessToken() {
    return this.getAuth()[LocalStorageKey.ACCESS_TOKEN] || '';
  }

  getRefreshToken() {
    return this.getAuth()[LocalStorageKey.REFRESH_TOKEN] || '';
  }

  getExpiryTime() {
    return new Date(this.getAuth()[LocalStorageKey.EXP] * 1000).getTime();
  }

  setAuthStatus(status: AuthStatus) {
    const auth = this.getAuth();

    auth[LocalStorageKey.AUTH_STATUS] = status;
    localStorage.setItem('auth', JSON.stringify(auth));
  }

  removeAccessToken() {
    const auth = this.getAuth();

    auth[LocalStorageKey.ACCESS_TOKEN] = "";
    localStorage.setItem('auth', JSON.stringify(auth));
  }

  saveAuthenticate(accessToken: string, refreshToken: string) {
    const config = StringHelper.parseJwt(accessToken);
    config[LocalStorageKey.AUTH_STATUS] = 1;
    config[LocalStorageKey.ACCESS_TOKEN] = accessToken;
    config[LocalStorageKey.REFRESH_TOKEN] = refreshToken;

    localStorage.setItem('auth', JSON.stringify(config));
  }

  getAuthStatus(): AuthStatus {
    const status = this.getAuth()[LocalStorageKey.AUTH_STATUS];
    switch (status) {
      case 1:
        return AuthStatus.SignedIn;
      case 2:
        return AuthStatus.SignedOut;
      case 3:
        return AuthStatus.SigningOut;
      default:
        return AuthStatus.Unknown;
    }
  }

  ping() {
    const url = `${this.auth_base_host}/${this.serviceName}/${this.controller}/ping?uid=${this.getUserId()}&${CommonConstant.DISALLOW_NOTICE}`;
    return this.httpService.get<ServiceResult>(url);
  }

  signUp(account: CreateAccount) {
    const url = `${this.auth_base_host}/${this.serviceName}/${this.controller}/sign-up`;
    return this.httpService.post<ServiceResult>(url, account);
  }

  signIn(userCred: UserCred) {
    const url = `${this.auth_base_host}/${this.serviceName}/${this.controller}/sign-in?${CommonConstant.DISALLOW_NOTICE}`;
    return this.httpService.post<AuthResponse>(url, userCred);
  }

  signOut(callback?: Function) {
    this.signOutCallback = callback;
    this.router.navigate([`/${Routing.SIGN_OUT.path}`]);
  }

  refreshToken() {
    const model = new RefreshTokenModel();
    model.userId = this.getUserId() || "";
    model.refreshToken = this.getRefreshToken();
    if (model.userId.isEmpty() || model.refreshToken.isEmpty()) {
      const response = new AuthResponse();
      response.status = "unauthorized";
      response.error = new ErrorModel();
      return of(response);
    }

    this.refreshing = true;
    const url = `${this.auth_base_host}/${this.serviceName}/${this.controller}/refresh-token?${CommonConstant.DISALLOW_NOTICE}`;
    return this.httpService
      .post<AuthResponse>(url, model)
      .pipe(catchError(error => {
        this.refreshing = false;
        return throwError(error);
      }));
  }

  getSignInLoggingPaging(paginationRequest: PaginationRequest) {
    const page = paginationRequest.pageIndex;
    const size = paginationRequest.pageSize;

    const url = `${this.auth_base_host}/${this.serviceName}/${this.controller}/history-paging?page=${page}&size=${size}`;
    return this.httpService.get<ServiceResult>(url);
  }

  verifySecretKey(secretKey: string, url: string) {
    return this.httpService.get<ServiceResult>(url + `?secretKey=${btoa(secretKey)}`);
  }

  getRequestInformation() {
    const url = `${this.auth_base_host}/${this.serviceName}/${this.controller}/request-information`;
    return this.httpService.get<ServiceResult>(url);
  }
}
