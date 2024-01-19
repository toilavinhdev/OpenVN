import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonColor, ButtonType, IconButtonType } from 'src/app/shared/constants/button.constant';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';

@Component({
  selector: 'n-404',
  template: '<div class="not-found">the requested url was not found on this server</div>',
  styles: ['.not-found { padding: 16px; }']
})
export class BaseNotFound implements OnInit {
  ButtonColor = ButtonColor;

  ButtonType = ButtonType;

  IconButtonType = IconButtonType;
  constructor(
    public router: Router,
    public sharedService: SharedService,
    public transferService: TransferDataService
  ) { }

  ngOnInit(): void {
    this.transferService.resolvedEvent.emit();
  }

  back() {
    this.router.navigate([`/${this.sharedService.lastVisitedScreen}`]);
  }

}
