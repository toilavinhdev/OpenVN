import { environment } from "src/environments/environment";
import { LocalStorageKey } from "../constants/localstorage.key";
import { StringHelper } from "./string.helper";
import { LocalHelper } from "./local.helper";

export class UserHelper {
  static get USER_PERMISSION(): string {
    try {
      const accessToken = LocalHelper.getAndParse('auth')[LocalStorageKey.ACCESS_TOKEN] || '';
      if (accessToken) {
        const permission = StringHelper.parseJwt(accessToken)["permission"];
        return permission + "";
      }
    } catch (e) {
      return '0';
    }
    return '0';
  }

  static get USER_ROLES(): string[] {
    try {
      const accessToken = LocalHelper.getAndParse('auth')[LocalStorageKey.ACCESS_TOKEN] || '';
      if (accessToken) {
        const roles = StringHelper.parseJwt(accessToken)["roles"];
        return roles.split(',');
      }
    } catch (e) {
      return [];
    }
    return [];
  }
}
