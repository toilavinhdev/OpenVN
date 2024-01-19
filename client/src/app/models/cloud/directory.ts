import { BaseModel } from "../base/base-model";

export class Directory extends BaseModel {
  public name = "";
  public parentId = "0";
  public children: Directory[] = [];
  public childrenCount = 0;
  public path = "";
  public isLocked = false;
  public hasPassword = false;
  public createdDate: Date = new Date();
  public lastModifiedDate?: Date = null;
}

export class DirectoryUI extends Directory {
  public isEditing = false;
  public isUpdating = false;
  public canChangeSelected = true;
  public selected = false;
  public hasUnlocked = false;
  public underCopyOrCut = false;
}
