import { BaseMessageResponse } from "./base-message-response";

export class ServiceResult extends BaseMessageResponse {
  public data: any;

  public total: number = 0;

  public serverTime = "";
}
