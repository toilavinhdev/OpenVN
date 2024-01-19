import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from "@angular/router";
import { HubConnectionState } from "@microsoft/signalr";
import { of } from "rxjs";
import { Observable } from "rxjs/internal/Observable";
import { switchMap } from "rxjs/operators";
import { UserConfig } from "src/app/models/config/user-config-model";
import { ServiceResult } from "../../models/base/service-result";
import { AuthService } from "../services/auth/auth.service";
import { HubConnectionService } from "../services/base/hub-connection.service";
import { SharedService } from "../services/base/shared.service";
import { TransferDataService } from "../services/base/transfer-data.service";
import { TranslationService } from "../services/base/translation.service";
import { ConfigService } from "../services/config/config.service";
import { SnackBar } from "../components/element/snackbar/snackbar.component";

@Injectable({
  providedIn: 'root'
})
export class BaseResolver<T> implements Resolve<T> {

  constructor(
    public router: Router,
    public authService: AuthService,
    public sharedService: SharedService,
    public configService: ConfigService,
    public translationService: TranslationService,
    public transferService: TransferDataService,
    public hubService: HubConnectionService
  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<T> | Promise<T> | T | any {
    // if (this.sharedService.deviceType == DeviceType.Mobile) {
    //   return this.router.navigateByUrl(`/${Routing.UNSUPPORT_DEVICE.path}`);
    // }

    if (!this.hubService.isAuthenticated && this.hubService.connection.state == HubConnectionState.Connected) {
      console.customize("revoke the current connection because it's  unauthenticated, and will then get a new authenticated connection");
      return this.hubService
        .stop()
        .pipe(
          switchMap(() => {
            setTimeout(() => SnackBar.close(), 50);
            return this.connect();
          })
        )
    }
    return this.connect();
  }

  connect() {
    return this.hubService
      .start()
      .pipe(
        switchMap(() => {
          this.hubService.isAuthenticated = true;
          if (!this.configService.userConfig && this.authService.isSignedIn()) {
            return this.configService.getConfig();
          }

          const service = new ServiceResult();
          service.data = this.configService.userConfig;
          service.status = 'success';
          return of(service);
        }),
        switchMap(resp => {
          if (resp.status == 'success') {
            this.configService.userConfig = resp.data;
            this.translationService.use(this.configService.userConfig.configValue.language);
            this.transferService.resolvedEvent.emit();
            return of(resp);
          }
          else {
            console.error(resp.error.message);
            this.configService.userConfig = new UserConfig();
            this.transferService.resolvedEvent.emit();
            return of(resp);
          }
        })
      );
  }
}
