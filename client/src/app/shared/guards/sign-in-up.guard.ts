import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CommonRedirect, Routing } from '../constants/common.constant';
import { AuthService } from '../services/auth/auth.service';
import { AuthStatus } from '../enumerations/auth-status.enum';

@Injectable({
  providedIn: 'root' // you can change to any level if needed
})
export class SignInUpGuard implements CanActivate {

  constructor(public router: Router, public authService: AuthService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    if (this.authService.getAuthStatus() === AuthStatus.SignedIn) {
      this.router.navigate([`/${CommonRedirect}`]);
      return false;
    }

    return true;
  }
}
