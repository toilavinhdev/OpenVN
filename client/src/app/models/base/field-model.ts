import { FilterCondition } from "src/app/shared/enumerations/common.enum";

export class Field {
  public fieldName = "";

  public value: any;

  public condition: FilterCondition = FilterCondition.E;

  constructor(fieldName: string = "", value: any = null, condition: FilterCondition = FilterCondition.E) {
    this.fieldName = fieldName;
    this.value = value;
    this.condition = condition;
  }
}
