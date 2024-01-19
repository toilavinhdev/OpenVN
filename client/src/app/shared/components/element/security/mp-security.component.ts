import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { Routing } from '../../constants/common.constant';
import { DxTextBoxComponent } from 'devextreme-angular';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { SessionStorageKey } from '../../constants/sessionstorage.key';
import { SnackBar } from '../snackbar/snackbar.component';
import { SnackBarParameter } from '../../../models/snackbar.param';
import { AuthService } from '../../services/auth/auth.service';
import { BaseButton } from '../mp-button/mp-button.component';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from '../base-component';
import { BaseService } from '../../services/base/base.service';
import { SharedService } from '../../services/base/shared.service';
import { DeviceType } from '../../enumerations/device.enum';

@Component({
  selector: 'mp-security',
  templateUrl: './mp-security.component.html',
  styleUrls: ['./mp-security.component.scss']
})
export class MpSecurityComponent extends BaseComponent implements AfterViewInit {

  secretKey = "";

  @Input()
  urlValidateSecretKey = "";

  @Input()
  actionAfterPassSecurity: Function;

  @Input()
  actionAfterRejectSecretKey: Function;

  @ViewChild("inputInstance")
  inputInstance!: DxTextBoxComponent;

  @ViewChild("commitBtn")
  commitBtn!: BaseButton;

  constructor(
    baseService: BaseService,
    public authService: AuthService,
    public router: Router,
    public sharedService: SharedService
  ) {
    super(baseService)
  }

  ngAfterViewInit(): void {
    if (SharedService.DeviceType == DeviceType.Desktop) {
      this.inputInstance.instance.focus();
    }
  }

  commit() {
    if (this.secretKey == '') {
      this.commitBtn.isFinished = true;
      SnackBar.warning(new SnackBarParameter(this, "Please enter your secret key"));
      return;
    }
    this.authService.verifySecretKey(this.secretKey, this.urlValidateSecretKey)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        response => {
          this.commitBtn.isFinished = true;
          if (response.success) {
            sessionStorage.setItem(`${environment.organization}_${SessionStorageKey.SECRET_CODE}`, response.data);
            if (this.actionAfterPassSecurity) {
              this.actionAfterPassSecurity();
            }
          }
          else {
            if (this.actionAfterRejectSecretKey) {
              this.actionAfterRejectSecretKey();
            }
            else {
              this.router.navigateByUrl(`/${Routing.ACCESS_DENIED.path}`);
            }
          }
        },
        () => this.commitBtn.isFinished = true
      );
  }

  onEnterKey(event) {
    this.commitBtn.clickExecute(event);
  }
}
