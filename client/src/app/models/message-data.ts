import { MessageBoxType } from "../shared/enumerations/common.enum";

export class MessageData {
  public title? = "";
  public content = "";
  public boxType? = MessageBoxType.Information;
}
