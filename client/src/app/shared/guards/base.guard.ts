import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AuthService } from '../services/auth/auth.service';
import { StringHelper } from '../helpers/string.helper';
import { Routing } from '../constants/common.constant';
import { AuthUtility } from '../utility/auth-utility';
import { ActionExponent } from '../enumerations/permission.enum';

@Injectable({
  providedIn: 'root' // you can change to any level if needed
})
export class BaseGuard implements CanActivate {
  constructor(public router: Router, public authService: AuthService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (this.authService.isSignedIn()) {
      return this.checkPermission(next);
    }
    if (StringHelper.isNullOrEmpty(this.authService.getUserId())) {
      this.authService.moveOut();
      return of(false);
    }

    return this.authService.ping().pipe(
      switchMap(response => {
        if (response.data === "pong") {
          return this.checkPermission(next);
        }
        this.authService.moveOut();
        return of(false);
      })
    )
  }

  checkPermission(next: ActivatedRouteSnapshot): Observable<boolean> {
    const exponents = (next.data['exponents'] as ActionExponent[]);
    if (!exponents || !exponents.length) {
      return of(true);
    }
    const valid = AuthUtility.checkPermission(exponents);
    if (!valid) {
      this.router.navigateByUrl(`/${Routing.ACCESS_DENIED.path}`);
      return of(false);
    }
    return of(true);
  }
}
