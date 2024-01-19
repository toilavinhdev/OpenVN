import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';

@Injectable({
    providedIn: 'root' // you can change to any level if needed
})
export class SignOutGuard implements CanActivate {
    constructor(public router: Router, public authService: AuthService) { }
    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

        if (this.authService.isSignedIn()) {
            return true;
        }
        this.authService.moveOut(false);
        return false;
    }
}
