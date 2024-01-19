import { BaseMessageResponse } from "src/app/models/base/base-message-response";

export class AuthResponse extends BaseMessageResponse {

  public accessToken: string = "";

  public refreshToken: string = "";

  public requiredMFA = false;
}
