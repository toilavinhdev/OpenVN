import { Injectable } from "@angular/core";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { PaginationRequest } from "src/app/models/base/pagination-request";
import { ServiceResult } from "src/app/models/base/service-result";

@Injectable({
  providedIn: 'root'
})
export class AuditService extends BaseService {
  constructor(
    http: HttpService
  ) {
    super(http);
    this.serviceName = 'opensync';
    this.controller = 'audit';
  }

  getAuditPaging(request: PaginationRequest) {
    const page = request.pageIndex;
    const size = request.pageSize;
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/paging?page=${page}&size=${size}`;
    return this.http.get<ServiceResult>(url);
  }
}
