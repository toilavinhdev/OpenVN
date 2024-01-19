import { Injectable } from "@angular/core";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { ServiceResult } from "../../../models/base/service-result";
import { StringHelper } from "../../helpers/string.helper";

@Injectable({
  providedIn: 'root'
})
export class LocationService extends BaseService {

  constructor(
    http: HttpService
  ) {
    super(http);

    this.serviceName = 'opensync';
    this.controller = 'location';
  }

  search(query: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/search?query=${query}`;
    return this.http.get<ServiceResult>(url);
  }

  getProvinces() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/provinces`;
    return this.http.get<ServiceResult>(url);
  }

  getDistricts(provinceId: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/districts?provinceId=${provinceId}`;
    return this.http.get<ServiceResult>(url);
  }

  getWards(districtId: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/wards?districtId=${districtId}`;
    return this.http.get<ServiceResult>(url);
  }
}
