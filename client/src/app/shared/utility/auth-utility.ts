import * as bigInt from "big-integer";
import { ActionExponent } from "../enumerations/permission.enum";
import { UserHelper } from "../helpers/user.helper";
import { DateHelper } from "../helpers/date.helper";
import { Utility } from "./utility";

export class AuthUtility {
  public static checkPermission(actionExponents: ActionExponent[]) {
    // Nếu không yêu cầu permission
    if (!actionExponents.length || actionExponents.every(p => p === ActionExponent.None)) {
      return true;
    }

    // Lấy quyền người dùng
    const numberTwo = bigInt(2);
    const userPermission = bigInt(UserHelper.USER_PERMISSION);

    for (let i = 0; i < actionExponents.length; i++) {
      const exponent = actionExponents[i];
      let action = bigInt(1);
      for (let e = 1; e <= exponent; e++) {
        action = action.multiply(numberTwo);
      }

      const hasPermission = userPermission.and(action).compare(action) == 0;
      console.customize(`per:`, userPermission.toString(), "action:", action.toString(), " => result: ", hasPermission);
      if (!hasPermission) {
        return false;
      }
    }

    return true;
  }
}
