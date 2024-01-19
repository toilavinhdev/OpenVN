import { RequestErrorType } from "./request-type";

export class RequestErrorMapping {
  public static mapping: RequestErrorMappingFunction[] = [];
}

export class RequestErrorMappingFunction {
  public type = "";
  public func: Function = null;
}
