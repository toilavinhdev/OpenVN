import { BaseModel } from "../base/base-model";

export class Feedback extends BaseModel {
  public avatarUrl?= "";
  public email = "";
  public content = "";
  public parentId = "0";
  public hasReply = false;
  public replyCount?= 0;
  public timeFormat = "";
  public fromIP = "";
}
