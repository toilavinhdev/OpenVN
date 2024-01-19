import { Component, Injector, OnInit } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { AuditService } from 'src/app/shared/services/audit/audit.service';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-audit-log',
  templateUrl: './audit-log.component.html',
  styleUrls: ['./audit-log.component.scss']
})
export class AuditLogComponent extends BaseComponent {

  Utility = Utility;

  logs: any[] = [];

  pageSizeOptions: number[] = [20, 50, 100];

  total = 0;

  descriptionWidth = Math.max(400, window.innerWidth - 736);

  constructor(
    public injector: Injector,
    public auditService: AuditService
  ) {
    super(injector);
  }

  initData() {
    super.initData();
    this.loadAudit();
  }

  loadAudit() {
    this.isLoading = true;
    this.auditService.getAuditPaging(this.paginationRequest)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoading = false;
          if (resp.status == 'success') {
            this.logs = resp.data;
            this.total = resp.total;
          }
        },
        _ => this.isLoading = false
      );
  }

  changePage(event) {
    if (event.pageSize != this.paginationRequest.pageSize) {
      this.paginationRequest.pageSize = event.pageSize;
      this.paginationRequest.pageIndex = 0;
    } else {
      this.paginationRequest.pageIndex = event.pageIndex;
    }

    this.loadAudit();
  }
}
