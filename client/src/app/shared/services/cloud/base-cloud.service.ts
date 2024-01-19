import { HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { SecretCode } from "src/app/models/cloud/secret-code";
import { HeaderNames } from "../../constants/header-names.constant";
import { LocalStorageKey } from "../../constants/localstorage.key";
import { StringHelper } from "../../helpers/string.helper";
import { BaseService } from "../base/base.service";
import { HttpOption, HttpService } from "../base/http.service";
import { ServiceResult } from "src/app/models/base/service-result";
import { CapacityConfiguration } from "src/app/models/cloud/capacity-configuration";

@Injectable({
  providedIn: 'root'
})
export class BaseCloudService extends BaseService {

  configuration = new CapacityConfiguration();

  constructor(
    http: HttpService
  ) {
    super(http);

    this.serviceName = 'opensync';
  }

  getObjectCode() {
    const objectCode = JSON.parse(localStorage.getItem(LocalStorageKey.UNLOCKED_DIR)) as {};
    if(!objectCode) {
      return {};
    }

    const keys = Object.keys(objectCode);
    for (let i = 0; i < keys.length; i++) {
      const key = keys[i];
      const code = objectCode[key] as SecretCode;
      if (StringHelper.isNullOrEmpty(code.value) || !code.expiry || new Date(code.expiry).getTime() < new Date().getTime()) {
        delete objectCode[key];
      }
    }

    return objectCode;
  }

  setUnlockedList(objectCode) {
    localStorage.setItem(LocalStorageKey.UNLOCKED_DIR, JSON.stringify(objectCode));
    // localStorage.setItem(LocalStorageKey.UNLOCKED_DIR_CODE_VALID_EXPIRY, new Date(new Date().getTime() + 5 * 60000).getTime() + "");
  }

  getCode(directoryId: string) {
    const unlocked = this.getObjectCode();
    return unlocked[directoryId]?.value || "";
  }

  httpOption(dirId: string) {
    const option = new HttpOption();
    option.headers = new HttpHeaders();
    option.headers = option.headers.set(HeaderNames.SECRET_KEY, this.getCode(dirId));
    return option;
  }

  getProperties(id: string, dirId: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/properties/${id}`;
    return this.http.get<ServiceResult>(url, this.httpOption(dirId));
  }
}
