import { Injectable } from "@angular/core";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { ServiceResult } from "src/app/models/base/service-result";
import { PaginationRequest } from "src/app/models/base/pagination-request";

@Injectable({
  providedIn: 'root'
})
export class CpanelService extends BaseService {

  constructor(
    http: HttpService
  ) {
    super(http);

    this.serviceName = 'opensync';
    this.controller = 'cpanel';
  }

  getDashboardRecords() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/dashboard/total-records`;
    return this.http.get<ServiceResult>(url);
  }

  getRoles() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/role/roles`;
    return this.http.get<ServiceResult>(url);
  }

  getUsers(request: PaginationRequest) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/user/users?page=${request.pageIndex}&size=${request.pageSize}`;
    return this.http.get<ServiceResult>(url);
  }

  updateRole(roleId, actionId, value: boolean) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/role/update-role`;
    return this.http.put<ServiceResult>(url, { roleId: roleId, actionId: actionId, value: value });
  }
}
