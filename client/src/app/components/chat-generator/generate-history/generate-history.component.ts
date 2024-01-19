import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { ChatGenerator } from 'src/app/models/chat/chat-generator';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { Routing } from 'src/app/shared/constants/common.constant';
import { ChatGeneratorService } from 'src/app/shared/services/chat/chat-generator.service';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-generate-history',
  templateUrl: './generate-history.component.html',
  styleUrls: ['./generate-history.component.scss']
})
export class GenerateHistoryComponent extends BaseComponent {

  Utility = Utility;

  gens: ChatGenerator[] = [];

  pageSizeOptions: number[] = [20, 50, 100];

  total = 0;

  constructor(
    public injector: Injector,
    public chatGeneratorService: ChatGeneratorService,
    public router: Router
  ) {
    super(injector);
  }

  initData() {
    super.initData();
    this.loadData();
  }

  loadData() {
    this.isLoading = true;
    this.chatGeneratorService.getGeneratesPaging(this.paginationRequest)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoading = false;
          if (resp.status == 'success') {
            this.gens = resp.data;
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

    this.loadData();
  }

  presentChat(id) {
    this.router.navigateByUrl(`/${Routing.GENERATE_CHAT.path}/${id}`);
  }

  upload() {
    this.router.navigateByUrl(`/${Routing.GENERATE_CHAT.path}`);
  }
}
