import { Injectable } from "@angular/core";
import { ServiceResult } from "../../../models/base/service-result";
import { ConfigValue } from "../../../models/config/config-value-model";
import { UserConfig } from "../../../models/config/user-config-model";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { CommonConstant } from "../../constants/common.constant";

@Injectable({
  providedIn: 'root'
})
export class ConfigService extends BaseService {

  userConfig: UserConfig;

  constructor(
    http: HttpService
  ) {
    super(http);

    this.serviceName = 'opensync';
    this.controller = 'config';
  }

  getConfig() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}`;
    return this.http.get<ServiceResult>(url);
  }

  setConfig(config: ConfigValue) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}`;
    return this.http.put<ServiceResult>(url, config);
  }
}
