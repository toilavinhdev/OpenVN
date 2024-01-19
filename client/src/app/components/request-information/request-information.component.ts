import { Component, Injector, OnInit } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { RequestInformation } from 'src/app/models/base/request-information';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';

@Component({
  selector: 'app-request-information',
  templateUrl: './request-information.component.html',
  styleUrls: ['./request-information.component.scss']
})
export class RequestInformationComponent extends BaseComponent {

  information = new RequestInformation();

  constructor(
    public injector: Injector,
    public transferService: TransferDataService,
    public authService: AuthService
  ) {
    super(injector);
  }

  initData(): void {
    super.initData();
    this.transferService.resolvedEvent.emit();
    this.loadInformation();
  }

  loadInformation() {
    this.isLoading = true;
    this.authService
      .getRequestInformation()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.isLoading = false;
        if (resp.status == 'success') {
          this.information = resp.data;
        }
      })
  }
}
