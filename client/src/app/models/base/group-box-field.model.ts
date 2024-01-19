import { GroupBoxFieldType } from "../../enumerations/common.enum";

export class GroupBoxField {
  public fieldName = "";

  public value: any;

  public title = "";

  public type?= GroupBoxFieldType.Text;

  public scale?= 12;

  public required?= false;

  public placeholder?= "";

  public isFetching?= false;

  public comboboxUrl?= "";

  public pickList?: ComboBoxItem[] = [];

  public comboboxMap?: ComboBoxMap;

  public addSelectorFunc?: Function;

  public isDisplay?= true;

  public width?= 0;

  public error?= true;

  public errorMessage?= "asdksakdskdsk";
}

export class ComboBoxMap {
  public id = "";
  public value = "";
}

export class ComboBoxItem {
  public id: any;
  public value: any;
}
