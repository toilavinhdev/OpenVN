import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { SnackBarParameter } from 'src/app/models/snackbar.param';

@Component({
  selector: 'snackbar',
  template: '',
  styleUrls: []
})
export class SnackBar implements OnInit, OnDestroy {

  private _onDestroySub: Subject<void> = new Subject<void>();

  public static snackBar: MatSnackBar;
  public static forever: number = 999999999;
  public static timeoutId: any;

  constructor(private _snackBar: MatSnackBar) {
    SnackBar.snackBar = this._snackBar;
  }

  ngOnInit(): void {
  }

  private static openMessage(parameter: SnackBarParameter, className: string) {
    clearTimeout(this.timeoutId);

    const hook = document.getElementById("notification-hook");
    const messageBox = (document.querySelector("#notification-hook .message") as HTMLElement);
    let timeout = 0;

    if (hook.classList.contains("opened")) {
      hook.style.opacity = "0";
      hook.removeAllListeners();
      timeout = 200;
    }

    this.timeoutId = setTimeout(() => {
      this.open(hook, messageBox, parameter, className);

      this.timeoutId = setTimeout(() => {
        this.internalClose(hook, messageBox, parameter, className);
      }, parameter.duration);
    }, timeout);
  }

  private static open(hook: HTMLElement, messageBox: HTMLElement, parameter: SnackBarParameter, className: string) {
    hook.style.visibility = "visible";
    hook.style.right = "40px";
    hook.style.opacity = "1";
    hook.classList.remove("success");
    hook.classList.remove("warning");
    hook.classList.remove("danger");
    hook.classList.add(className);
    hook.classList.add("opened");
    messageBox.innerText = parameter.message;

    if (parameter.callback) {
      hook.addEventListener("click", () => parameter.callback());
    } else {
      hook.addEventListener("click", () => {
        this.internalClose(hook, messageBox, parameter, className);
      });
    }
  }

  private static internalClose(hook: HTMLElement, messageBox: HTMLElement, parameter: SnackBarParameter, className: string) {
    hook.style.visibility = "hidden";
    hook.style.right = "-400px";
    hook.classList.remove(className);
    hook.classList.remove("opened");
    messageBox.innerText = "";

    if (parameter.callback) {
      hook.removeAllListeners();
    }
  }

  public static close() {
    const hook = document.getElementById("notification-hook");
    const messageBox = (document.querySelector("#notification-hook .message") as HTMLElement);

    this.internalClose(hook, messageBox, new SnackBarParameter(null, ""), "success");
    this.internalClose(hook, messageBox, new SnackBarParameter(null, ""), "warning");
    this.internalClose(hook, messageBox, new SnackBarParameter(null, ""), "danger");
  }

  public static closeSuccess() {
    const hook = document.getElementById("notification-hook");
    const messageBox = (document.querySelector("#notification-hook .message") as HTMLElement);

    this.internalClose(hook, messageBox, new SnackBarParameter(null, ""), "success");
  }

  public static closeWarning() {
    const hook = document.getElementById("notification-hook");
    const messageBox = (document.querySelector("#notification-hook .message") as HTMLElement);

    this.internalClose(hook, messageBox, new SnackBarParameter(null, ""), "warning");
  }

  public static closeDanger() {
    const hook = document.getElementById("notification-hook");
    const messageBox = (document.querySelector("#notification-hook .message") as HTMLElement);

    this.internalClose(hook, messageBox, new SnackBarParameter(null, ""), "danger");
  }

  /**
   * SnackBar success
   */
  public static success(parameter: SnackBarParameter) {
    this.openMessage(parameter, "success");
  }

  /**
   * SnackBar warning
   */
  public static warning(parameter: SnackBarParameter) {
    this.openMessage(parameter, "warning");
  }


  /**
   * SnackBar danger
   */
  public static danger(parameter: SnackBarParameter) {
    this.openMessage(parameter, "danger");
  }

  /**
   * Dismiss
   */
  public static dismiss() {
    SnackBar.snackBar.dismiss();
  }

  ngOnDestroy(): void {
    // unsubscribe khi destroy
    if (this._onDestroySub) {
      this._onDestroySub.next();
      this._onDestroySub.complete();
      this._onDestroySub.unsubscribe();
    }
  }
}
