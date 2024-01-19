import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { CommonRedirect } from 'src/app/shared/constants/common.constant';

@Component({
  selector: 'app-feedback-header',
  templateUrl: './feedback-header.component.html',
  styleUrls: ['./feedback-header.component.scss']
})
export class FeedbackHeaderComponent extends BaseComponent {

  constructor(
    public inejctor: Injector,
    public router: Router
  ) {
    super(inejctor);
  }

  backHome() {
    this.router.navigateByUrl(`/${CommonRedirect}`);
  }
}
