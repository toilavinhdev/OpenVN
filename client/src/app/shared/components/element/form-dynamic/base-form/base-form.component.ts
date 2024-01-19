import { Component, Directive, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormMode } from 'src/app/shared/enumerations/common.enum';
import { GroupBox } from 'src/app/models/base/group-box.model';
import { BaseService } from 'src/app/shared/services/base/base.service';
import { BaseComponent } from '../../base-component';
import { IBaseForm } from './ibase-form';


@Directive()
export class BaseFormComponent extends BaseComponent implements IBaseForm {

  formMode = FormMode.None;

  groupBoxes: GroupBox[] = [];

  serviceName = "";

  controller = "";

  constructor(
    baseService: BaseService,
    public activatedRoute: ActivatedRoute
  ) {
    super(baseService);
  }

  ngOnInit() {
    super.ngOnInit();
  }

  initData() {
    this.getFormMode();
    this.initGroupboxes();
  }

  initGroupboxes(): void {
    throw new Error('Method not implemented.');
  }

  getFormMode() {
    this.formMode = this.activatedRoute.snapshot.data["formMode"];
  }

}
