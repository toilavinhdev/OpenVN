import { Component, Injector, OnInit } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { ColumnGrid } from 'src/app/models/base/column-grid.model';
import { User } from 'src/app/models/user/user';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { GroupBoxFieldType } from 'src/app/shared/enumerations/common.enum';
import { CpanelService } from 'src/app/shared/services/cpanel/cpanel.service';

@Component({
  selector: 'app-cpanel-user',
  templateUrl: './cpanel-user.component.html',
  styleUrls: ['./cpanel-user.component.scss']
})
export class CpanelUserComponent extends BaseComponent {

  pageSizeOptions = [20, 50, 100];
  current = 0;
  total = 0;
  displayColumn: ColumnGrid[] = [];
  users: User[] = [];

  constructor(
    public inejctor: Injector,
    public cpanelService: CpanelService
  ) {
    super(inejctor);
  }

  initData(): void {
    super.initData();
    this.loadUsers();
    this.initColumns();
  }

  initColumns() {
    this.displayColumn = [];
    // this.displayColumn.push({ displayText: 'STT', column: 'ordinal', width: 60, sortable: false });
    this.displayColumn.push({ displayText: 'Id', column: 'id', width: 160, sortable: false });
    this.displayColumn.push({ displayText: 'Tài khoản', column: 'username', width: 140, sortable: false });
    this.displayColumn.push({ displayText: 'Họ và tên', column: 'fullName', width: 160, sortable: false });
    this.displayColumn.push({ displayText: 'Ngày sinh', column: 'dateOfBirth', width: 160, type: GroupBoxFieldType.Date, sortable: false });
    this.displayColumn.push({ displayText: 'Số điện thoại', column: 'phoneNumber', width: 120, sortable: false });
    this.displayColumn.push({ displayText: 'Email', column: 'email', width: 240, sortable: false });
    this.displayColumn.push({ displayText: 'Địa chỉ', column: 'address', width: 320, sortable: false });
    this.displayColumn.push({ displayText: 'Ngày tạo tài khoản', column: 'createdDate', width: 200, type: GroupBoxFieldType.DateTime, sortable: false });
  }

  loadUsers() {
    this.isLoading = true;
    this.cpanelService.getUsers(this.paginationRequest)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.isLoading = false;
        if (resp.status == 'success') {
          this.users = resp.data;
          this.current = this.users.length;
          this.total = resp.total;
          // this.users.forEach((x, index) => x['ordinal'] = this.paginationRequest.pageIndex * this.paginationRequest.pageSize + index + 1);
        }
      },
        _ => this.isLoading = false
      )
  }

  changePage(event) {
    if (event.pageSize != this.paginationRequest.pageSize) {
      this.paginationRequest.pageSize = event.pageSize;
      this.paginationRequest.pageIndex = 0;
    } else {
      this.paginationRequest.pageIndex = event.pageIndex;
    }
    this.loadUsers();
  }
}
