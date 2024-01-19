import { HttpStatusCode } from "@angular/common/http";
import { ErrorModel } from "./error";

export class BaseMessageResponse {
  public status = "success";

  public error: ErrorModel;
}
