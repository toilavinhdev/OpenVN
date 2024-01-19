import { Component, Injector, OnInit } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { GroupBoxFieldType } from 'src/app/shared/enumerations/common.enum';
import { BaseModel } from 'src/app/models/base/base-model';
import { ColumnGrid } from 'src/app/models/base/column-grid.model';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { BaseService } from 'src/app/shared/services/base/base.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';

@Component({
  selector: 'app-sign-in-logging',
  templateUrl: './sign-in-logging.component.html',
  styleUrls: ['./sign-in-logging.component.scss']
})
export class SignInLoggingComponent extends BaseComponent {

  displayColumn: ColumnGrid[] = [];

  data: BaseModel[] = [];

  pageSizeOptions = [20, 50, 100];

  current = 0;

  total = 0;

  constructor(
    public injector: Injector,
    public authService: AuthService,
    public translationService: TranslationService,
  ) {
    super(injector);
  }

  initData() {
    super.initData();
    this.initColumn();
    this.getData();

    this.translationService
      .changeLanguageEvent
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(() => {
        this.initColumn();
      })
  }

  initColumn() {
    this.displayColumn = [];
    this.displayColumn.push({ column: "signInTime", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['SIGN_IN_TIME'], width: 160, type: GroupBoxFieldType.DateTime, sortable: false });
    this.displayColumn.push({ column: "ip", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['IP'], width: 120, sortable: false });
    this.displayColumn.push({ column: "device", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['DEVICE'], width: 120, sortable: false });
    this.displayColumn.push({ column: "browser", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['BROWSER'], width: 180, sortable: false });
    this.displayColumn.push({ column: "os", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['OS'], width: 140, sortable: false });
    this.displayColumn.push({ column: "city", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['CITY'], width: 120, sortable: false });
    this.displayColumn.push({ column: "country", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['COUNTRY'], width: 100, sortable: false });
    this.displayColumn.push({ column: "lat", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['LAT'], width: 80, sortable: false });
    this.displayColumn.push({ column: "long", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['LONG'], width: 80, sortable: false });
    this.displayColumn.push({ column: "timezone", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['TIMEZONE'], width: 120, sortable: false });
    this.displayColumn.push({ column: "org", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['ORG'], width: 160, sortable: false });
    this.displayColumn.push({ column: "postal", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['POSTAL'], width: 80, sortable: false });
    this.displayColumn.push({ column: "origin", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['ORIGIN'], width: 160, sortable: false, type: GroupBoxFieldType.Link, getLink: (event) => event.origin, target: '_blank' });
    this.displayColumn.push({ column: "ua", displayText: TranslationService.VALUES['SIGN_IN_LOGGING']['COLUMNS']['USER_AGENT'], width: 320, sortable: false });
  }

  getData() {
    this.isLoading = true;
    this.authService.getSignInLoggingPaging(this.paginationRequest)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoading = false;
          if (resp.status == "success") {
            this.data = resp.data;
            this.total = resp.total;
            this.current = resp.data?.length;
          }
        },
        () => this.isLoading = false
      );
  }

  changePage(event) {
    if (event.pageSize != this.paginationRequest.pageSize) {
      this.paginationRequest.pageSize = event.pageSize;
      this.paginationRequest.pageIndex = 0;
    } else {
      this.paginationRequest.pageIndex = event.pageIndex;
    }
    this.getData();
  }
}
