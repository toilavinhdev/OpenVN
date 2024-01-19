import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { AuthResponse } from 'src/app/models/auth/responses/auth-response';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { LocalStorageKey } from 'src/app/shared/constants/localstorage.key';
import { SessionStorageKey } from 'src/app/shared/constants/sessionstorage.key';
import { AuthStatus } from 'src/app/shared/enumerations/auth-status.enum';
import { DateHelper } from 'src/app/shared/helpers/date.helper';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { HubConnectionService } from 'src/app/shared/services/base/hub-connection.service';
import { ConfigService } from 'src/app/shared/services/config/config.service';
import { UserService } from 'src/app/shared/services/user/user.serivce';
import { Utility } from 'src/app/shared/utility/utility';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-sign-out',
  templateUrl: './sign-out.component.html',
  styleUrls: ['./sign-out.component.scss']
})
export class SignOutComponent extends BaseComponent {

  private clearListSession = [
    SessionStorageKey.SIDEBAR_INDEX,
    SessionStorageKey.SECRET_CODE,
  ]

  private clearListLocal = [
    LocalStorageKey.UNLOCKED_DIR
  ]

  constructor(
    public injector: Injector,
    public authService: AuthService,
    public configService: ConfigService,
    public hubService: HubConnectionService,
    public router: Router,
    public userService: UserService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    console.customize(`signing out...`);
    Utility.changeTitle("signing out...");
    this.onSignOut();
  }

  onSignOut() {
    const url = `${this.authService.auth_base_host}/${this.authService.serviceName}/${this.authService.controller}/sign-out?uid=${this.authService.getUserId()}`;
    this.authService.setAuthStatus(AuthStatus.SigningOut);
    this.configService.userConfig = null;
    this.userService.user = null;
    this.authService.httpService.get<AuthResponse>(url).pipe(takeUntil(this._onDestroySub)).subscribe(
      response => {
        localStorage.removeItem('auth');
        this.clearListSession.forEach(item => sessionStorage.removeItem(`${environment.organization}_${item}`));
        this.clearListLocal.forEach(item => localStorage.removeItem(item));

        this.configService.userConfig = null;
        this.hubService.closeHub(() => {
          this.hubService.connectHub(() => this.hubService.isAuthenticated = false);
          setTimeout(() => SnackBar.close(), 40);
        });

        if (this.authService.signOutCallback) {
          this.authService.signOutCallback(response);
        } else {
          this.authService.moveOut(false);
        }
      },
      err => {
        if (this.authService.signOutCallback) {
          this.authService.signOutCallback(err);
        }
        else {
          this.authService.moveOut(false);
        }
      }
    );
  }
}
