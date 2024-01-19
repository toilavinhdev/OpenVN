import { BaseModel } from "../base/base-model";

export class Note extends BaseModel {
  public title  = "";
  public content  = "";
  public order = 0;
  public categoryId  = "";
  public categoryName  = "";
  public isPublic = false;
  public isPinned = false;
  public originBackgroundColor = "#fff";
  public backgroundColor = "#fff";
  public ownerId = "0";
}
