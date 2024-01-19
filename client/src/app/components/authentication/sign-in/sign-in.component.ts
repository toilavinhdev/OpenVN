import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { SignInUpError } from 'src/app/models/auth/requests/sign-in-up-error';
import { UserCred } from 'src/app/models/auth/requests/user-cred';
import { AuthResponse } from 'src/app/models/auth/responses/auth-response';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { BaseButton } from 'src/app/shared/components/micro/button/button.component';
import { SnackBar } from 'src/app/shared/components/element/snackbar/snackbar.component';
import { CommonRedirect } from 'src/app/shared/constants/common.constant';
import { Event } from 'src/app/shared/constants/event';
import { DeviceType } from 'src/app/shared/enumerations/device.enum';
import { StringHelper } from 'src/app/shared/helpers/string.helper';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { HubConnectionService } from 'src/app/shared/services/base/hub-connection.service';
import { SharedService } from 'src/app/shared/services/base/shared.service';
import { TransferDataService } from 'src/app/shared/services/base/transfer-data.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';
import { Utility } from 'src/app/shared/utility/utility';
import { environment } from 'src/environments/environment';

declare var google: any;
declare var gapi: any;

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent extends BaseComponent implements AfterViewInit {

  Utility = Utility;

  registerUrl = "#";

  userCred = new UserCred();

  error = new SignInUpError();

  next = '';

  @ViewChild("username") usernameInput!: ElementRef;

  @ViewChild("password") passwordInput!: ElementRef;

  @ViewChild("signinBtn") signinBtn!: BaseButton;

  @ViewChild("googleBtn") googleBtn!: ElementRef;

  constructor(
    public injector: Injector,
    private transferService: TransferDataService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private hubService: HubConnectionService
    // private socialAuthService: SocialAuthService
  ) {
    super(injector);
  }

  initData(): void {
    this.setBackground();
    this.transferService.resolvedEvent.emit();
    this.next = this.activatedRoute.snapshot.queryParams['next'];
  }

  ngAfterViewInit(): void {
    this.initForm();
    this.cdr.detectChanges();
    // google.accounts.id.initialize({
    //   client_id: "483405168173-ng58air6q83sgnmeo80sc6g0gsm2nmpf.apps.googleusercontent.com",
    //   callback: (response: any) => this.handleGoogleSignIn(response)
    // });
    // google.accounts.id.renderButton(
    //   document.getElementById("googleBtn"),
    //   {
    //     theme: 'outline',
    //     size: 'large',
    //     width: this.googleBtn.nativeElement.offsetWidth,
    //     text: 'Sign in with Google',
    // }  // customization attributes
    // );
  }

  initServices(): void {
    super.initServices();
  }

  handleGoogleSignIn(response: any) {
    // gapi.load('auth2', function() {
    //   gapi.auth2.init({'client_id': '483405168173-ng58air6q83sgnmeo80sc6g0gsm2nmpf.apps.googleusercontent.com'});
    // });

    // // This next is for decoding the idToken to an object if you want to see the details.
    // let base64Url = response.credential.split('.')[1];
    // let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    // let jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
    //   return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    // }).join(''));
  }

  signout() {
    gapi.auth2.getAuthInstance().signOut();
  }

  initForm() {
    if (true || environment.production) {
      this.userCred.userName = "guest";
      this.userCred.password = "123";
    }
    if (SharedService.DeviceType == DeviceType.Desktop) {
      this.usernameInput.nativeElement.focus();
    }
  }

  setBackground() {
    const start = 1;
    const end = 6;
    const val = Math.floor(Math.random() * (end - start + 1) + start);
    const element = document.getElementById("signin") as HTMLElement;
    element.style.backgroundImage = `url(../../../../assets/img/bg${val}.jpg)`;
  }

  focusOnFieldError() {
    (this as any)[`${this.error.type}Input`].nativeElement.focus();
  }

  validateBeforeSignIn() {
    this.error = new SignInUpError();

    if (this.userCred.userName.isEmpty()) {
      this.error.isValid = false;
      this.error.type = "username";
      this.error.message = TranslationService.VALUES["SIGN_IN"]["PLEASE_ENTER_USERNAME"];
    }
    else if (this.userCred.password.isEmpty()) {
      this.error.isValid = false;
      this.error.type = "password";
      this.error.message = TranslationService.VALUES["SIGN_IN"]["PLEASE_ENTER_PASSWORD"];
    }
    return this.error;
  }

  signIn(e: any) {
    this.tracking(this.commonTrackingEvent(Event.CLICK_SIGN_IN_BUTTON));
    const validate = this.validateBeforeSignIn();
    if (!validate.isValid) {
      this.signinBtn.isFinished = true;
      if (SharedService.DeviceType == DeviceType.Desktop) {
        this.focusOnFieldError();
      }
      return;
    }

    SnackBar.dismiss();
    this.authService.signIn(this.userCred).pipe(takeUntil(this._onDestroySub)).subscribe(
      response => {
        this.signinBtn.isFinished = true;
        if (response.status == "success") {
          this.handleLoggedIn(response);
        } else {
          this.error.isValid = false;
          this.error.message = response.error.message;
          this.error.type = "signInError";
        }
      },
      () => {
        this.signinBtn.isFinished = true;
        this.error.isValid = false;
        this.error.message = TranslationService.VALUES["ERROR"]["UNKNOWN_MSG"];
        this.error.type = "unkown";
      }
    );
  }

  signinByEnter(e: any) {
    if (e.key === "Enter") {
      this.signinBtn.clickExecute(e);
    }
  }

  handleLoggedIn(response: AuthResponse) {
    this.authService.saveAuthenticate(response.accessToken, response.refreshToken);
    if (!StringHelper.isNullOrEmpty(this.next)) {
      window.location.href = this.next;
    } else {
      this.router.navigateByUrl(`/${CommonRedirect}`);
    }
  }

  signinWithGoogle(): void {
    // this.socialAuthService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }
}
