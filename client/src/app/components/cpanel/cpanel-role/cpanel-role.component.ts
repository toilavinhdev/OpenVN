import { Component, Injector, OnInit } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { CpanelService } from 'src/app/shared/services/cpanel/cpanel.service';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-cpanel-role',
  templateUrl: './cpanel-role.component.html',
  styleUrls: ['./cpanel-role.component.scss']
})
export class CpanelRoleComponent extends BaseComponent {

  roles: any[] = [];

  constructor(
    public inejctor: Injector,
    public cpanelService: CpanelService
  ) {
    super(inejctor);
  }

  initData(): void {
    super.initData();
    this.loadRoles();
  }

  loadRoles() {
    this.isLoading = true;
    this.cpanelService.getRoles()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.isLoading = false;
        if (resp.status == 'success') {
          this.roles = resp.data;
          const sa = this.roles.find(x => x.roleCode == 'SUPER_ADMIN');
          const admin = this.roles.find(x => x.roleCode == 'ADMIN');
          const others = this.roles.filter(x => x.roleCode != 'SUPER_ADMIN' && x.roleCode != 'ADMIN');

          sa['actions'] = sa['actions'].filter(x => x.code == 'SUPER_ADMIN');
          sa['actions'][0]['disabled'] = true;

          admin['actions'] = admin['actions'].filter(x => x.code == 'FULL_BASIC');
          admin['actions'][0]['disabled'] = true;

          others.forEach(other => {
            other['actions'].find(x => x.code == 'SUPER_ADMIN')['disabled'] = true;
            other['actions'].find(x => x.code == 'FULL_BASIC')['disabled'] = true;
          });
        }
      },
        _ => this.isLoading = false
      );
  }

  updateRole(role, action, event) {
    event.stopPropagation();
    event.preventDefault();
    if (action.disabled) {
      return;
    }

    action.isContain = !action.isContain;
    this.cpanelService.updateRole(role.id, action.id, action.isContain)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        console.customize(resp);
      });
  }
}
