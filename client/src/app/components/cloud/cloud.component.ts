import { Component, ElementRef, HostListener, Injector, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatMenuTrigger } from '@angular/material/menu';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, forkJoin, of } from 'rxjs';
import { catchError, switchMap, takeUntil } from 'rxjs/operators';
import { ErrorModel } from 'src/app/models/base/error';
import { ServiceResult } from 'src/app/models/base/service-result';
import { CloudFileUI } from 'src/app/models/cloud/cloud-file';
import { CopyCut } from 'src/app/models/cloud/copy-cut';
import { Directory, DirectoryUI } from 'src/app/models/cloud/directory';
import { Path } from 'src/app/models/cloud/path';
import { Message } from 'src/app/models/message';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { MessageBox } from 'src/app/shared/components/element/message-box/message-box.component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { RequestErrorMapping } from 'src/app/shared/core/request-error-handler/request-mapping-handler';
import { RequestErrorType } from 'src/app/shared/core/request-error-handler/request-type';
import { FileType } from 'src/app/shared/enumerations/file.enum';
import { StringHelper } from 'src/app/shared/helpers/string.helper';
import { PopupService } from 'src/app/shared/services/base/popup.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { AuthCloudService } from 'src/app/shared/services/cloud/auth-cloud.service';
import { CloudFileService } from 'src/app/shared/services/cloud/cloud-file.service';
import { DirectoryService } from 'src/app/shared/services/cloud/directory.service';
import { TransferCloudService } from 'src/app/shared/services/cloud/transfer-cloud.service';
import { Utility } from 'src/app/shared/utility/utility';
import { CloudEnterPasswordComponent } from '../z-popup/cloud/cloud-enter-password/cloud-enter-password.component';
import { CloudSetPasswordComponent } from '../z-popup/cloud/cloud-set-password/cloud-set-password.component';
import { CloudUploaderComponent } from '../z-popup/cloud/cloud-uploader/cloud-uploader.component';
import { SecretCode } from 'src/app/models/cloud/secret-code';
import { CapacityConfiguration } from 'src/app/models/cloud/capacity-configuration';
import { MovingProgessComponent } from '../z-popup/cloud/moving-progess/moving-progess.component';
import { CountInformation } from 'src/app/models/cloud/count-information';
import { DeviceType } from 'src/app/shared/enumerations/device.enum';
import { HubConnectionService } from 'src/app/shared/services/base/hub-connection.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';

@Component({
  selector: 'app-cloud',
  templateUrl: './cloud.component.html',
  styleUrls: ['./cloud.component.scss']
})
export class CloudComponent extends BaseComponent {
  Utility = Utility;

  FileType = FileType;

  directory = new Directory();

  directories: DirectoryUI[] = [];

  cloudFiles: CloudFileUI[] = [];

  entity = new Directory();

  entityClone = new DirectoryUI();

  path = new Path();

  menuTopLeftPosition = { x: '0', y: '0' };

  isAdding = false;

  isSaving = false;

  isLoadingDir = false;

  isLoadingCF = false;

  isLoadingCapacity = false;

  isLoadingProperties = false;

  isValidCopy = false;

  lastDirId = "";

  loadingBlockCount = Math.max(Math.floor((window.innerWidth - 32) / 180) * 2 - 3, 1);

  contextState = {
    onDir: false,
    onCf: false,
    onItem: () => this.contextState.onCf || this.contextState.onDir,
    hasSelected: () => this.hasDirSelected() || this.hasCfSelected(),
    setClickOnDir: () => { this.contextState.onDir = true; this.contextState.onCf = false },
    setClickOnCf: () => { this.contextState.onDir = false; this.contextState.onCf = true },
  };

  cloudUploaderRef: MatDialogRef<CloudUploaderComponent>;

  validateRef: MatDialogRef<CloudEnterPasswordComponent>;

  movingProgressRef: MatDialogRef<MovingProgessComponent>;

  shouldRefresh = false;

  isCtrl = false;

  isShowPaste = false;

  isMouseDown = false;

  popupOpening = false;

  capacityConfiguration = new CapacityConfiguration();

  countInformation = new CountInformation();

  properties: any = {};

  dragItem: DirectoryUI | CloudFileUI;

  dragType = "";

  timeoutHandler: any;

  perDirectorySize = 0;

  perFileSize = 0;

  directorySize = 0;

  fileSize = 0;

  @ViewChild(MatMenuTrigger, { static: true }) matMenuTrigger: MatMenuTrigger;

  @ViewChild("newDirInput")
  newDirInput: ElementRef;

  @ViewChild("renameDirInput")
  renameDirInput: ElementRef;

  @HostListener('document:keydown', ['$event']) onKeydownHandler(event: KeyboardEvent) {
    if (this.popupOpening) {
      return;
    }

    const key = event.key.toLocaleLowerCase();
    if (this.isCtrl && ['a', 'c', 'x'].includes(key)) {
      event.preventDefault();
    }

    switch (key) {
      case 'control':
        this.isCtrl = true;
        break;
      case 'a':
        if (this.isCtrl) {
          if (this.directories.find(x => !x.selected) || this.cloudFiles.find(x => !x.selected)) {
            this.directories.forEach(x => x.selected = true);
            this.cloudFiles.forEach(x => x.selected = true);
          } else {
            this.directories.forEach(x => x.selected = false);
            this.cloudFiles.forEach(x => x.selected = false);
          }
        }
        break;
      case 'delete':
        if (this.contextState.hasSelected()) {
          this.confirmCoupleDelete();
        }
        break;
      case 'c':
        if (this.isCtrl) {
          SnackBar.warning(new SnackBarParameter(this, "Chưa hỗ trợ copy"));
        }
        // this.copy();
        break;
      case 'x':
        if (this.isCtrl) {
          this.cut();
        }
        break;
      case 'v':
        if (this.isCtrl) {
          this.paste();
        }
        break;

      default:
        break;
    }
  }

  @HostListener('document:keyup', ['$event']) onKeyupHandler(event: KeyboardEvent) {
    if (event.key == 'Control') {
      this.isCtrl = false;
    }
  }

  @HostListener('document:mousedown', ['$event']) onMouseDownHandler(event: KeyboardEvent) {
    this.isMouseDown = true;
  }

  @HostListener('document:mouseup', ['$event']) onMouseUpHandler(event: KeyboardEvent) {
    this.isMouseDown = false;
  }
  constructor(
    public injector: Injector,
    public directoryService: DirectoryService,
    public cloudFileService: CloudFileService,
    public authCloudService: AuthCloudService,
    public transferCloudService: TransferCloudService,
    public popupService: PopupService,
    public activatedRoute: ActivatedRoute,
    public router: Router,
    public dialog: MatDialog,
    public hubService: HubConnectionService,
    public transferService: TransferDataService
  ) {
    super(injector);
  }

  //#region  data
  ngOnInit(): void {
    super.ngOnInit();
    this.onReceivedFileFromServer();
    this.onValidatePassword = this.onValidatePassword.bind(this);

    RequestErrorMapping.mapping.push({
      type: RequestErrorType.CLOUD_DIR_CODE,
      func: (error: ErrorModel, body) => {
        this.checkBadRequest(error, body);
      }
    });


  }

  initData() {
    super.initData();
    let dirId = this.activatedRoute.snapshot.params['dirId'];
    if (!Utility.isNumber(dirId)) {
      dirId = "0";
    }
    this.directory.id = dirId;
    this.directory.name = "C:";

    this.initSize();
    this.fetchDir();
    this.fetchPath();
    this.fetchCapacity();
    this.fetchCountInformation();

    window.addEventListener('resize', () => {
      this.initSize();
    });
  }

  initSize() {
    this.perDirectorySize = Math.max(10, Math.floor((window.innerWidth - 32) / 172) * 2);
    this.perFileSize = this.perDirectorySize;
    this.directorySize = this.perDirectorySize;
    this.fileSize = this.perFileSize;
  }

  onReceivedFileFromServer() {
     this.transferService.receivedFileEvent
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.cloudFiles = this.cloudFiles.concat(resp.message);
      });
  }

  fetchPath() {
    this.directoryService.getPath(this.directory.id)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        if (resp.status == 'success') {
          this.path = resp.data;
        }
      });
  }

  fetchCapacity() {
    this.isLoadingCapacity = true;
    this.cloudFileService.getCloudConfiguration()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoadingCapacity = false;
          if (resp.status == 'success') {
            this.capacityConfiguration = resp.data;
            this.cloudFileService.configuration = resp.data;
          }
        },
        _ => this.isLoadingCapacity = false
      );
  }

  fetchDir() {
    if (parseInt(this.directory.id) > 0) {
      this.isLoading = true;
      this.directoryService.getById(this.directory.id)
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(resp => {
          this.isLoading = false;
          if (resp.status == 'success' && resp.data) {
            this.directory = resp.data;
            this.findSecretCodeAndSave();
            this.loadData();
            // Utility.changeTitle(this.directory.name);
          } else {
            MessageBox.information(new Message(this, { content: TranslationService.VALUES['COMMON']['DATA_DOES_NOT_EXIST_OR_WAS_DELETED'] })).subscribe(_ => {
              this.router.navigateByUrl(`/${this.Routing.DIRECTORY.path}`);
              setTimeout(() => {
                location.reload();
              }, 200);
            });
          }
        },
          _ => this.router.navigateByUrl(`/${this.Routing.DIRECTORY.path}`)
        );
    } else {
      this.loadData();
    }
  }

  fetchCountInformation() {
    this.cloudFileService
      .getCountInformation()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        if (resp.status == 'success') {
          this.countInformation = resp.data;
        }
      })
  }

  loadData() {
    this.isLoadingDir = true;
    this.isLoadingCF = true;
    this.directories = [];
    this.cloudFiles = [];

    this.directoryService.getChildrenDirectory(this.directory.id)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoadingDir = false;
          if (resp.status == 'success') {
            this.directories = resp.data;
            this.directories.forEach(x => {
              x.canChangeSelected = true;
              if (this.directoryService.isUnlocked(x.id)) {
                x.hasUnlocked = true;
              }
            });
            this.findSecretCodeAndSave();
          }
        },
        _ => this.isLoadingDir = false
      );
    this.fetchFiles();
  }

  fetchFiles() {
    this.cloudFileService.getFilesInDir(this.directory.id, this.hubService.connection.connectionId)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoadingCF = false;
          if (resp.status == 'success') {
            this.cloudFiles = resp.data;
          }
        },
        _ => this.isLoadingCF = false
      );
  }

  softRefresh() {
    this.directories = this.directories.filter(x => !x.selected);
    this.cloudFiles = this.cloudFiles.filter(x => !x.selected);
  }

  refresh() {
    this.lastDirId = '';
    this.loadData();
    this.fetchPath();
    this.fetchCapacity();
    this.fetchCountInformation();
  }

  getFileProperties(event, cf: CloudFileUI) {
    event.stopPropagation();
    event.preventDefault();

    this.properties = {};
    this.isLoadingProperties = true;
    this.cloudFileService.getProperties(cf.id, this.directory.id)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoadingProperties = false;
          if (resp.status == 'success' && resp.data) {
            this.properties = resp.data;
          }
        },
        _ => this.isLoadingProperties = false
      )
  }

  getDirProperties(event, dir: DirectoryUI) {
    event.stopPropagation();
    event.preventDefault();

    event.stopPropagation();
    event.preventDefault();

    this.properties = {};
    this.isLoadingProperties = true;
    this.directoryService.getProperties(dir.id, this.directory.id)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoadingProperties = false;
          if (resp.status == 'success' && resp.data) {
            this.properties = resp.data;
          }
        },
        _ => this.isLoadingProperties = false
      )
  }
  //#endregion

  //#region context
  hasDirSelected() {
    return this.directories.find(dir => dir.selected) != null;
  }

  hasCfSelected() {
    return this.cloudFiles.find(cf => cf.selected) != null;
  }

  openInNewTab(event, dir: DirectoryUI) {
    if (dir.isLocked && !dir.hasUnlocked) {
      this.enterPassword(dir, this.onValidatePassword);
    }
    else {
      window.open(`/${this.Routing.DIRECTORY.path}/${dir.id}`);
    }
  }

  unSelectAll() {
    this.directories.forEach(x => x.selected = false);
    this.cloudFiles.forEach(x => x.selected = false);
  }

  onDirClick(event, item: DirectoryUI) {
    event.preventDefault();
    event.stopPropagation();

    if (event.ctrlKey) {
      item.selected = !item.selected;
    } else {
      this.unSelectAll();
      item.selected = true;
    }
    this.contextState.onDir = true;
    this.contextState.onCf = false;
  }

  onCfClick(event, item: CloudFileUI) {
    event.preventDefault();
    event.stopPropagation();

    if (event.ctrlKey) {
      item.selected = !item.selected;
    } else {
      this.unSelectAll();
      item.selected = true;
    }
    this.contextState.onDir = false;
    this.contextState.onCf = true;
  }

  outsideClick(event) {
    this.contextState.onDir = false;
    this.contextState.onCf = false;
    this.unSelectAll();
  }

  onDirDblClick(event, dir: DirectoryUI) {
    event.stopPropagation();
    event.preventDefault();
    if (dir.isLocked && !dir.hasUnlocked) {
      this.enterPassword(dir, this.onValidatePassword);
    }
    else {
      window.location.href = `/${this.Routing.DIRECTORY.path}/${dir.id}`;
    }
  }

  onCfDblClick(event, cf: CloudFileUI) {
    this.openCf(cf.url);
  }

  // download(url) {
  //   const iframe = (document.getElementById('download') as any);
  //   iframe.href = url;
  //   iframe.click();
  // };

  openCf(url) {
    window.open(url);
  }

  openMenu(event: MouseEvent, item) {
    this.checkValidPaste();
    this.menuTopLeftPosition.x = event.clientX + 'px';
    this.menuTopLeftPosition.y = event.clientY + 'px';
    this.matMenuTrigger.menuData = { item: item };
    this.matMenuTrigger.openMenu();
  }

  dblClickOutside(event: MouseEvent) {
    event.preventDefault();
    event.stopPropagation();
    if (SharedService.DeviceType == DeviceType.Mobile) {
      this.onOutsideRightClick(event);
    }
  }

  onOutsideRightClick(event: MouseEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.contextState.onDir = false;
    this.contextState.onCf = false;
    this.openMenu(event, null);
  }

  onDirRightClick(event: MouseEvent, item) {
    event.preventDefault();
    event.stopPropagation();

    this.contextState.setClickOnDir();
    this.openMenu(event, item);
  }

  onCfRightClick(event: MouseEvent, item) {
    event.preventDefault();
    event.stopPropagation();

    this.contextState.setClickOnCf();
    this.openMenu(event, item);
  }

  mouseup(event) {
    event.preventDefault();
    event.stopPropagation();
    if (this.timeoutHandler) {
      clearTimeout(this.timeoutHandler);
      this.timeoutHandler = null;
    }
  }

  outsideMousedown(event) {
    event.preventDefault();
    event.stopPropagation();
    if (SharedService.DeviceType == DeviceType.Mobile) {
      this.timeoutHandler = setTimeout(() => {
        this.onOutsideRightClick(event);
        this.timeoutHandler = null;
      }, 200);
    }
  }

  dirMousedown(event, dir: DirectoryUI) {
    event.preventDefault();
    event.stopPropagation();
    this.timeoutHandler = setTimeout(() => {
      this.onDirRightClick(event, dir);
      this.timeoutHandler = null;
    }, 200);
  }

  fileMousedown(event, cf: CloudFileUI) {
    event.preventDefault();
    event.stopPropagation();
    this.timeoutHandler = setTimeout(() => {
      this.onCfRightClick(event, cf);
      this.timeoutHandler = null;
    }, 200);
  }
  //#endregion

  //#region save
  new() {
    this.entity = new Directory();
    this.isAdding = true;
    setTimeout(() => {
      this.newDirInput.nativeElement.focus();
    }, 100);
  }

  save() {
    if (this.isSaving) {
      return;
    }
    this.isSaving = true;

    this.isLoading = true;
    this.entity.parentId = this.directory.id;
    this.directoryService.saveOne(this.entity)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isAdding = false;
          this.isLoading = false;
          this.isSaving = false;
          if (resp.status == 'success') {
            this.lastDirId = resp.data.id;
            if (!StringHelper.isNullOrEmpty(resp.data.path)) {
              // x
              // const unlockedList = this.directoryService.getObjectCode();
              // const nodes = resp.data.path.split("-");
              // for (let i = nodes.length - 1; i >= 0; i--) {
              //   if (!StringHelper.isNullOrEmpty(unlockedList[nodes[i]])) {
              //     unlockedList[resp.data.id] = unlockedList[nodes[i]];

              //     this.directoryService.setUnlockedList(unlockedList);
              //     break;
              //   }
              // }
            }
            this.refresh();
          }
        },
        _ => { this.isLoading = false; this.isAdding = false; this.isSaving = false; }
      )
  }

  onBlur() {
    if (this.entity.name.trim() == '') {
      this.isAdding = false;
      return;
    }
    this.save();
  }

  onKeyup(event) {
    if (event.key == 'Enter') {
      this.save();
    }
  }
  //#endregion

  //#region edit
  editDir(dir: DirectoryUI) {
    if (JSON.stringify(this.entityClone) == JSON.stringify(dir)) {
      dir.isEditing = false;
      return;
    }
    this.isLoading = true;
    this.directoryService.update(dir)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.isLoading = false;
          dir.isEditing = false;
          if (resp.status == 'success') {
            this.lastDirId = dir.id;
            this.loadData();
          }
        },
        _ => { this.isLoading = false; dir.isEditing = false; }
      )
  }

  onRenameDirKeyup(event, dir: DirectoryUI) {
    if (event.key == 'Enter') {
      this.editDir(dir);
    }
  }

  rename(dir: DirectoryUI) {
    this.directories.forEach(dir => dir.isEditing = false);
    dir.isEditing = true;

    this.entityClone = JSON.parse(JSON.stringify(dir));
    setTimeout(() => {
      this.renameDirInput.nativeElement.select();
    }, 100);
  }
  // #endregion

  //#region delete
  confirmDirDelete(dir: DirectoryUI) {
    this.lastDirId = '';
    MessageBox.confirmDelete(new Message(this, { content: TranslationService.VALUES["COMMON"]["WARNING_DELETE_SUCCESS_MSG"] }, () => {
      dir.isUpdating = true;
      this.directoryService
        .delete([dir.id])
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(
          resp => {
            if (resp.status == 'success') {
              SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["DELETE_SUCCESS_MSG"], 2000));
              this.directories = this.directories.filter(x => x.id != dir.id);
            }
          },
          _ => dir.isUpdating = false
        );
    }));
  }

  confirmCfDelete(cf: CloudFileUI) {
    this.lastDirId = '';
    MessageBox.confirmDelete(new Message(this, { content: TranslationService.VALUES["COMMON"]["WARNING_DELETE_SUCCESS_MSG"] }, () => {
      cf.isDeleting = true;
      this.cloudFileService
        .deletes(this.directory.id, [cf.id])
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(
          resp => {
            cf.isDeleting = false;
            if (resp.status == 'success') {
              SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["DELETE_SUCCESS_MSG"], 2000));
              this.cloudFiles = this.cloudFiles.filter(x => x.id != cf.id);
            }
          },
          _ => cf.isDeleting = false
        )
    }));
  }

  confirmCoupleDelete() {
    this.lastDirId = '';
    MessageBox.confirmDelete(new Message(this, { content: TranslationService.VALUES["COMMON"]["WARNING_DELETE_SUCCESS_MSG"] }, () => {
      const selectedCf = this.cloudFiles.filter(x => x.selected);
      const selectedDir = this.directories.filter(x => x.selected);
      const tasks: Observable<ServiceResult>[] = [];

      this.changeDeletingStatus(true);
      if (selectedCf.length) {
        tasks.push(this.cloudFileService.deletes(this.directory.id, selectedCf.map(x => x.id)).pipe(takeUntil(this._onDestroySub)));
      }
      if (selectedDir.length) {
        tasks.push(this.directoryService.delete(selectedDir.map(x => x.id)).pipe(takeUntil(this._onDestroySub)));
      }
      forkJoin(tasks)
        .subscribe(resp => {
          this.changeDeletingStatus(false);
          if (resp.every(x => x.status == 'success')) {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["COMMON"]["DELETE_SUCCESS_MSG"], 2000));
            this.softRefresh();
          }
        },
          _ => this.changeDeletingStatus(false)
        )
    }));
  }

  changeDeletingStatus(value: boolean) {
    this.cloudFiles.forEach(x => {
      if (x.selected) {
        x.isDeleting = value;
      }
    });

    this.directories.forEach(x => {
      if (x.selected) {
        x.isUpdating = value;
      }
    });
  }
  // #endregion

  //#region security
  isLockedDir(dir: DirectoryUI) {
    return dir.isLocked && !dir.hasUnlocked;
  }

  lock(event, dir: DirectoryUI) {
    dir.isUpdating = true;
    this.authCloudService.lock(dir.id)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          dir.isUpdating = false;
          if (resp.status == 'success') {
            dir.isLocked = true;
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['CLOUD']['DIRECTORY']['LOCK_SUCCESS_MSG']));
          }
        },
        _ => dir.isUpdating = false
      );
  }

  unlock(event, dir: DirectoryUI) {
    this.enterPassword(dir, (ref) => {
      dir.isUpdating = true;
      this.authCloudService.unlock(dir.id, ref.password)
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(
          resp => {
            dir.isUpdating = false;
            if (resp.status == 'success') {
              dir.isLocked = false;
              this.validateRef.close();
              SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['CLOUD']['DIRECTORY']['UNLOCK_SUCCESS_MSG']));
            } else {
              ref.nextBtn.isFinished = true;
            }
          },
          _ => dir.isUpdating = false
        );
    })
  }

  setPassword(event, dir: DirectoryUI) {
    if (this.popupOpening) {
      return;
    }
    this.popupOpening = true;
    const config = this.popupService.maxPingConfig(440, 64);
    config.panelClass = 'c-set-password';
    config.position = { top: '96px' };
    config.data = {
      dir: dir ?? this.directory,
      ref: this
    };

    this.dialog.open(CloudSetPasswordComponent, config)
      .afterClosed()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(_ => this.popupOpening = false);
  }

  changePassword(event, dir: DirectoryUI) {
    Utility.featureIsInDevelopment(event);
  }

  enterPassword(dir: Directory, callback: Function, otherDir?: Directory) {
    if (this.popupOpening) {
      return;
    }
    this.popupOpening = true;
    const config = this.popupService.maxPingConfig(440, 64);
    config.panelClass = 'c-enter-password';
    config.position = { top: '96px' };
    config.data = {
      dir: dir ?? this.directory,
      ref: this,
      otherDir: otherDir,
      callback: callback
    };

    this.validateRef = this.dialog.open(CloudEnterPasswordComponent, config);
    this.validateRef
      .afterClosed()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(_ => this.popupOpening = false)
  }

  onValidatePassword(ref) {
    this.authCloudService.genCode(ref.directory.id, ref.password)
      .pipe(
        takeUntil(this._onDestroySub),
        catchError(error => {
          ref.nextBtn.isFinished = true;
          throw Error(error);
        }),
        switchMap((resp: ServiceResult) => {
          if (resp.status == 'success') {
            const objectCode = this.directoryService.getObjectCode();
            const code = new SecretCode();
            code.value = resp.data;
            code.expiry = new Date().getTime() + 15 * 60000;

            objectCode[ref.directory.id] = code;
            this.directoryService.setUnlockedList(objectCode);
            return this.directoryService.getChildrenNodeId(ref.directory.id)
          }
          return of(resp);
        })
      )
      .pipe(
        takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          ref.nextBtn.isFinished = true;
          if (resp.status == 'success') {
            if (resp.data.length) {
              const objectCode = this.directoryService.getObjectCode();
              resp.data.forEach(x => {
                const code = objectCode[ref.directory.id];
                code.expiry = new Date().getTime() + 15 * 60000;
                objectCode[x] = code;
              });
              this.directoryService.setUnlockedList(objectCode);
            }
            window.location.href = `/${this.Routing.DIRECTORY.path}/${ref.directory.id}`;
          }
        },
        _ => ref.nextBtn.isFinished = true
      );
  }

  checkBadRequest(error: ErrorModel, body: any) {
    if (body) {
      const dir = new Directory();
      dir.id = body.key;
      dir.name = body.name;
      this.enterPassword(this.directory, this.onValidatePassword, dir);
    }
    else {
      this.enterPassword(this.directory, this.onValidatePassword);
    }
  }

  findSecretCodeAndSave() {
    const objectCode = this.directoryService.getObjectCode();

    if (!StringHelper.isNullOrEmpty(this.directory.path) && !this.directory.isLocked) {
      const values = this.directory.path.split('-');
      for (let i = values.length - 1; i >= 0; i--) {
        const obj = objectCode[values[i]];
        if (obj && !objectCode[this.directory.id]) {
          objectCode[this.directory.id] = obj;
          this.directoryService.setUnlockedList(objectCode);
          break;
        }
      }
    }

    this.directories.forEach(x => {
      if (!StringHelper.isNullOrEmpty(x.path) && !x.isLocked) {
        const values = x.path.split('-');
        for (let i = values.length - 1; i >= 0; i--) {
          const obj = objectCode[values[i]];
          if (obj && !objectCode[x.id]) {
            objectCode[x.id] = obj;
            this.directoryService.setUnlockedList(objectCode);
            break;
          }
        }
      }
    });
  }
  //#endregion

  //#region copy-cut
  createCopyCutObject(obj: DirectoryUI | CloudFileUI, isDir: boolean, isCopy: boolean) {
    obj.underCopyOrCut = true;
    obj.selected = false;

    const copyCut = new CopyCut();
    copyCut.from = this.directory.id;
    copyCut.sourceSecretCode = this.directoryService.getObjectCode()[this.directory.id]?.value;
    copyCut.isCopy = isCopy;
    copyCut.moveObjects = [{ type: isDir ? "dir" : "cf", sourceId: obj.id }];

    return copyCut;

  }

  copyDir(dir: DirectoryUI) {
    navigator.clipboard.writeText(JSON.stringify(this.createCopyCutObject(dir, true, true)));
  }

  copyCf(cf: CloudFileUI) {
    navigator.clipboard.writeText(JSON.stringify(this.createCopyCutObject(cf, false, true)));
  }

  copy() {
    const copyCut = new CopyCut();
    copyCut.from = this.directory.id;
    copyCut.isCopy = true;

    const dirs = this.directories.filter(x => x.selected).forEach(x => {
      x.underCopyOrCut = true;
      x.selected = false;
      copyCut.moveObjects.push({ type: "dir", sourceId: x.id });
    });

    const cfs = this.cloudFiles.filter(x => x.selected).forEach(x => {
      x.underCopyOrCut = true;
      x.selected = false;
      copyCut.moveObjects.push({ type: "cf", sourceId: x.id });
    });
    navigator.clipboard.writeText(JSON.stringify(copyCut));
  }

  cutDir(dir: DirectoryUI) {
    navigator.clipboard.writeText(JSON.stringify(this.createCopyCutObject(dir, true, false)));
  }

  cutCf(cf: CloudFileUI) {
    navigator.clipboard.writeText(JSON.stringify(this.createCopyCutObject(cf, false, false)));
  }

  cut() {
    const copyCut = new CopyCut();
    copyCut.from = this.directory.id;
    copyCut.sourceSecretCode = this.directoryService.getObjectCode()[this.directory.id]?.value;
    copyCut.isCopy = false;

    const dirs = this.directories.filter(x => x.selected).forEach(x => {
      x.underCopyOrCut = true;
      x.selected = false;
      copyCut.moveObjects.push({ type: "dir", sourceId: x.id });
    });

    const cfs = this.cloudFiles.filter(x => x.selected).forEach(x => {
      x.underCopyOrCut = true;
      x.selected = false;
      copyCut.moveObjects.push({ type: "cf", sourceId: x.id });
    });
    navigator.clipboard.writeText(JSON.stringify(copyCut));
  }

  checkValidPaste() {
    if (SharedService.DeviceType == DeviceType.Desktop) {
      navigator.clipboard.readText()
        .then(resp => {
          try {
            const content = JSON.parse(resp);
            console.customize(content as CopyCut);
            this.isShowPaste = true;
          } catch (error) {
            this.isShowPaste = false;
          }
        });
    }
  }

  pasteToDir(dir: DirectoryUI) {
    this.paste(dir.id);
  }

  paste(destinationId?: string) {
    navigator.clipboard.readText().then(result => {
      try {
        this.isLoading = true;
        this.openMovingProgess();

        const content = JSON.parse(result) as CopyCut;
        content.destinationId = destinationId ?? this.directory.id;
        content.destinationSecretCode = this.directoryService.getObjectCode()[content.destinationId]?.value;

        if (!content.isCopy) {
          const name = (this.directories.find(x => x.id == content.destinationId)?.name) || (this.directory.id == content.destinationId ? this.directory.name : '');
          this.transferCloudService.move(content)
            .subscribe(resp => {
              this.isLoading = false;
              if (resp.status == 'success') {
                navigator.clipboard.writeText("");

                setTimeout(() => {
                  this.movingProgressRef.close();
                  if (!StringHelper.isNullOrEmpty(name)) {
                    const message = TranslationService.VALUES['CLOUD']['MOVE_SUCCESS_MSG'].replace('{0}', name.toUpperCase());
                    SnackBar.success(new SnackBarParameter(this, message, 2500));
                  }
                  this.refresh();
                }, 300);
              } else {
                this.movingProgressRef.close();
              }
            });
        }
        else {
          SnackBar.warning(new SnackBarParameter(this, "Chưa hỗ trợ copy"));
          navigator.clipboard.writeText("");
          this.directories.forEach(x => x.underCopyOrCut = false);
          this.cloudFiles.forEach(x => x.underCopyOrCut = false);
          this.isLoading = false;
        }
      } catch (error) {
        this.isLoading = false;
        SnackBar.warning(new SnackBarParameter(this, TranslationService.VALUES['CLOUD']['INVALID_PASTE_CONTENT'], SnackBar.forever));
        return;
      }
    });
  }

  drag(event, item: DirectoryUI | CloudFileUI, type: string) {
    this.dragItem = item;
    this.dragType = type;
  }

  drop(event, dir: DirectoryUI) {
    event.preventDefault();
    navigator.clipboard.writeText(JSON.stringify(this.createCopyCutObject(this.dragItem, this.dragType == "dir", false)));
    this.paste(dir.id);
  }

  openMovingProgess() {
    const config = this.popupService.maxPingConfig(400, 120);
    config.position = { top: '50px' };
    config.panelClass = 'moving-progess';
    this.movingProgressRef = this.dialog.open(MovingProgessComponent, config);
  }
  //#endregion

  //#region other func
  upload(dir: Directory) {
    const config = this.popupService.maxPingConfig(window.innerWidth * 0.8, window.innerHeight * 0.8);
    config.position = { top: '50px' };
    config.panelClass = 'c-uploader';
    config.data = {
      dir: dir ?? this.directory,
      ref: this
    };

    this.shouldRefresh = false;
    this.cloudUploaderRef = this.dialog.open(CloudUploaderComponent, config);
    this.cloudUploaderRef
      .afterClosed()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        if (this.shouldRefresh) {
          this.refresh();
        }
      });
  }

  toDir(id: string) {
    window.location.href = `/${this.Routing.DIRECTORY.path}/${id}`;
  }

  copyPath(cf: CloudFileUI) {
    navigator.clipboard.writeText(cf.url);
    SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES['CLOUD']['DIRECTORY']['COPY_PATH_MSG']));
  }

  viewHistory() {
    this.router.navigateByUrl(`/${this.Routing.SIGN_IN_HISTORY.path}`);
  }
  //#endregion
}
