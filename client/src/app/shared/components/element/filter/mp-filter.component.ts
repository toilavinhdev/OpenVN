import { Component, Input, ViewChild } from '@angular/core';
import { ButtonType, IconButtonType } from '../../constants/button.constant';
import { FilterCondition } from '../../enumerations/common.enum';
import { Field } from '../../../models/base/field-model';
import { Filterable } from '../../../models/base/filterable-model';
import { BaseService } from '../../services/base/base.service';
import { Utility } from '../../utility/utility';
import { BaseComponent } from '../base-component';
import { BaseButton } from '../mp-button/mp-button.component';

@Component({
  selector: 'mp-filter',
  templateUrl: './mp-filter.component.html',
  styleUrls: ['./mp-filter.component.scss']
})
export class MpFilterComponent extends BaseComponent {

  Utility = Utility;

  ButtonType = ButtonType;

  IconButtonType = IconButtonType;

  conditionTypes = [
    { value: FilterCondition.E, text: "Equal" },
    { value: FilterCondition.NE, text: "Not equal" },
    { value: FilterCondition.GT, text: "Greater than" },
    { value: FilterCondition.GE, text: "Greater than or equal" },
    { value: FilterCondition.LT, text: "Less than" },
    { value: FilterCondition.LE, text: "Less than or equal" },
    { value: FilterCondition.C, text: "Contain" },
    { value: FilterCondition.NC, text: "Not contain" },
    { value: FilterCondition.SW, text: "Starts with" },
    { value: FilterCondition.NSW, text: "Not start with" },
    { value: FilterCondition.EW, text: "Ends with" },
    { value: FilterCondition.NEW, text: "Not end with" },
  ]

  groups: any[] = [];

  conditions: Field[] = [];

  @Input()
  filterable: Filterable[] = [];

  @ViewChild("addConditionBtn")
  addConditionBtn!: BaseButton;

  constructor(baseService: BaseService) {
    super(baseService);
  }

  ngOnInit() {
    super.ngOnInit();
  }

  initData() {
    this.conditions = [new Field()];
  }

  addCondition() {
    this.conditions.push(new Field());
  }
}
