import { CommonConstant } from "src/app/shared/constants/common.constant";
import { NotificationType } from "./notify-type.enum";
import { SnackBarType } from "./snackbar-type.enum";
import { StringHelper } from "src/app/shared/helpers/string.helper";

export class Mark {
  public allowNotice = true;
  public notificationType = NotificationType.MessageBox;

  constructor(allowNotice: boolean, notificationType = NotificationType.MessageBox) {
    this.allowNotice = allowNotice;
    this.notificationType = notificationType;
  }

  static getMark(url: string): Mark {
    if(StringHelper.isNullOrEmpty(url) || url.search(CommonConstant.DISALLOW_NOTICE) !== -1) {
      return new Mark(false);

    }

    if (url.search(CommonConstant.ALLOW_NOTICE_WITH_SNACKBAR_WARNING) !== -1) {
      return new Mark(true, NotificationType.SnackBarWarning);
    }

    if (url.search(CommonConstant.ALLOW_NOTICE_WITH_SNACKBAR_DANGER) !== -1) {
      return new Mark(true, NotificationType.SnackBarDanger);
    }

    return new Mark(true, NotificationType.MessageBox);
  }
}
