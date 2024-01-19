import { FileType } from "src/app/shared/enumerations/file.enum";
import { BaseModel } from "../base/base-model";

export class CloudFile extends BaseModel {
    public fileName = "";
    public originalFileName = "";
    public fileExtension = "";
    public size = 0;
    public directoryId = "";
    public url = "";
}

export class CloudFileUI extends CloudFile {
    public selected = false;
    public fileType = FileType.Other;
    public isDeleting = false;
    public canChangeSelected = true;
    public underCopyOrCut = false;
}