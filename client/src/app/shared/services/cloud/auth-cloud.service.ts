import { Injectable } from "@angular/core";
import { ServiceResult } from "src/app/models/base/service-result";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { CommonConstant } from "../../constants/common.constant";
import { BaseCloudService } from "./base-cloud.service";

@Injectable({
  providedIn: 'root'
})
export class AuthCloudService extends BaseCloudService {

  constructor(
    http: HttpService
  ) {
    super(http);
    this.controller = 'authcloud';
  }

  genCode(dirId: string, password: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/gen-code?${CommonConstant.ALLOW_NOTICE_WITH_SNACKBAR_DANGER}`;
    return this.http.post<ServiceResult>(url, { directoryId: dirId, password: password });
  }

  lock(dirId: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/lock/${dirId}`;
    return this.http.put<ServiceResult>(url, null);
  }

  unlock(dirId: string, password: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/unlock`;
    return this.http.put<ServiceResult>(url, { directoryId: dirId, password: password });
  }

  setPassword(dirId: string, password: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/set-password`;
    return this.http.post<ServiceResult>(url, { directoryId: dirId, password: password });
  }
}
