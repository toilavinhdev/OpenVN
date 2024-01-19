import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { ServiceResult } from "src/app/models/base/service-result";
import { UserConfig } from "src/app/models/config/user-config-model";

@Injectable({
  providedIn: 'root'
})
export class AppService extends BaseService {

  userConfig: UserConfig;

  constructor(
    http: HttpService
  ) {
    super(http);

    this.serviceName = 'master';
    this.controller = 'app';
  }

  getApp() {
    const url = `${environment.master_app_api}/${this.serviceName}/${this.controller}`;
    return this.http.get<ServiceResult>(url);
  }

  updateFavourite(appId: string, isFavourite: boolean) {
    const url = `${environment.master_app_api}/${this.serviceName}/${this.controller}/update-favourite?appId=${appId}&isFavourite=${isFavourite}`;
    return this.http.put<ServiceResult>(url, {});
  }
}
