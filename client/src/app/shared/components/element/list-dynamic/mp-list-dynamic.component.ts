import { ChangeDetectorRef, Component, EventEmitter, HostListener, Input, NgZone, Output, ViewChild } from '@angular/core';
import { DialogPosition, MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BaseListConstant } from '../../constants/base-list.constant';
import { IconButtonType } from '../../constants/button.constant';
import { BreakPoint } from '../../constants/common.constant';
import { FilterCondition, FormMode } from '../../enumerations/common.enum';
import { ActionExponent } from '../../enumerations/permission.enum';
import { ObjectHelper } from '../../helpers/object.helper';
import { StringHelper } from '../../helpers/string.helper';
import { BaseModel } from '../../../models/base/base-model';
import { ColumnGrid } from '../../../models/base/column-grid.model';
import { Field } from '../../../models/base/field-model';
import { Filter } from '../../../models/base/filter-model';
import { Filterable } from '../../../models/base/filterable-model';
import { PaginationRequest } from '../../../models/base/pagination-request';
import { ServiceResult } from '../../../models/base/service-result';
import { Message } from '../../../models/message';
import { SnackBarParameter } from '../../../models/snackbar.param';
import { BaseService } from '../../services/base/base.service';
import { Utility } from '../../utility/utility';
import { BaseComponent } from '../base-component';
import { BaseButton } from '../mp-button/mp-button.component';
import { MpFilterComponent } from '../mp-filter/mp-filter.component';
import { MpGridComponent } from '../mp-grid/mp-grid.component';
import { MessageBox } from '../message-box/message-box.component';
import { SnackBar } from '../snackbar/snackbar.component';
import { PopupChooseExportComponent } from './popup-choose-export/popup-choose-export.component';
import { TranslationService } from '../../services/base/translation.service';
import { ListDynamicOption } from '../../../models/base/list-dynamic-option';

@Component({
  selector: 'mp-list-dynamic',
  templateUrl: './mp-list-dynamic.component.html',
  styleUrls: ['./mp-list-dynamic.component.scss']
})
export class MpListDynamicComponent extends BaseComponent {

  FormMode = FormMode;

  Utility = Utility;

  IconButtonType = IconButtonType;

  @Input()
  options: ListDynamicOption = new ListDynamicOption();

  @Output()
  rowClick = new EventEmitter();

  @Output()
  rowDblClick = new EventEmitter();

  @ViewChild("grid")
  grid!: MpGridComponent;

  @ViewChild("deleteBtn")
  deleteBtn!: BaseButton;

  @ViewChild("addBtn")
  addBtn!: BaseButton;

  data: BaseModel[] = [];

  pageSizeOptions = [20, 50, 100];

  current = 0;

  total = 0;

  isShowDeleteBtn = false;

  selectedItemCount = 0;

  enableFilter = true;

  popupExportRef!: MatDialogRef<PopupChooseExportComponent>;

  popupFilterRef!: MatDialogRef<MpFilterComponent>;

  basePath = "";

  filterValue = "";

  filterable: Filterable[] = [];

  enableEmitScroll = true;

  timeout: any;

  isFirstLoad = true;

  isCtrl = false;

  isShift = false;

  isDeleting = false;

  isFilterFetching = false;

  virtualScroll = false;

  @HostListener('document:keyup', ['$event']) onKeyupHandler(event: KeyboardEvent) {
    if (!this.options.enabledKeyEvent) {
      return;
    }
    if (event.key == 'Control') {
      this.isCtrl = false;
    }
    else if (event.key == 'Shift') {
      this.isShift = false;
    }
  }

  @HostListener('document:keydown', ['$event']) onKeydownHandler(event: KeyboardEvent) {
    if (!this.options.enabledKeyEvent) {
      return;
    }

    if (this.isCtrl) {
      event.preventDefault();
    }

    if (event.key == 'Control') {
      this.isCtrl = true;
    }
    else if (event.key == 'Shift') {
      this.isShift = true;
    }

    if (event.key.toLocaleLowerCase() == 'a' && this.isCtrl && this.isShift) {
      this.onCtrlShiftA();
      return;
    }

    if (event.key.toLocaleLowerCase() == 'r' && this.isCtrl) {
      this.onCtrlR();
      return;
    }

    if (event.key.toLocaleLowerCase() == 'f' && this.isCtrl) {
      this.onCtrlF();
      return;
    }
  }
  constructor(
    baseService: BaseService,
    public router: Router,
    public dialog: MatDialog,
    public ngZone: NgZone,
    public cdr: ChangeDetectorRef,
    public activatedRoute: ActivatedRoute,
    public translationService: TranslationService,
  ) {
    super(baseService);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.getBasePath();
  }

  initData() {
    if (this.options.pagingUrl === '') {
      this.options.pagingUrl = `${this.baseService.getBaseHost()}/${this.options.serviceName}/${this.options.controller}/paging`;
    }
    this.getDataGrid();
    this.getFilterable();
  }

  getBasePath() {
    const splits = this.activatedRoute.snapshot["_routerState"]["url"].split("/");
    if (splits && splits.length >= 2) {
      this.basePath = splits[1];
    }
  }

  onCtrlShiftA() {
    this.toAddForm();
  }

  onCtrlR() {
    this.reload();
  }

  onCtrlF() {
    this.openFilter();
  }

  /**
   * Lấy dữ liệu grid
   */
  getDataGrid() {
    this.isLoading = true;
    this.enableEmitScroll = false;
    this.paginationRequest.filter = this.buildFilter();

    this.baseService.http.post<ServiceResult>(this.options.pagingUrl, this.paginationRequest)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        response => {
          this.isLoading = false;
          this.isFirstLoad = false;
          this.enableEmitScroll = true;

          if (response.status == "success" && response.data) {

            // virtual scroll
            if (this.grid.virtualScroll) {
              const dataClone = ObjectHelper.clone(this.data);
              this.data = dataClone;
              (response.data as Array<any>).forEach(item => {
                this.data.push(item);
              })
            }
            // traditional paging
            else {
              this.data = [];
              this.data = response.data;
            }

            this.current = this.data.length;
            this.total = response.total;
          } else {
            if (this.options.callbackGetDataFailed) {
              this.options.callbackGetDataFailed(response);
            }
          }
        },
        () => {
          this.isLoading = false;
          this.enableEmitScroll = true;
        }
      )
  }

  getFilterable() {
    this.isFilterFetching = true;

    const url = `${this.baseService.getBaseHost()}/${this.options.serviceName}/${this.options.controller}/filterable`;
    this.baseService.http.get<ServiceResult>(url)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        response => {
          this.isFilterFetching = false;
          if (response.status == "success") {
            this.filterable = response.data;
          }
        },
        () => this.isFilterFetching = false
      );
  }

  reload() {
    if (!this.isLoading) {
      this.paginationRequest = new PaginationRequest();
      this.data = [];
      this.current = 0;
      this.total = 0;
      this.isFirstLoad = true;
      this.grid.changeAllCheckBox(false);
      this.grid.table.nativeElement.scrollTop = 0; // reset scroll position
      this.getDataGrid();
    }
  }

  prepareConfig() {
    const config = new MatDialogConfig();
    const position: DialogPosition = {};
    position.top = '100px';

    config.maxWidth = '80%';
    config.maxHeight = `${window.innerHeight * 0.8}px`;
    config.position = position;

    return config;
  }

  openPopupExport() {
    const config = this.prepareConfig();
    config.data = new Message(this, { service: this.options.serviceName }, () => this.popupExportRef.close());
    this.popupExportRef = this.dialog.open(PopupChooseExportComponent, config);
  }

  onScrollToPosition(e: any) {
    if (!this.enableEmitScroll || this.current >= this.total)
      return;

    this.paginationRequest.pageIndex++;
    this.getDataGrid();
  }

  changePage(event) {
    if (event.pageSize != this.paginationRequest.pageSize) {
      this.paginationRequest.pageSize = event.pageSize;
      this.paginationRequest.pageIndex = 0;
    } else {
      this.paginationRequest.pageIndex = event.pageIndex;
    }

    this.getDataGrid();
  }

  decideToShowButtons(e: any) {
    if (this.grid.hasCheckedItem()) {
      this.isShowDeleteBtn = true;
      this.enableFilter = false;
      this.selectedItemCount = this.grid.getCheckedItems().length;
    } else {
      this.isShowDeleteBtn = false;
      this.enableFilter = true;
      this.selectedItemCount = 0;
    }
  }

  confirmDelete() {
    MessageBox.confirmDelete(new Message(this, { content: TranslationService.VALUES["COMMON"]["WARNING_DELETE_SUCCESS_MSG"] }, () => {
      this.deleteCheckedItems();
    }));
    this.deleteBtn.isFinished = true;
  }

  deleteCheckedItems() {
    this.isDeleting = true;

    const ids = this.grid.getCheckedItems().map(item => item.id);
    const url = !StringHelper.isNullOrEmpty(this.options.deleteUrl) ? this.options.deleteUrl : `${this.baseService.getBaseHost()}/${this.options.serviceName}/${this.options.controller}/delete`;

    this.baseService.delete(ids, url)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        response => {
          this.isDeleting = false;
          if (response.status == "success") {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["DELETE_SUCCESS_MSG"]));
            this.reload();
          }
        },
        () => this.isDeleting = false
      );
  }

  toAddForm() {
    this.addBtn.isFinished = true;
    if (this.options.customizeAddFunc != null) {
      this.options.customizeAddFunc();
    } else {

      if (StringHelper.isNullOrEmpty(this.options.customizeAddUrl)) {
        // this.router.navigateByUrl(`/${this.controller.replace(/[A-Z]/g, letter => `-${letter.toLowerCase()}`)}/${BaseListConstant.FormAddPath}`);
        this.router.navigateByUrl(`/${this.basePath}/${BaseListConstant.AddPath}`);
      } else {
        this.router.navigateByUrl(this.options.customizeAddUrl);
      }
    }
  }

  toViewForm(event: any) {
    this.rowDblClick.emit(event);
    if (!this.options.enableRowDblclick)
      return;

    if (this.options.customizeViewFunc != null) {
      this.options.customizeViewFunc(event);
    } else {
      setTimeout(() => {
        if (StringHelper.isNullOrEmpty(this.options.customizeViewUrl)) {
          this.router.navigateByUrl(`/${this.basePath}/${BaseListConstant.ViewPath}/${event.id}`);
        } else {
          this.router.navigateByUrl(this.options.customizeViewUrl);
        }
      }, 150);
    }
  }

  onRowClick(e: any) {
    this.rowClick.emit(e);
  }

  onClickEdit(event: any) {
    if (!this.options.customizeEditFunc) {
      this.router.navigateByUrl(`/${this.basePath}/${BaseListConstant.UpdatePath}/${event.id}`);
    } else {
      this.options.customizeEditFunc(event);
    }
  }

  /**
   * In màn hình
   */
  print() {
    window.print();
  }

  sortGrid(e: any) {
    this.paginationRequest = new PaginationRequest();
    this.paginationRequest.sorts = e.sorts;
    this.data = [];
    this.getDataGrid();
  }

  /**
   * Mở popup filter
   */
  openFilter() {
    Utility.featureIsInDevelopment(null);
    return;

    const config = this.prepareConfig();
    if (window.innerWidth <= BreakPoint.SM) {
      config.width = `${window.innerWidth * 0.8}px`;
      config.height = `${window.innerHeight * 0.8}px`;
    } else {
      config.width = `${window.innerWidth * 0.7}px`;
      config.height = `${window.innerHeight * 0.7}px`;
    }

    this.popupFilterRef = this.dialog.open(MpFilterComponent, config);
    const instance = this.popupFilterRef.componentInstance;
    instance.filterable = this.filterable;
  }

  filter() {
    this.reload();
  }


  buildFilter() {
    const result = new Filter();
    const arr = [];

    for (let i = 0; i < this.filterable.length; i++) {
      result.fields.push(new Field(this.filterable[i].key, this.filterValue, FilterCondition.C));
      arr.push(`{${i}}`);
    }
    result.formula = arr.join(" OR ");

    return result;
  }
}


export class DynamicFunction {
  public onClick: Function;
  public tooltip = "";
  public text = "";
  public iconPosition = "";
  public style?= {};
  public isShowIcon?= true;
}
