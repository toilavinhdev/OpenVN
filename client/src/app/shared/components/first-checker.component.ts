import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { CommonRedirect } from '../constants/common.constant';
import { AuthStatus } from '../enumerations/auth-status.enum';
import { AuthService } from '../services/auth/auth.service';

@Component({
  selector: 'first-check',
  template: '',
  styles: []
})
export class FirstCheckerComponent implements OnInit {

  constructor(public router: Router, public authService: AuthService) { }

  ngOnInit(): void {
    const status = this.authService.getAuthStatus();
    console.customize(`first-check`, AuthStatus[status]);

    setTimeout(() => {
      if (environment.app_allows_guests || status === AuthStatus.SignedIn) {
        this.router.navigate([`/${CommonRedirect}`]);
      } else {
        this.authService.moveOut();
      }
    }, 10);
  }
}
