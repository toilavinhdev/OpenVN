import { Injectable } from "@angular/core";
import { HttpService } from "../base/http.service";
import { BaseService } from "../base/base.service";
import { ServiceResult } from "src/app/models/base/service-result";
import { PaginationRequest } from "src/app/models/base/pagination-request";

@Injectable({
  providedIn: 'root'
})
export class ChatGeneratorService extends BaseService {

  constructor(
    http: HttpService
  ) {
    super(http);
    this.serviceName = 'opensync';
    this.controller = 'chatgenerator';
  }

  upload(formData: FormData) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/upload`;
    return this.http.post<ServiceResult>(url, formData);
  }

  getGeneratesPaging(request: PaginationRequest) {
    const page = request.pageIndex;
    const size = request.pageSize;

    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/paging?page=${page}&size=${size}`;
    return this.http.get<ServiceResult>(url);
  }
}
