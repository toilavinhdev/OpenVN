import { Injectable } from "@angular/core";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { ServiceResult } from "src/app/models/base/service-result";
import { PaginationRequest } from "src/app/models/base/pagination-request";
import { CommonConstant } from "../../constants/common.constant";

@Injectable({
  providedIn: 'root'
})
export class NotificationService extends BaseService {
  constructor(
    http: HttpService
  ) {
    super(http);

    this.serviceName = 'opensync';
    this.controller = 'notification';
  }

  getOnlineUsers() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/online-users`;
    return this.http.get<ServiceResult>(url);
  }

  getOnlineDetail() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/online-detail`;
    return this.http.get<ServiceResult>(url);
  }

  getNumberOfUnreadNotification() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/number-of-unread-notification`;
    return this.http.get<ServiceResult>(url);
  }

  getNotificationPaging(paginationRequest: PaginationRequest, type = "") {
    if (paginationRequest == null) {
      throw new Error(`Parameter paginationRequest cannot be null`);
    }
    const page = paginationRequest.pageIndex;
    const size = paginationRequest.pageSize;

    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/paging?page=${page}&size=${size}&type=${type}&${CommonConstant.ALLOW_NOTICE_WITH_SNACKBAR_DANGER}`;
    return this.http.get<ServiceResult>(url, this._baseOptions);
  }

  markAsRead(id: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/mark-as-read/${id}`;
    return this.http.put<ServiceResult>(url, null);
  }

  markAsUnread(id: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/mark-as-unread/${id}`;
    return this.http.put<ServiceResult>(url, null);
  }

  markAllAsRead() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/mark-all-as-read`;
    return this.http.put<ServiceResult>(url, null);
  }
}
