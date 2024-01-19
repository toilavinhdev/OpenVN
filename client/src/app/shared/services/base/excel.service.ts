import { Injectable } from '@angular/core';
import { BaseMessageResponse } from '../../../models/base/base-message-response';
import { PaginationRequest } from '../../../models/base/pagination-request';
import { BaseService } from './base.service';
import { HttpService } from './http.service';
import { ServiceResult } from '../../../models/base/service-result';

@Injectable({
  providedIn: 'root'
})
export class ExcelService extends BaseService {

  serviceName = "";

  get apiExcelUrl() {
    return `${this.getBaseHost()}/${this.serviceName}/excel`;
  }
  constructor(
    http: HttpService
  ) {
    super(http);
  }

  /**
   * Get export key
   */
  public getExportKey(module = "", request: PaginationRequest) {
    const url = `${this.apiExcelUrl}/${module}/key`;
    return this.http.post<ServiceResult>(url, request);
  }


  /**
   * Export data theo key
   */
  public getExportUrl(module = "", key: string) {
    return `${this.apiExcelUrl}/${module}/export?key=${key}`;
  }

  /**
   * Get import template key
   */
  public getImportTemplateKey(module = "") {
    const url = `${this.apiExcelUrl}/${module}/import-template-key`;
    return this.http.post<BaseMessageResponse>(url, null);
  }

  /**
   * Download import template file
   */
  public downloadImportTemplate(module = "", key: string) {
    return `${this.apiExcelUrl}/${module}/download-template?key=${key}`;
  }

}
