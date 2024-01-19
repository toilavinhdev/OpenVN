import { Injectable } from "@angular/core";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { AssetService } from "./asset.service";

@Injectable({
  providedIn: 'root'
})
export class SpendingService extends AssetService {

  constructor(
    http: HttpService
  ) {
    super(http);

    this.controller = 'spending';
  }

}
