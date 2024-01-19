import { Injectable } from "@angular/core";
import { ServiceResult } from "src/app/models/base/service-result";
import { environment } from "src/environments/environment";
import { SessionStorageKey } from "../../constants/sessionstorage.key";
import { HttpService } from "../base/http.service";
import { BaseCloudService } from "./base-cloud.service";

@Injectable({
  providedIn: 'root'
})
export class CloudFileService extends BaseCloudService {
  constructor(
    http: HttpService
  ) {
    super(http);
    this.controller = 'cloud';
  }

  getCloudConfiguration() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/capacity-configuration`;
    return this.http.get<ServiceResult>(url, this.httpOption("0"));
  }

  getFilesInDir(directoryId: string, connectionId: string) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/files-in-dir/${directoryId}?connectionId=${connectionId}`;
    return this.http.get<ServiceResult>(url, this.httpOption(directoryId));
  }

  getCountInformation() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/count-information`;
    return this.http.get<ServiceResult>(url);
  }

  deletes(directoryId: string, ids: any[]) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/${directoryId}`;
    return this.http.delete<ServiceResult>(url, ids.map(id => id + ""), this.httpOption(directoryId));
  }

  upload(directoryId: string, formData: FormData) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/upload/${directoryId}`;
    const options = this.httpOption(directoryId);
    // options["reportProgress"] = true;
    // options["observe"] = "events";
    return this.http.post<ServiceResult>(url, formData, options);
  }
}
