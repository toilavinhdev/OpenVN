import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonRedirect, Routing } from 'src/app/shared/constants/common.constant';
import { StringHelper } from 'src/app/shared/helpers/string.helper';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-token-receiver',
  templateUrl: './token-receiver.component.html',
  styleUrls: ['./token-receiver.component.scss']
})
export class TokenReceiverComponent implements OnInit {

  accessToken = "";

  refreshToken = "";

  next = "";

  constructor(
    public authSerice: AuthService,
    public activatedRoute: ActivatedRoute,
    public router: Router
  ) { }

  ngOnInit(): void {
    Utility.changeTitle(Routing.TOKEN_RECEIVER.key);

    this.accessToken = this.activatedRoute.snapshot.queryParams['t'];
    this.refreshToken = this.activatedRoute.snapshot.queryParams['r'];
    this.next = this.activatedRoute.snapshot.queryParams['n'];

    if (StringHelper.isNullOrEmpty(this.accessToken) || StringHelper.isNullOrEmpty(this.refreshToken)) {
      console.customize("token not found");
      this.router.navigateByUrl(`/${Routing.NOT_FOUND.path}`);
    } else {
      console.customize("token received");
      this.authSerice.saveAuthenticate(this.accessToken, this.refreshToken);
      if (!StringHelper.isNullOrEmpty(this.next)) {
        window.location.href = this.next;
      } else {
        this.router.navigateByUrl(`/${CommonRedirect}`);
      }
    }
  }

}
