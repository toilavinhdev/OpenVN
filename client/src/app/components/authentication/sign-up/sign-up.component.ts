import { AfterViewInit, Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { CreateAccount } from 'src/app/models/auth/requests/create-account';
import { SignInUpError } from 'src/app/models/auth/requests/sign-in-up-error';
import { UserCred } from 'src/app/models/auth/requests/user-cred';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { Tenant } from 'src/app/models/tenant/tenant';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { BaseButton } from 'src/app/shared/components/micro/button/button.component';
import { CommonRedirect } from 'src/app/shared/constants/common.constant';
import { DeviceType } from 'src/app/shared/enumerations/device.enum';
import { StringHelper } from 'src/app/shared/helpers/string.helper';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { MockDataService } from 'src/app/shared/services/base/mock-data.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { TenantService } from 'src/app/shared/services/tenant/tenant.service';
import { UserService } from 'src/app/shared/services/user/user.serivce';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent extends BaseComponent implements AfterViewInit {

  error = new SignInUpError();

  account = new CreateAccount();

  isShowPassword = true;

  next = '';

  tenant = new Tenant();

  @ViewChild("email") email!: ElementRef;
  @ViewChild("username") username!: ElementRef;
  @ViewChild("password") password!: ElementRef;
  @ViewChild("confirmPassword") confirmPassword!: ElementRef;
  @ViewChild("firstName") firstName!: ElementRef;
  @ViewChild("lastName") lastName!: ElementRef;
  @ViewChild("signupBtn") signupBtn!: BaseButton;

  constructor(
    public injector: Injector,
    public authService: AuthService,
    public userService: UserService,
    public router: Router,
    public mockDataService: MockDataService,
    public tenantService: TenantService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.setBackground();

    this.error.message = 'Đang hỗ trợ đăng ký trải nghiệm nhanh, 1 số thông tin sẽ được tạo tự động';
    const length = this.mockDataService.firstNames.length;
    const length2 = this.mockDataService.lastNames.length;
    const length3 = this.mockDataService.middleNames.length;

    this.account.email = "@gmail.com";
    this.account.password = "123";
    this.account.firstName = this.mockDataService.firstNames[Utility.randomInRange(0, length - 1)];
    this.account.lastName = this.mockDataService.lastNames[Utility.randomInRange(0, length2 - 1)] +
      ' ' +
      this.mockDataService.middleNames[Utility.randomInRange(0, length3 - 1)];
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.email.nativeElement.setSelectionRange(0, 0);
      this.email.nativeElement.focus();
    }, 100);
  }

  initData(): void {
    super.initData();
    this.next = this.activatedRoute.snapshot.queryParams['next'];
    this.loadData();
  }

  setBackground() {
    const start = 1;
    const end = 6;
    const val = Math.floor(Math.random() * (end - start + 1) + start);
    const element = document.getElementById("signup") as HTMLElement;
    element.style.backgroundImage = `url(../../../../assets/img/bg${val}.jpg)`;
  }

  loadData() {
    this.isLoading = true;
    this.tenantService.getAll()
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(resp => {
        this.isLoading = false;
        if (resp.status == 'success' && resp.data) {
          this.tenant = resp.data[0];
        }
      })
  }

  displayUsername() {
    const index = this.account.email.indexOf("@");
    return this.account.email.substring(0, index != -1 ? index : this.account.email.length - 1);
  }

  validateBeforeSignUp() {
    this.error = new SignInUpError();

    this.account.username = this.displayUsername();
    this.account.confirmPassword = this.account.password;
    if (this.account.email.isEmpty()) {
      this.error.isValid = false;
      this.error.type = "email";
      this.error.message = TranslationService.VALUES["SIGN_UP"]["PLEASE_ENTER_EMAIL"];
    } else if (!this.account.email.isMail()) {
      this.error.isValid = false;
      this.error.type = "email";
      this.error.message = TranslationService.VALUES["SIGN_UP"]["PLEASE_ENTER_VALID_EMAIL"];
    }
    else if (this.account.username.isEmpty()) {
      this.error.isValid = false;
      this.error.type = "username";
      this.error.message = TranslationService.VALUES["SIGN_UP"]["PLEASE_ENTER_USERNAME"];
    }
    else if (this.account.password.isEmpty()) {
      this.error.isValid = false;
      this.error.type = "password";
      this.error.message = TranslationService.VALUES["SIGN_UP"]["PLEASE_ENTER_PASSWORD"];
    }
    else if (this.account.confirmPassword.isEmpty()) {
      this.error.isValid = false;
      this.error.type = "confirmPassword";
      this.error.message = TranslationService.VALUES["SIGN_UP"]["PLEASE_ENTER_CONFIRM_PASSWORD"];
    }
    else if (this.account.password != this.account.confirmPassword) {
      this.error.isValid = false;
      this.error.type = "confirmPassword";
      this.error.message = TranslationService.VALUES["SIGN_UP"]["PLEASE_ENTER_SAME_PASSWORD"];
    }
    else if (this.account.firstName.isEmpty()) {
      this.error.isValid = false;
      this.error.type = "firstName";
      this.error.message = TranslationService.VALUES["SIGN_UP"]["PLEASE_ENTER_FIRST_NAME"];
    }
    else if (this.account.lastName.isEmpty()) {
      this.error.isValid = false;
      this.error.type = "lastName";
      this.error.message = TranslationService.VALUES["SIGN_UP"]["PLEASE_ENTER_LAST_NAME"];
    }
    return this.error;
  }

  focusOnFieldError() {
    (this as any)[`${this.error.type}`].nativeElement.focus();
  }

  signUp(event) {
    this.signupBtn.isFinished = false;
    const validate = this.validateBeforeSignUp();
    if (!validate.isValid) {
      this.signupBtn.isFinished = true;
      if (SharedService.DeviceType == DeviceType.Desktop) {
        this.focusOnFieldError();
      }
      return;
    }

    this.account.tenantId = this.tenant.id;
    this.authService
      .signUp(this.account)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.signupBtn.isFinished = true;
          if (resp.status == 'success') {
            SnackBar.success(new SnackBarParameter(this, TranslationService.VALUES["SIGN_UP"]["CREATED_SUCCESS_MSG"], 3000));

            const cred = new UserCred();
            cred.userName = this.account.email;
            cred.password = this.account.password;
            this.authService.signIn(cred)
              .pipe(takeUntil(this._onDestroySub))
              .subscribe(resp => {
                if (resp.status == 'success') {
                  this.authService.saveAuthenticate(resp.accessToken, resp.refreshToken);
                  if (!StringHelper.isNullOrEmpty(this.next)) {
                    window.location.href = this.next;
                  } else {
                    this.router.navigateByUrl(`/${CommonRedirect}`);
                  }
                }
              })
          }
        },
        _ => this.signupBtn.isFinished = true
      );
  }
}
