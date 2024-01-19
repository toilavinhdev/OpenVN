import { HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ServiceResult } from "src/app/models/base/service-result";
import { HeaderNames } from "../../constants/header-names.constant";
import { SessionStorageKey } from "../../constants/sessionstorage.key";
import { HttpOption, HttpService } from "../base/http.service";
import { BaseCloudService } from "./base-cloud.service";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class DirectoryService extends BaseCloudService {

  constructor(
    http: HttpService
  ) {
    super(http);
    this.controller = 'directory';
  }

  isUnlocked(dirId) {
    const unlocked = this.getObjectCode();
    return unlocked[dirId] ?? false;
  }

  getChildrenDirectory(dirId: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/all-by-dir/${dirId}`;
    return this.http.get<ServiceResult>(url, this.httpOption(dirId));
  }

  getPath(dirId: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/path/${dirId}`;
    return this.http.get<ServiceResult>(url, this.httpOption(dirId));
  }

  getChildrenNodeId(dirId: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/children-node-id/${dirId}`;
    return this.http.get<ServiceResult>(url, this.httpOption(dirId));
  }

  saveOne(entity: any, customUrl?: string): Observable<ServiceResult> {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}`;
    return this.http.post<ServiceResult>(url, JSON.parse(JSON.stringify(entity)), this.httpOption(entity.parentId));
  }
}
