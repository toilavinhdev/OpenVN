import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from 'src/environments/environment';
import { CommonConstant } from '../../constants/common.constant';
import { LocalStorageKey } from '../../constants/localstorage.key';
import { PaginationRequest } from '../../../models/base/pagination-request';
import { ServiceResult } from '../../../models/base/service-result';
import { HttpOption, HttpService } from './http.service';
import { LocalHelper } from '../../helpers/local.helper';

@Injectable({
  providedIn: 'root'
})
export class BaseService {

  private baseHost = environment.base_host + "/" + environment.api_version;

  private apiVersion = environment.api_version;

  public serviceName = "";

  public controller = "";

  userId = '';

  _baseOptions!: HttpOption;

  constructor(
    public http: HttpService
  ) {
    this.userId = LocalHelper.getAndParse('auth')[LocalStorageKey.USER_ID] || '';
  }

  getBaseHost() {
    return this.baseHost;
  }

  getApiVersion() {
    return this.apiVersion;
  }

  getById(id: any, customizeUrl = "") {
    const url = customizeUrl ? 
                customizeUrl : 
                `${this.baseHost}/${this.serviceName}/${this.controller}/${id}`;
    return this.http.get<ServiceResult>(url, this._baseOptions);
  }

  getAll(customizeUrl = ""): Observable<ServiceResult> {
    const url = customizeUrl ? 
                customizeUrl : 
                `${this.baseHost}/${this.serviceName}/${this.controller}?${CommonConstant.ALLOW_NOTICE_WITH_SNACKBAR_DANGER}`;
    return this.http.get<ServiceResult>(url, this._baseOptions);
  }

  paging(paginationRequest: PaginationRequest, customizeUrl = ""): Observable<ServiceResult> {
    const url = customizeUrl ? 
                customizeUrl : 
                `${this.baseHost}/${this.serviceName}/${this.controller}/paging?page=${paginationRequest.pageIndex}&size=${paginationRequest.pageSize}&${CommonConstant.ALLOW_NOTICE_WITH_SNACKBAR_DANGER}`;
    return this.http.get<ServiceResult>(url, this._baseOptions);
  }

  saveOne(entity, customizeUrl: string = "") {
    const url = customizeUrl ? 
                customizeUrl : 
                `${this.baseHost}/${this.serviceName}/${this.controller}`;
    return this.http.post<ServiceResult>(url, JSON.parse(JSON.stringify(entity)), this._baseOptions);
  }

  saveMany(entities: any[], customizeUrl: string = "") {
    const url = customizeUrl ? 
                customizeUrl : 
                `${this.baseHost}/${this.serviceName}/${this.controller}`;
    return this.http.post<ServiceResult>(url, JSON.parse(JSON.stringify(entities)), this._baseOptions);
  }

  update(entity: any, customizeUrl = "") {
    const url = customizeUrl ? 
                customizeUrl : 
                `${this.baseHost}/${this.serviceName}/${this.controller}`;
    return this.http.put<ServiceResult>(url, entity, this._baseOptions);
  }

  delete(ids: any[], customizeUrl = "") {
    const url = customizeUrl ? 
                customizeUrl : 
                `${this.baseHost}/${this.serviceName}/${this.controller}`;
    return this.http.delete<ServiceResult>(url, ids.map(id => id + ""), this._baseOptions);
  }
}
