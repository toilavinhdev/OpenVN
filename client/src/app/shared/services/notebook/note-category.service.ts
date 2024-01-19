import { Injectable } from "@angular/core";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { CommonConstant } from "../../constants/common.constant";
import { ServiceResult } from "../../../models/base/service-result";
import { NoteCategory } from "../../../models/notebook/note-category";

@Injectable({
  providedIn: 'root'
})
export class NoteCategoryService extends BaseService {

  constructor(
    http: HttpService
  ) {
    super(http);

    this.serviceName = 'opensync';
    this.controller = 'notebook';
  }

  getCategories() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/categories?${CommonConstant.ALLOW_NOTICE_WITH_SNACKBAR_DANGER}`;
    return this.http.get<ServiceResult>(url);
  }

  saveCategory(category: NoteCategory) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/add-category`;
    return this.saveOne(category, url);
  }

  updateCategory(category: NoteCategory) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/update-category`;
    return this.update(category, url);
  }

  deleteCategories(ids: any[]) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/delete-category`;
    return this.delete(ids, url);
  }
}
