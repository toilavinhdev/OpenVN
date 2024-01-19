import { Component, HostListener, Input, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { ButtonColor, ButtonType, IconButtonType } from '../../constants/button.constant';
import { FormMode, GroupBoxFieldType } from '../../enumerations/common.enum';
import { ActionExponent } from '../../enumerations/permission.enum';
import { GroupBoxField } from '../../../models/base/group-box-field.model';
import { GroupBox } from '../../../models/base/group-box.model';
import { ServiceResult } from '../../../models/base/service-result';
import { Message } from '../../../models/message';
import { SnackBarParameter } from '../../../models/snackbar.param';
import { BaseService } from '../../services/base/base.service';
import { Utility } from '../../utility/utility';
import { BaseComponent } from '../base-component';
import { BaseButton } from '../mp-button/mp-button.component';
import { SnackBar } from '../snackbar/snackbar.component';
import { BaseListConstant } from '../../constants/base-list.constant';
import { MessageBox } from '../message-box/message-box.component';

@Component({
  selector: 'mp-form-dynamic',
  templateUrl: './mp-form-dynamic.component.html',
  styleUrls: ['./mp-form-dynamic.component.scss']
})
export class MpFormDynamicComponent extends BaseComponent {

  FormFieldType = GroupBoxFieldType;
  ButtonColor = ButtonColor;
  ButtonType = ButtonType;
  IconButtonType = IconButtonType;
  FormMode = FormMode;

  @Input()
  groupBoxes: GroupBox[] = [];

  @Input()
  formMode = FormMode.None;

  @Input()
  serviceName = "";

  @Input()
  controller = "";

  @Input()
  backUrl!: string;

  @Input()
  title: {
    view: string,
    add: string,
    edit: string,
  } = { view: "", add: "", edit: "" }

  @Input()
  saveSuccessMessage = "Save successfully!";

  @Input()
  allowedFileExtensions = Utility.videoExtensions.map(i => `.${i}`).concat(Utility.imageExtensions.map(i => `.${i}`)).join(",");

  @Input()
  addPermissions: ActionExponent[] = [ActionExponent.Add];

  @Input()
  editPermissions: ActionExponent[] = [ActionExponent.Edit];

  @Input()
  deletePermissions: ActionExponent[] = [ActionExponent.Delete];

  @Input()
  editAndAddPermissions: ActionExponent[] = [ActionExponent.Add, ActionExponent.Edit];

  @ViewChild("saveBtn")
  saveBtn!: BaseButton;

  @ViewChild("saveAndAddBtn")
  saveAndAddBtn!: BaseButton;

  @ViewChild("saveBtnR")
  saveBtnR!: BaseButton;

  @ViewChild("saveAndAddBtnR")
  saveAndAddBtnR!: BaseButton;

  @ViewChild("deleteBtn")
  deleteBtn!: BaseButton;

  masterId!: string;

  masterData: any;

  isSaving = false;

  isCtrl = false;

  isShift = false;

  basePath = "";

  @HostListener('document:keyup', ['$event']) onKeyupHandler(event: KeyboardEvent) {
    if (event.key == 'Control') {
      this.isCtrl = false;
    }
    else if (event.key == 'Shift') {
      this.isShift = false;
    }
  }

  @HostListener('document:keydown', ['$event']) onKeydownHandler(event: KeyboardEvent) {
    if (this.isCtrl && !['a', 'c', 'z', 'x', 'y', 'v', 'n'].includes(event.key)) {
      event.preventDefault();
    }

    if (event.key == 'Control') {
      this.isCtrl = true;
    }
    else if (event.key == 'Shift') {
      this.isShift = true;
    }

    if (event.key.toLocaleLowerCase() == 's' && this.isCtrl && this.isShift) {
      this.onCtrlShiftS();
      return;
    }

    if (event.key.toLocaleLowerCase() == 's' && this.isCtrl) {
      this.onCtrlS();
      return;
    }

    if (event.key.toLocaleLowerCase() == 'b' && this.isCtrl) {
      this.onCtrlB();
      return;
    }
  }
  constructor(
    baseService: BaseService,
    public router: Router,
    public activatedRoute: ActivatedRoute,
  ) {
    super(baseService);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.getBasePath();
  }

  initData() {
    if (this.formMode === FormMode.View || this.formMode === FormMode.Update) {
      this.masterId = this.activatedRoute.snapshot.params["id"];
      this.initDataComboBoxes();
      this.getFormData();
    }
  }

  getBasePath() {
    const splits = this.activatedRoute.snapshot["_routerState"]["url"].split("/");
    if (splits && splits.length >= 2) {
      this.basePath = splits[1];
    }
  }

  initDataComboBoxes() {
    this.groupBoxes.forEach(groupBox => {
      var comboBoxes = groupBox.groupBoxFields.filter(field => field.type === GroupBoxFieldType.ComboBox);
      if (comboBoxes && comboBoxes.length) {
        comboBoxes.forEach(comboBox => {
          this.getComboboxData(comboBox);
        })
      }
    })
  }

  onCtrlB() {
    this.back();
  }

  onCtrlS() {
    this.save(false);
  }

  onCtrlShiftS() {
    this.save(true);
  }

  /**
   * Lấy dữ liệu form
   */
  getFormData() {
    this.baseService.serviceName = this.serviceName;
    this.baseService.controller = this.controller;
    this.isLoading = true;
    this.baseService.getById(this.masterId).pipe(takeUntil(this._onDestroySub)).subscribe(response => {
      this.isLoading = false;
      this.mapData(response.data);
      this.masterData = response.data;
    });
  }

  /**
   * Map dữ liệu từ groupboxes
   */
  mapData(data: any) {
    if (data) {
      const keys = Object.keys(data);
      keys.forEach(key => {
        this.groupBoxes.forEach(groupBox => {
          const mappingField = groupBox.groupBoxFields.find(f => f.fieldName === key);
          if (mappingField) {
            mappingField.value = data[key];
          }
        })
      });
    }
  }

  /**
   * Save dữ liệu
   */
  save(isSaveAndAdd = false) {
    if (this.isSaving) {
      return;
    }

    const valid = this.validateBeforeSave();
    if (!valid) {
      SnackBar.warning(new SnackBarParameter(this, "Please re-check information"));
      this.resetButtons();
      return;
    }

    // Save
    const url = `${this.baseService.getBaseHost()}/${this.serviceName}/${this.controller}/${this.formMode === FormMode.Add ? 'save' : 'update'}`;
    const data = this.getDynamicData();

    // Nếu là sửa thì đính kèm thêm id
    if (this.formMode === FormMode.Update) {
      (data as any).Id = this.masterId;
    }

    const api = this.formMode === FormMode.Add ? this.baseService.http.post<ServiceResult>(url, [data]) : this.baseService._http.put<ServiceResult>(url, data)

    this.isSaving = true;
    api.pipe(takeUntil(this._onDestroySub)).subscribe(response => {
      this.isSaving = false;
      this.resetButtons();

      if (response.success) {
        SnackBar.success(new SnackBarParameter(this, this.saveSuccessMessage, "", 1500));

        // Nếu không phải lưu và thêm thì redirect
        if (!isSaveAndAdd) {
          if (this.formMode === FormMode.Add) {
            const viewUrl = (this.activatedRoute.snapshot as any)._routerState.url.replace(`/${BaseListConstant.AddPath}`, `/${BaseListConstant.ViewPath}`);
            const id = response.data;
            this.router.navigate([`${viewUrl}/${id}`]);
          } else {
            const viewUrl = (this.activatedRoute.snapshot as any)._routerState.url.replace(`/${BaseListConstant.UpdatePath}`, `/${BaseListConstant.ViewPath}`);
            this.router.navigate([`${viewUrl}`]);
          }
        } else {
          this.resetData();

          if (this.formMode === FormMode.Update) {
            const index = (this.activatedRoute.snapshot as any)._routerState.url.indexOf(`/${BaseListConstant.UpdatePath}`);
            const addUrl = (this.activatedRoute.snapshot as any)._routerState.url.substring(0, index);
            this.router.navigate([`${addUrl}/${BaseListConstant.AddPath}`]);
          }
        }

      }
    },
      () => { this.resetButtons(); this.isSaving = false; }
    )
  }

  edit() {
    const path = (this.activatedRoute.snapshot as any)._routerState.url;
    const index = (path as string).indexOf(`/${BaseListConstant.ViewPath}`);
    const editUrl = `${(path as string).substring(0, index)}/${BaseListConstant.UpdatePath}/${this.masterId}`;

    this.router.navigate([`${editUrl}`]);
  }

  resetButtons() {
    this.saveBtn.isFinished = true;
    this.saveAndAddBtn.isFinished = true;
    this.saveBtnR.isFinished = true;
    this.saveAndAddBtnR.isFinished = true;
  }

  /**
   * Xác nhận xóa
   */
  confirmDelete() {
    MessageBox.confirmDelete(new Message(this, { content: "Are you sure you want to delete this record?" }, () => this.delete()));
    this.deleteBtn.isFinished = true;
  }

  delete() {
    this.baseService.delete([this.masterId]).pipe(takeUntil(this._onDestroySub)).subscribe(response => {
      if (response.success) {
        SnackBar.success(new SnackBarParameter(this, "Delete successfully"));
        const path = (this.activatedRoute.snapshot as any)._routerState.url;
        const index = (path as string).indexOf(`/${BaseListConstant.ViewPath}`);
        const viewUrl = `${(path as string).substring(0, index)}/${BaseListConstant.ListPath}`;
        this.router.navigate([`${viewUrl}`]);
      }
    });
  }

  /**
   * Lấy dữ liệu trong form dynamic
   */
  getDynamicData() {
    const dataForm = this.groupBoxes.map(g => g.groupBoxFields);
    const data = this.formMode == FormMode.Update ? this.masterData : {};
    dataForm.forEach(formFields => {
      formFields.forEach(f => {
        const obj: any = {};
        if (f.type === GroupBoxFieldType.Date) {
          obj[f.fieldName] = new Date(f.value);
        } else {
          obj[f.fieldName] = f.value;
        }
        Object.assign(data, obj);
      })
    });

    return data;
  }

  resetData() {
    this.groupBoxes.forEach(box => {
      box.groupBoxFields.forEach(g => {
        g.value = null;
        if (g.type === GroupBoxFieldType.Date) {
          g.value = new Date();
        }
      });
    });
  }

  back() {
    if (this.backUrl) {
      this.router.navigateByUrl(this.backUrl);
    } else {
      this.router.navigateByUrl(`/${this.basePath}`);
    }
  }
  validateBeforeSave() {
    // Validate required
    let valid = true;

    this.groupBoxes.forEach(g => g.groupBoxFields.forEach(f => f.error = false));
    const groupBoxFields = this.groupBoxes.map(g => g.groupBoxFields);
    for (let i = 0; i < groupBoxFields.length; i++) {
      const errorFields = groupBoxFields[i].filter(field => field.required && !field.value);
      if (errorFields && errorFields.length) {
        errorFields.forEach(errorField => {
          errorField.error = true;
          errorField.errorMessage = `${errorField.title} cannot be empty`;
        });
        valid = false;
      }
    }

    return valid;
  }

  getComboboxData(field: GroupBoxField) {
    if (field.pickList && (field.pickList || []).length > 0)
      return;

    const url = `${this.baseService.getBaseHost()}/${field.comboboxUrl}`;
    this.paginationRequest.pageSize = 500;

    field.isFetching = true;
    this.baseService.http.post<ServiceResult>(url, this.paginationRequest).pipe(takeUntil(this._onDestroySub)).subscribe(
      response => {
        field.isFetching = false;
        if (response.success) {
          field.pickList = response.data.map((item: any) => {
            if (!field.comboboxMap) {
              return {};
            }
            return {
              id: item[field.comboboxMap.id] + "",
              value: item[field.comboboxMap.value],
            }
          });
        }
      },
      () => field.isFetching = false
    )
  }
}
