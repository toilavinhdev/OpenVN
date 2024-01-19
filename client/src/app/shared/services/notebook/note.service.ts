import { Injectable } from "@angular/core";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { ServiceResult } from "../../../models/base/service-result";
import { Note } from "../../../models/notebook/note";
import { StringHelper } from "../../helpers/string.helper";
import { CommonConstant } from "../../constants/common.constant";

@Injectable({
  providedIn: 'root'
})
export class NoteService extends BaseService {

  constructor(
    http: HttpService
  ) {
    super(http);

    this.serviceName = 'opensync';
    this.controller = 'notebook';
  }

  getNotes(query: string = "") {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/notes` + (!StringHelper.isNullOrEmpty(query) ? `?query=${query}` : '');
    return this.http.get<ServiceResult>(url);
  }

  getNote(id: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/note-by-id/${id}`;
    return this.http.get<ServiceResult>(url);
  }

  getNoteWithoutWarning(id: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/note-by-id/${id}?${CommonConstant.DISALLOW_NOTICE}`;
    return this.http.get<ServiceResult>(url);
  }

  saveNote(note: Note) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/add-note`;
    return this.saveOne(note, url);
  }

  updateNote(note: Note, type: number) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/update-note?type=${type}`;
    return this.update(note, url);
  }

  deleteNote(ids: string[]) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/delete-note`;
    return this.delete(ids, url);
  }
}
