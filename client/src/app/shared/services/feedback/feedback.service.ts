import { Injectable } from "@angular/core";
import { BaseService } from "../base/base.service";
import { HttpService } from "../base/http.service";
import { CommonConstant } from "../../constants/common.constant";
import { Feedback } from "src/app/models/feedback/feedback";

@Injectable({
  providedIn: 'root'
})
export class FeedbackService extends BaseService {

  constructor(
    http: HttpService
  ) {
    super(http);
    this.serviceName = 'opensync';
    this.controller = 'feedback';
  }

  get() {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}?${CommonConstant.ALLOW_NOTICE_WITH_SNACKBAR_DANGER}`;
    return this.getAll(url);
  }

  send(feedback: Feedback) {
    const url = `${this.getBaseHost()}/${this.serviceName}/${this.controller}`;
    return this.saveOne(feedback, url);
  }
}
