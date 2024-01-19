import { Component, Injector, OnInit } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { CpanelService } from 'src/app/shared/services/cpanel/cpanel.service';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-cpanel-dashboard',
  templateUrl: './cpanel-dashboard.component.html',
  styleUrls: ['./cpanel-dashboard.component.scss']
})
export class CpanelDashboardComponent extends BaseComponent {

  blocks: any[] = [];

  isRecordLoading = false;

  constructor(
    public inejctor: Injector,
    public cpanelService: CpanelService
  ) {
    super(inejctor);
  }

  initData(): void {
    super.initData();
    this.initBlocks();
  }

  initBlocks() {
    // this.blocks = [
    //   {
    //     type: 'audit_log',
    //     title: TranslationService.VALUES['CPANEL']['TITLE_AUDIT_LOG'],
    //     value: 0
    //   },
    //   {
    //     type: 'view_signin',
    //     title: TranslationService.VALUES['CPANEL']['TITLE_VIEW_SIGN_IN_LOGGING'],
    //     value: 0
    //   },
    //   {
    //     type: 'view_information',
    //     title: TranslationService.VALUES['CPANEL']['TITLE_VIEW_INFORMATION'],
    //     value: 0
    //   },
    //   {
    //     type: 'view_policy',
    //     title: TranslationService.VALUES['CPANEL']['TITLE_VIEW_POLICY'],
    //     value: 0
    //   },
    // ];

    this.isRecordLoading = true;
    this.cpanelService.getDashboardRecords()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.isRecordLoading = false;
        if (resp.status == 'success') {
          this.blocks = resp.data;

          const colors = ['#007bff', '#6610f2', '#6f42c1', '#e83e8c', '#dc3545', '#fd7e14', '#ffc107', '#28a745', '#20c997', '#17a2b8', '#6c7,57', '#343a40', '#007bff', '#6c757d', '#28a745', '#17a2b8', '#ffc107', '#dc3545', '#343a40',];
          this.blocks.forEach(x => {
            let attempt = 1;
            while (attempt++ < colors.length) {
              const color = this.randomColor(colors);
              if (!this.blocks.find(x => x.color == color)) {
                x['color'] = color;
                break;
              }
            }
          });
        }
      },
        _ => this.isRecordLoading = false
      )
  }

  randomColor(colors: string[]) {
    return colors[Utility.randomInRange(0, colors.length - 1)];
  }
}
