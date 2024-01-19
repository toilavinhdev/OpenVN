import { Location } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, Injector, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from '../../base-component';
import { ButtonColor } from 'src/app/shared/constants/button.constant';
import { Utility } from 'src/app/shared/utility/utility';
import { AuthStatus } from 'src/app/shared/enumerations/auth-status.enum';
import { BaseButton } from '../../micro/button/button.component';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { CommonRedirect, Routing } from 'src/app/shared/constants/common.constant';
import { Event } from 'src/app/shared/constants/event';


interface ModuleHeader {
  moduleName: string;
  path: string;
  iconPosition: string;
  iconCheckedPosition: string;
  hint: string;
}

@Component({
  selector: 'header-v1',
  templateUrl: './header-v1.component.html',
  styleUrls: ['./header-v1.component.scss']
})
export class HeaderV1Component extends BaseComponent implements AfterViewInit {
  ButtonColor = ButtonColor;

  Utility = Utility;

  AuthStatus = AuthStatus;

  _sharedService = this.sharedService;

  @ViewChild("header", { static: true })
  header!: ElementRef;

  @ViewChildren("modules")
  moduleInstances!: QueryList<ElementRef>;

  @ViewChildren("modulesR")
  moduleInstancesR!: QueryList<ElementRef>;

  moduleWidths: number[] = [];

  @ViewChild("signoutBtn")
  signoutBtn!: BaseButton;

  @ViewChild("signinBtn")
  signinBtn!: BaseButton;

  modules: ModuleHeader[] = [];

  currentIndex = -1;

  breakPointIndex = 9999;

  fullName = '';

  avatarUrl = '';

  avatarNameDefault = '';

  timerId: any;

  isLoadingModule = false;

  isLoadingAvatar = true;

  signInStatus = AuthStatus.Unknown;

  constructor(
    public injector: Injector,
    public router: Router,
    public location: Location,
    public authService: AuthService,
    public cdr: ChangeDetectorRef,
    public sharedService: SharedService,
    public translationService: TranslationService,
    public transferService: TransferDataService,
  ) {
    super(injector);
  }


  ngOnInit(): void {
    this.checkSignInStatus();
    if (this.signInStatus === AuthStatus.Unknown) {
      this.signInStatus = AuthStatus.SignedOut;
    }

    super.ngOnInit();
  }

  ngAfterViewInit(): void {
    this.calcToDisplayModules();
    window.addEventListener('resize', () => {
      clearTimeout(this.timerId);
      this.timerId = setTimeout(() => {
        this.calcToDisplayModules();
      }, 100);
    });
  }

  initData() {
    this.setFullName();
    this.intiModules();
    this.findCurrentModule();
    if (this.signInStatus === AuthStatus.SignedIn) {
      this.getAvatarUrl();
    }

    this.transferService.changeScreenEvent
      .pipe(takeUntil(this._onDestroySub))
      .subscribe( () => {
        setTimeout(() => {
          this.findCurrentModule();
        }, 100);
      });
  }

  checkSignInStatus() {
    this.signInStatus = this.authService.getAuthStatus();
  }

  setFullName() {
    const auth = this.authService.getAuth();
    this.fullName = auth.username;
  }

  getAvatarUrl() {
    this.isLoadingAvatar = false;
    this.avatarUrl = 'https://yt3.googleusercontent.com/-CFTJHU7fEWb7BYEb6Jh9gm1EpetvVGQqtof0Rbh-VQRIznYYKJxCaqv_9HeBcmJmIsp2vOO9JU=s900-c-k-c0x00ffffff-no-rj';
    // this.avatarService.getAvatarUrl().subscribe(
    //   response => {
    //     this.isLoadingAvatar = false;
    //     if (response.success) {
    //       if (StringHelper.isNullOrEmpty(response.data)) {
    //         this.avatarNameDefault = this.getAvatarNameDefault();
    //       } else {
    //         this.avatarUrl = response.data;
    //       }

    //     } else {
    //       this.avatarNameDefault = this.getAvatarNameDefault();
    //     }
    //   },
    //   error => {
    //     this.isLoadingAvatar = false;
    //     this.avatarNameDefault = this.getAvatarNameDefault();
    //   }
    // )
  }

  /**
   * Khởi tạo module
   */
  intiModules() {
    this.defaultModules();
    this.calcToDisplayModules();
  }

  defaultModules() {
    this.modules = [];
    this.modules.push({
      path: Routing.NOTEBOOK.path,
      moduleName: Routing.NOTEBOOK.key,
      iconPosition: '-112px -272px',
      iconCheckedPosition: '-112px -288px',
      hint: '',
    });
  }

  calcToDisplayModules() {
    if (!this.moduleInstances || this.moduleInstances.length === 0)
      return;

    if (!this.moduleWidths.length)
      this.moduleWidths = (this.moduleInstances as any)["_results"].map((instance: any) => (instance.nativeElement as HTMLElement).offsetWidth);

    const screenWidth = window.innerWidth;
    let sumWidth = 0;

    this.breakPointIndex = 9999;
    for (let index = 0; index < this.moduleWidths.length; index++) {
      const width = this.moduleWidths[index];
      sumWidth += width;
      if (sumWidth + 128 >= screenWidth) {
        this.breakPointIndex = index;
        break;
      }
    }
    this.cdr.detectChanges();
  }

  /**
   * Bay tới phân hệ chỉ định
   */
  routeUrl(path: string) {
    this.router.navigateByUrl(`/${path}`);
  }

  findCurrentModule() {
    const path = this.location.path();

    // tìm chính xác trước, nếu không thấy tìm startsWith
    const pathSplit = path.split('/');
    if (pathSplit.length >= 1) {
      const index = this.modules.findIndex((m) => m.path === pathSplit[1]);
      if (index !== -1) {
        this.currentIndex = index;
        return;
      }
    }

    this.currentIndex = this.modules.findIndex((m) => path.startsWith(`/${m.path}`)) || -1;
  }

  signOut() {
    this.tracking(this.commonTrackingEvent(Event.CLICK_SIGN_OUT_BUTTON));
    this.authService.signOut(() => {
      this.signoutBtn.isFinished = true;
      this.authService.moveOut(false);
    });
  }

  toCommonPage() {
    this.router.navigate([`/${CommonRedirect}`]);
  }

  toSignInPage() {
    this.router.navigate([`/${Routing.SIGN_IN.path}`]);
    this.signinBtn.isFinished = true;
  }

  redirect(path: string, index: number) {
    this.currentIndex = index;
    this.router.navigateByUrl(`/${path}`);
  }

  openUpdateAvatarPopup(event) {
    Utility.featureIsInDevelopment(event);
  }
}
