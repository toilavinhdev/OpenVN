import { GroupBoxFieldType } from "../../enumerations/common.enum";
import { GroupBoxField } from "./group-box-field.model";

export class GroupBox {
  public name = "";

  public groupBoxFields: GroupBoxField[] = [];
}
