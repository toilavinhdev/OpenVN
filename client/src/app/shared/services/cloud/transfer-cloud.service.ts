import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ServiceResult } from "src/app/models/base/service-result";
import { HttpService } from "../base/http.service";
import { BaseCloudService } from "./base-cloud.service";
import { MoveObject } from "src/app/models/cloud/move-object";
import { CopyCut } from "src/app/models/cloud/copy-cut";

@Injectable({
  providedIn: 'root'
})
export class TransferCloudService extends BaseCloudService {

  constructor(
    http: HttpService
  ) {
    super(http);
    this.controller = 'transfer-cloud';
  }

  move(copyCut: CopyCut) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}/move`;
    return this.http.post<ServiceResult>(url, copyCut);
  }

  saveOne(entity: any, customUrl?: string): Observable<ServiceResult> {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}`;
    return this.http.post<ServiceResult>(url, JSON.parse(JSON.stringify(entity)), this.httpOption(entity.parentId));
  }
}
