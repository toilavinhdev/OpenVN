import { Component, Directive, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormMode } from 'src/app/shared/enumerations/common.enum';
import { ActionExponent } from 'src/app/shared/enumerations/permission.enum';
import { ColumnGrid } from 'src/app/models/base/column-grid.model';
import { BaseService } from 'src/app/shared/services/base/base.service';
import { environment } from 'src/environments/environment';
import { BaseComponent } from '../../base-component';
import { IBaseList as IBaseList } from './ibase-list';
import { Routing } from 'src/app/shared/constants/common.constant';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { takeUntil } from 'rxjs/operators';
import { ListDynamicOption } from 'src/app/models/base/list-dynamic-option';

@Directive()
export class BaseListComponent extends BaseComponent implements IBaseList {

  Routing = Routing;

  ActionExponent = ActionExponent;

  FormMode = FormMode;

  basePath = "";

  options = new ListDynamicOption();

  constructor(
    baseService: BaseService,
    public activatedRoute: ActivatedRoute,
    public translationService: TranslationService,
  ) {
    super(baseService);

    this.translationService.changeLanguageEvent
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(() => {
        this.initOptions();
      });
  }

  initOptions() {
    throw new Error('Method not implemented.');
  }

  ngOnInit() {
    super.ngOnInit();
    this.getBasePath();
  }

  initData() {
    this.initOptions();
  }

  getBasePath() {
    const splits = this.activatedRoute.snapshot["_routerState"]["url"].split("/");
    if (splits && splits.length >= 2) {
      this.basePath = splits[1];
    }
  }
}
