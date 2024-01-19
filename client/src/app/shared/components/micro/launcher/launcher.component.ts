import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { BaseComponent } from '../../base-component';
import { App } from 'src/app/models/app/app';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { AppService } from 'src/app/shared/services/app/app.service';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { SnackBar } from '../../element/snackbar/snackbar.component';
import { CommonRedirect } from 'src/app/shared/constants/common.constant';


@Component({
  selector: 'app-launcher',
  templateUrl: './launcher.component.html',
  styleUrls: ['./launcher.component.scss']
})
export class LauncherComponent extends BaseComponent {
  isOpened = false;

  apps: App[] = [];

  @Output("clickEvent")
  clickEvent = new EventEmitter();

  constructor(
    public injector: Injector,
    public transferService: TransferDataService,
    public authService: AuthService,
    public appService: AppService,
    public router: Router
  ) {
    super(injector);
  }

  ngOnInit() {
    super.ngOnInit();
    this.onClickOutside();
  }

  click(event) {
    this.clickEvent.emit(event);
  }

  openApps(event) {
    event.stopPropagation();
    event.preventDefault();

    this.isOpened = true;
    setTimeout(() => {
      (document.querySelector('.apps') as any).style.height = '84px';

      if (!this.apps || !this.apps.length) {
        this.isLoading = true;
        this.apps = [];
        this.appService.getApp()
          .pipe(takeUntil(this._onDestroySub))
          .subscribe(resp => {
            this.isLoading = false;
            if (resp.status == 'success') {
              this.apps = resp.data.filter(x => x.isRelease);
            }
          },
            _ => this.isLoading = false
          )
      }
    }, 50);
  }

  viewApp(app, event) {
    if (app.requiredAuth) {
      window.location.href = app.host + '/token?t=' + this.authService.getAccessToken() + '&r=' + this.authService.getRefreshToken() + '&n=' + app.redirectUrl;
    } else {
      window.location.href = app.redirectUrl;
    }
  }

  viewAppUrl(app) {
    if (app.requiredAuth) {
      return app.host + '/token?t=' + this.authService.getAccessToken() + '&r=' + this.authService.getRefreshToken() + '&n=' + app.redirectUrl;
    }
    return app.redirectUrl;
  }

  viewAllApps(event) {
    window.location.href = 'https://kibana.vn/token?t=' + this.authService.getAccessToken() + '&r=' + this.authService.getRefreshToken();
  }

  viewAllAppsUrl() {
    return 'https://launcher.kibana.vn/token?t=' + this.authService.getAccessToken() + '&r=' + this.authService.getRefreshToken();
  }

  onClickOutside() {
    this.transferService.appClickEvent.pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.isOpened = false;
      })
  }

  clickAppInstance(event) {
    event.stopPropagation();
    event.preventDefault();
  }

  updateFavourite(app: App, isFavourite: boolean, event) {
    event.stopPropagation();
    event.preventDefault();

    this.isLoading = true;
    this.appService.updateFavourite(app.id, isFavourite)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoading = false;
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['COMMON']['UPDATE_SUCCESS_MSG']));
            this.apps = [];
            this.openApps(event);
          }
        },
        _ => this.isLoading = false
      )
  }

  back() {
    this.router.navigateByUrl(`/${CommonRedirect}`);
  }
}
