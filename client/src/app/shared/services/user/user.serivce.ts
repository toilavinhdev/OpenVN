import { Injectable } from "@angular/core";
import { ServiceResult } from "src/app/models/base/service-result";
import { User } from "src/app/models/user/user";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService {
  public user: User = null;

  constructor(
    http: HttpService
  ) {
    super(http);
    this.serviceName = 'opensync';
    this.controller = 'user';
  }

  getInformation() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/user-information`;
    return this.http.get<ServiceResult>(url);
  }

  getAvatar() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/avatar`;
    return this.http.get<ServiceResult>(url);
  }

  setAvatar(formData: FormData) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/set-avatar`;
    return this.http.post<ServiceResult>(url, formData);
  }

  removeAvatar() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/remove-avatar`;
    return this.http.delete<ServiceResult>(url);
  }
}
