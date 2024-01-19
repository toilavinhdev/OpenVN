import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-cpanel',
  templateUrl: './cpanel.component.html',
  styleUrls: ['./cpanel.component.scss']
})
export class CpanelComponent extends BaseComponent{

  SharedService = SharedService;

  path = '';

  constructor(
    public injector: Injector,
    public activatedRoute: ActivatedRoute,
    public sharedService: SharedService
  ) {
    super(injector);
  }

  initData() {
    super.initData();
    this.path = window.location.pathname.trim().replace('/cpanel/', '');
  }
}
